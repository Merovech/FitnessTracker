using System;
using System.Collections.Generic;
using System.Linq;
using FitnessTracker.Core.Models;
using FitnessTracker.Core.Services.Implementations;
using FitnessTracker.Core.Services.Interfaces;
using FitnessTracker.Services.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FitnessTracker.Core.Tests.Helpers.Builders
{
	public class DatabaseServiceBuilder : IBuilder<IDatabaseService>
	{
		private Mock<IDataCalculatorService> _dataCalculatorServiceMock;

		public IConfigurationService ConfigurationService
		{
			get;
			set;
		}

		public IDataCalculatorService DataCalculatorService
		{
			get;
			set;
		}

		public DatabaseServiceBuilder()
		{
			SetupMocks();
		}

		public IDatabaseService Build()
		{
			return new DatabaseService(DataCalculatorService, ConfigurationService);
		}

		public void VerifyRecordListsAreEqual(List<DailyRecord> expected, List<DailyRecord> actual)
		{
			Assert.IsTrue(expected.SequenceEqual(actual, new DailyRecordEqualityComparer()), "Actual and expected lists were not equal.");
		}

		public void VerifyDataCalculatorServiceWasCalled(int expectedCallCount)
		{
			_dataCalculatorServiceMock.Verify(svc => svc.FillCalculatedDataFields(It.IsAny<IEnumerable<DailyRecord>>()), Times.Exactly(expectedCallCount));
		}

		private void SetupMocks()
		{
			var configMock = new Mock<ConfigurationService>() { CallBase = true };
			configMock.Setup(svc => svc.DataFileName).Returns(Constants.TEST_DATABASE_FILENAME);
			ConfigurationService = configMock.Object;

			_dataCalculatorServiceMock = new Mock<IDataCalculatorService>();
			DataCalculatorService = _dataCalculatorServiceMock.Object;
		}
	}
}
