using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using FitnessTracker.Core;
using FitnessTracker.Core.Models;
using FitnessTracker.Core.Services.Interfaces;
using FitnessTracker.UI.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Extensions.Logging;

namespace FitnessTracker.UI
{
	public partial class App : Application
	{
		private readonly Logger _logger;

		private readonly string[] _loadableAssemblyNames = new[]
		{
			"FitnessTracker.UI",
			"FitnessTracker.Core"
		};

		public static ServiceProvider ServiceProvider
		{
			get; private set;
		}

		public IConfiguration Configuration
		{
			get; private set;
		}

		public App()
		{
			var serviceCollection = new ServiceCollection();
			var configBuilder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", false, true);

			Configuration = configBuilder.Build();
			serviceCollection.Configure<ApplicationSettings>(Configuration.GetSection("settings"));
			LogManager.Configuration = new NLogLoggingConfiguration(Configuration.GetSection("NLog"));
			serviceCollection.AddLogging(builder =>
			{
				builder.ClearProviders();
				builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
				builder.AddNLog();
			});

			NLog.LogLevel logLevel = Configuration.GetSection("settings")["logLevel"] switch
			{
				"Debug" => NLog.LogLevel.Debug,
				"Trace" => NLog.LogLevel.Trace,
				_ => NLog.LogLevel.Debug
			};

			SetNLogLevel(logLevel);

			_logger = LogManager.GetCurrentClassLogger();
			_logger.Debug("Application startup");
			RegisterInjectables(serviceCollection);
			ServiceProvider = serviceCollection.BuildServiceProvider();
			Task.Run(async () => await VerifyOrCreateDatabase()).Wait();
		}

		private void OnStartup(object sender, StartupEventArgs e)
		{
			var mainWindow = ServiceProvider.GetService<MainWindowView>();
			mainWindow.Show();
		}

		private void OnExit(object sender, ExitEventArgs e)
		{
			_logger.Debug("Application exit");
		}

		private void SetNLogLevel(NLog.LogLevel level)
		{
			if (level == NLog.LogLevel.Debug)
			{
				GlobalDiagnosticsContext.Set("GlobalDebugLevel", "Debug");
				GlobalDiagnosticsContext.Set("GlobalTraceLevel", "Off");
			}
			else if (level == NLog.LogLevel.Trace)
			{
				GlobalDiagnosticsContext.Set("GlobalDebugLevel", "Off");
				GlobalDiagnosticsContext.Set("GlobalTraceLevel", "Trace");
			}

			LogManager.ReconfigExistingLoggers();
		}

		private void RegisterInjectables(IServiceCollection serviceCollection)
		{
			var injectables = Assembly.GetExecutingAssembly().ExportedTypes.ToList();
			injectables.AddRange(GetAllReferencedAssemblyTypes());
			_logger.Trace("DI Registration: Found {count} potential injectables.", injectables.Count);

			List<Type> interfaceList = new();
			List<Type> implementationList = new();
			List<Type> singletonList = new();
			List<Type> otherList = new();

			foreach (var inj in injectables)
			{
				AddTypeForInjection(inj, interfaceList, implementationList, singletonList, otherList);
			}

			// We can do this in one loop, using the larger of the interface and other lists as our counter.
			// We're taking advantage of the fact that interfaces and implementations should have the same name,
			// one with an I prefix on the interface, so they can be done in parallel.
			int i = 0;
			while (i < interfaceList.Count || i < singletonList.Count || i < otherList.Count)
			{
				if (i < interfaceList.Count)
				{
					_logger.Debug("DI Registration: Registering service {interfacename}, {implementationname}", interfaceList[i].Name, implementationList[i].Name);
					serviceCollection.AddTransient(interfaceList[i], implementationList[i]);
				}

				if (i < singletonList.Count)
				{
					_logger.Debug("DI Registration: Registering singleton {singleton}", singletonList[i].Name);
					serviceCollection.AddSingleton(singletonList[i]);
				}

				if (i < otherList.Count)
				{
					_logger.Debug("DI Registration: Registering other {other}", otherList[i].Name);
					serviceCollection.AddTransient(otherList[i]);
				}

				i++;
			}
		}

		private void AddTypeForInjection(Type t, List<Type> interfaceList, List<Type> implementationList, List<Type> singletonList, List<Type> otherList)
		{
			// Check to see if there's a DependencyInjectionType attribute
			var attribute = t.GetCustomAttribute<DependencyInjectionTypeAttribute>();
			if (attribute == null)
			{
				_logger.Trace("DI Registration: Ignoring {other}", t.Name);
				return;
			}

			switch (attribute.Type)
			{
				case DependencyInjectionType.Service:
					_logger.Trace("DI Registration: Found service implementation {svc}", t.Name);
					implementationList.Add(t);
					break;

				case DependencyInjectionType.Interface:
					_logger.Trace("DI Registration: Found service interface {interface}", t.Name);
					interfaceList.Add(t);
					break;

				case DependencyInjectionType.Singleton:
					_logger.Trace("DI Registration: Found singleton {singleton}", t.Name);
					singletonList.Add(t);
					break;

				case DependencyInjectionType.Other:
					_logger.Trace("DI Registration: Found other {other}", t.Name);
					otherList.Add(t);
					break;

				case DependencyInjectionType.None:
				default:
					_logger.Trace("DI Registration: Item {item} has an injection type of None and will be ignored", t.Name);
					break;
			}
		}

		private async Task VerifyOrCreateDatabase()
		{
			var config = ServiceProvider.GetService<IOptions<ApplicationSettings>>();

			if (!File.Exists(config.Value.DataFileName))
			{
				_logger.Debug("No database found.  Creating a new one.");
				var dbService = ServiceProvider.GetService<IDatabaseService>();
				await dbService.CreateDatabase();
			}
			else
			{
				_logger.Debug("Found existing database.");
			}
		}

		private List<Type> GetAllReferencedAssemblyTypes()
		{
			var assemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies();
			var validAssemblies = assemblies.Where(a => _loadableAssemblyNames.Contains(a.Name)).ToList();
			_logger.Trace("Valid assemblies: {assemblies}", string.Join(",", validAssemblies));

			var returnList = new List<Type>();
			foreach (var assm in validAssemblies)
			{
				var loadedAssembly = Assembly.Load(assm);
				returnList.AddRange(loadedAssembly.ExportedTypes);
			}

			return returnList;
		}
	}
}
