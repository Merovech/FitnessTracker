using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using FitnessTracker.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FitnessTracker
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static ServiceProvider ServiceProvider;

		public App()
		{
			var serviceCollection = new ServiceCollection();
			RegisterInjectables(serviceCollection);
			ServiceProvider = serviceCollection.BuildServiceProvider();
		}

		private void OnStartup(object sender, StartupEventArgs e)
		{
			var mainWindow = ServiceProvider.GetService<MainWindowView>();
			mainWindow.Show();

			base.OnStartup(e);
		}

		private void RegisterInjectables(IServiceCollection serviceCollection)
		{
			var assembly = this.GetType().Assembly;
			var types = assembly.GetTypes();

			// Build a dictionary of each type we need
			var mappings = new Dictionary<Type, Type>();
			var dataContexts = types.Where(t => t.IsSubclassOf(typeof(DbContext)));
			var serviceInterfaces = types.Where(t => t.IsInterface && t.Name.EndsWith("Service")).OrderBy(t => t.Name).ToList();
			var serviceImplementations = types.Where(t => !t.IsInterface && t.Name.EndsWith("Service")).OrderBy(t => t.Name).ToList();
			var viewModels = types.Where(t => t.Name.EndsWith("ViewModel"));

			// MainViewModel is a special case, as it's the entry point for the app
			var mainViewModel = types.FirstOrDefault(t => t.Name == nameof(MainWindowView));
			serviceCollection.AddSingleton(mainViewModel);

			RegisterDataContexts(serviceCollection, dataContexts);
			RegisterServices(serviceCollection, serviceInterfaces, serviceImplementations);
			RegisterViewModels(serviceCollection, viewModels);
		}

		private void RegisterDataContexts(IServiceCollection serviceCollection, IEnumerable<Type> dataContextTypes)
		{
			foreach (var dc in dataContextTypes)
			{
				Debug.WriteLine($"Registering data context: {dc.Name}");
				serviceCollection.AddTransient(dc);
			}
		}

		private void RegisterServices(IServiceCollection serviceCollection, List<Type> interfaces, List<Type> implementations)
		{
			// We're cheating here and taking advantage of the fact that every service has a corresponding interface.  So if
			// we take a sorted list of services implementations and a sorted list of service interfaces, they'll match up.
			for (int i = 0; i < interfaces.Count; i++)
			{
				Debug.WriteLine($"Registering service: {interfaces[i].Name}, {implementations[i].Name}");
				serviceCollection.AddTransient(interfaces[i], implementations[i]);
			}
		}

		private void RegisterViewModels(IServiceCollection serviceCollection, IEnumerable<Type> viewModels)
		{
			foreach (var vm in viewModels)
			{
				Debug.WriteLine($"Registering view model: {vm.Name}");
				serviceCollection.AddSingleton(vm);
			}
		}
	}
}
