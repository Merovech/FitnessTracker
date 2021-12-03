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
using FitnessTracker.Core.Services.Interfaces;
using FitnessTracker.UI.Views;
using Microsoft.Extensions.DependencyInjection;

namespace FitnessTracker.UI
{
	public partial class App : Application
	{
		private readonly string[] loadableAssemblyNames = new[]
		{
			"FitnessTracker.UI",
			"FitnessTracker.Core"
		};

		public static ServiceProvider ServiceProvider { get; private set; }

		public App()
		{
			var serviceCollection = new ServiceCollection();
			RegisterInjectables(serviceCollection);
			ServiceProvider = serviceCollection.BuildServiceProvider();
			Task.Run(async () => await VerifyOrCreateDatabase()).Wait();
		}

		private void OnStartup(object sender, StartupEventArgs e)
		{
			var mainWindow = ServiceProvider.GetService<MainWindowView>();
			mainWindow.Show();

			base.OnStartup(e);
		}

		private void RegisterInjectables(IServiceCollection serviceCollection)
		{
			var types = Assembly.GetExecutingAssembly().GetTypes().ToList();
			types.AddRange(GetAllReferencedAssemblyTypes());

			var serviceInterfaces = new List<Type>();
			var serviceImplementations = new List<Type>();

			// MainViewModel is a special case, as it's the entry point for the app
			var mainViewModel = types.FirstOrDefault(t => t.Name == nameof(MainWindowView));
			serviceCollection.AddSingleton(mainViewModel);

			// One-offs - not really a service, so looping through services and view models would catch these
			serviceCollection.AddTransient<IImportPreparerFactory, ImportPreparerFactory>();

			// ViewModels and Services
			foreach (var t in types)
			{
				if (t.Name.EndsWith("ViewModel", StringComparison.OrdinalIgnoreCase))
				{
					Debug.WriteLine($"Registering view model: {t.Name}.");
					serviceCollection.AddTransient(t);
				}
				else if (t.IsInterface && t.Name.EndsWith("Service", StringComparison.OrdinalIgnoreCase))
				{
					Debug.WriteLine($"Registering service interface: {t.Name}.");
					serviceInterfaces.Add(t);
				}
				else if (!t.IsInterface && t.Name.EndsWith("Service", StringComparison.OrdinalIgnoreCase))
				{
					Debug.WriteLine($"Registering service: {t.Name}.");
					serviceImplementations.Add(t);
				}

				// Ignore the rest of the types in the assembly
			}

			RegisterServices(serviceCollection, serviceInterfaces.OrderBy(t => t.Name).ToList(), serviceImplementations.OrderBy(t => t.Name).ToList());
		}

		private void RegisterServices(IServiceCollection serviceCollection, List<Type> interfaces, List<Type> implementations)
		{
			// We're cheating here and taking advantage of the fact that every service has a corresponding interface.  So if
			// we take a sorted list of services implementations and a sorted list of service interfaces, they'll match up.
			for (var i = 0; i < interfaces.Count; i++)
			{
				Debug.WriteLine($"Registering service: {interfaces[i].Name}, {implementations[i].Name}");
				serviceCollection.AddTransient(interfaces[i], implementations[i]);
			}
		}

		private async Task VerifyOrCreateDatabase()
		{
			var configService = ServiceProvider.GetService<IConfigurationService>();

			if (!File.Exists(configService.DataFileName))
			{
				var dbService = ServiceProvider.GetService<IDatabaseService>();
				await dbService.CreateDatabase();
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
				returnList.AddRange(loadedAssembly.GetTypes());
			}

			return returnList;
		}
	}
}
