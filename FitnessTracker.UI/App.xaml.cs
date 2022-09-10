using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using FitnessTracker.Core.ImportPreparer.Implementations;
using FitnessTracker.Core.ImportPreparer.Interfaces;
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
		private readonly string[] loadableAssemblyNames = new[]
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

			var logger = LogManager.GetCurrentClassLogger();
			logger.Debug("Application startup");
			RegisterInjectables(serviceCollection, logger);
			ServiceProvider = serviceCollection.BuildServiceProvider();
			Task.Run(async () => await VerifyOrCreateDatabase(logger)).Wait();
		}

		private void OnStartup(object sender, StartupEventArgs e)
		{
			var mainWindow = ServiceProvider.GetService<MainWindowView>();
			mainWindow.Show();
		}

		private void OnExit(object sender, ExitEventArgs e)
		{
			var logger = LogManager.GetCurrentClassLogger();
			logger.Debug("Application exit");
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

		private void RegisterInjectables(IServiceCollection serviceCollection, Logger logger)
		{
			var types = Assembly.GetExecutingAssembly().ExportedTypes.ToList();
			types.AddRange(GetAllReferencedAssemblyTypes());
			logger.Trace("DI Registration: Found {count} potential injectables.", types.Count);

			var serviceInterfaces = new List<Type>();
			var serviceImplementations = new List<Type>();

			// MainViewModel is a special case, as it's the entry point for the app
			logger.Debug("DI Registration: MainViewModel (singleton)");
			var mainViewModel = types.FirstOrDefault(t => t.Name == nameof(MainWindowView));
			serviceCollection.AddSingleton(mainViewModel);

			// One-offs - not really a service, so looping through services and view models would catch these
			serviceCollection.AddTransient<IImportPreparerFactory, ImportPreparerFactory>();

			// ViewModels and Services
			foreach (var t in types)
			{
				if (t.Name.EndsWith("ViewModel", StringComparison.OrdinalIgnoreCase))
				{
					logger.Debug("DI Registration: Registering view model {vm}", t.Name);
					serviceCollection.AddTransient(t);
				}
				else if (t.IsInterface && t.Name.EndsWith("Service", StringComparison.OrdinalIgnoreCase))
				{
					logger.Trace("DI Registration: Found {svci} (service interface)", t.Name);
					serviceInterfaces.Add(t);
				}
				else if (!t.IsInterface && t.Name.EndsWith("Service", StringComparison.OrdinalIgnoreCase))
				{
					logger.Trace("DI Registration: Found {svc} (service implementation)", t.Name);
					serviceImplementations.Add(t);
				}
				else
				{
					// Ignore the rest of the types in the assembly
					logger.Trace("DI Registration: Ignoring {item}", t.Name);
				}
			}

			RegisterServices(serviceCollection, serviceInterfaces.OrderBy(t => t.Name).ToList(), serviceImplementations.OrderBy(t => t.Name).ToList(), logger);
		}

		private void RegisterServices(IServiceCollection serviceCollection, List<Type> interfaces, List<Type> implementations, Logger logger)
		{
			// We're cheating here and taking advantage of the fact that every service has a corresponding interface.  So if
			// we take a sorted list of services implementations and a sorted list of service interfaces, they'll match up.
			for (var i = 0; i < interfaces.Count; i++)
			{
				logger.Debug("DI Registration: Registering service {interface}/{implementation}", interfaces[i].Name, implementations[i].Name);
				serviceCollection.AddTransient(interfaces[i], implementations[i]);
			}
		}

		private async Task VerifyOrCreateDatabase(Logger logger)
		{
			var config = ServiceProvider.GetService<IOptions<ApplicationSettings>>();

			if (!File.Exists(config.Value.DataFileName))
			{
				logger.Debug("No database found.  Creating a new one.");
				var dbService = ServiceProvider.GetService<IDatabaseService>();
				await dbService.CreateDatabase();
			}
			else
			{
				logger.Debug("Found existing database.");
			}
		}

		private List<Type> GetAllReferencedAssemblyTypes()
		{
			var assemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies();
			var validAssemblies = assemblies.Where(a => loadableAssemblyNames.Contains(a.Name)).ToList();
			Debug.WriteLine($"Found {validAssemblies.Count} assemblies with types to load.  Expected {loadableAssemblyNames.Length}.");

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
