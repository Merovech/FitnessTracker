using FitnessTracker.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace FitnessTracker.UI
{
	public class ViewModelLocator
	{
		public MainViewModel MainViewModel => App.ServiceProvider.GetService<MainViewModel>();

		public RawDataViewModel RawDataViewModel => App.ServiceProvider.GetService<RawDataViewModel>();

		public WeightChartViewModel WeightChartViewModel => App.ServiceProvider.GetService<WeightChartViewModel>();

		public SummaryViewModel SummaryViewModel => App.ServiceProvider.GetService<SummaryViewModel>();

		public AddEditDataViewModel AddEditDataViewModel => App.ServiceProvider.GetService<AddEditDataViewModel>();

		public SettingsViewModel SettingsViewModel => App.ServiceProvider.GetService<SettingsViewModel>();

		public ImportViewModel ImportViewModel => App.ServiceProvider.GetService<ImportViewModel>();
	}
}
