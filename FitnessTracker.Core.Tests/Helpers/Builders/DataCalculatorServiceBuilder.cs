using FitnessTracker.Core.Services.Implementations;
using FitnessTracker.Core.Services.Interfaces;

namespace FitnessTracker.Core.Tests.Helpers.Builders
{
	public class DataCalculatorServiceBuilder : IBuilder<IDataCalculatorService>
	{
		public IDataCalculatorService Build()
		{
			return new DataCalculatorService();
		}
	}
}
