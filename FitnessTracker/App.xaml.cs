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

			var serviceInterfaces = new List<Type>();
			var serviceImplementations = new List<Type>();

			// MainViewModel is a special case, as it's the entry point for the app
			var mainViewModel = types.FirstOrDefault(t => t.Name == nameof(MainWindowView));
			serviceCollection.AddSingleton(mainViewModel);

			foreach (var t in types)
			{
				if (t.IsSubclassOf(typeof(DbContext)) || t.Name.EndsWith("ViewModel"))
				{
					// Register VMs and DbContexts straightaway.  No further processing needed.
					Debug.WriteLine($"Registering type: {t.Name}");
					serviceCollection.AddTransient(t);
				}
				else if (t.IsInterface && t.Name.EndsWith("Service"))
				{
					serviceInterfaces.Add(t);
				}
				else if (!t.IsInterface && t.Name.EndsWith("Service"))
				{
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
			for (int i = 0; i < interfaces.Count; i++)
			{
				Debug.WriteLine($"Registering service: {interfaces[i].Name}, {implementations[i].Name}");
				serviceCollection.AddTransient(interfaces[i], implementations[i]);
			}
		}
	}
}
