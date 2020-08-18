using System.Windows;
using FitnessTracker.Models;
using FitnessTracker.Services.Implementations;
using FitnessTracker.Services.Interfaces;
using FitnessTracker.ViewModels;
using FitnessTracker.Views;
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
			ConfigureServices(serviceCollection);
			ServiceProvider = serviceCollection.BuildServiceProvider();
		}

		private void ConfigureServices(IServiceCollection services)
		{
			// TODO: Reflection to load this using naming conventions
			RegisterViews(services);
			RegisterViewModels(services);
			RegisterDatabaseContexts(services);
			RegisterServices(services);
		}

		private void RegisterViews(IServiceCollection services)
		{
			services.AddSingleton<MainWindowView>();
		}

		private void RegisterViewModels(IServiceCollection services)
		{
			services.AddSingleton<MainViewModel>();
			services.AddSingleton<WeightChartViewModel>();
			services.AddSingleton<MovementChartViewModel>();
			services.AddSingleton<RawDataViewModel>();
			services.AddSingleton<SummaryViewModel>();
			services.AddSingleton<AddEditDataViewModel>();
		}

		private void RegisterDatabaseContexts(IServiceCollection services)
		{
			services.AddTransient<DatabaseContext>();
		}

		private void RegisterServices(IServiceCollection services)
		{
			services.AddTransient<IDatabaseService, DatabaseService>();
			services.AddTransient<IDataImporterService, DataImporterService>();
			services.AddTransient<IDataCalculatorService, DataCalculatorService>();
			services.AddTransient<IFileDialogService, FileDialogService>();
			services.AddTransient<IConfigurationService, ConfigurationService>();
		}

		private void OnStartup(object sender, StartupEventArgs e)
		{
			var mainWindow = ServiceProvider.GetService<MainWindowView>();
			mainWindow.Show();

			base.OnStartup(e);
		}
	}
}
