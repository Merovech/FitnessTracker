using Microsoft.Extensions.DependencyInjection;

namespace FitnessTracker.ViewModels
{
	public class ViewModelLocator
	{
		public MainViewModel MainViewModel => App.ServiceProvider.GetService<MainViewModel>();

		public DebugViewModel DebugViewModel => App.ServiceProvider.GetService<DebugViewModel>();

		public RawDataViewModel RawDataViewModel => App.ServiceProvider.GetService<RawDataViewModel>();

		public WeightChartViewModel WeightChartViewModel => App.ServiceProvider.GetService<WeightChartViewModel>();
	}
}
