using FitnessTracker.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace FitnessTracker.Utilities
{
	public class ViewModelLocator
	{
		public MainViewModel MainViewModel => App.ServiceProvider.GetService<MainViewModel>();

		public RawDataViewModel RawDataViewModel => App.ServiceProvider.GetService<RawDataViewModel>();

		public WeightChartViewModel WeightChartViewModel => App.ServiceProvider.GetService<WeightChartViewModel>();

		public MovementChartViewModel MovementChartViewModel => App.ServiceProvider.GetService<MovementChartViewModel>();

		public SummaryViewModel SummaryViewModel => App.ServiceProvider.GetService<SummaryViewModel>();

		public AddEditDataViewModel AddEditDataViewModel => App.ServiceProvider.GetService<AddEditDataViewModel>();
	}
}
