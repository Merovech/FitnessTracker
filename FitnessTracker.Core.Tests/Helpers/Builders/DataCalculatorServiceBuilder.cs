using FitnessTracker.Core.Services.Implementations;
using FitnessTracker.Core.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace FitnessTracker.Core.Tests.Helpers.Builders
{
	public class DataCalculatorServiceBuilder : IBuilder<IDataCalculatorService>
	{
		public IDataCalculatorService Build()
		{
			var loggerMock = new Mock<ILogger<DataCalculatorService>>();
			return new DataCalculatorService(loggerMock.Object);
		}
	}
}
