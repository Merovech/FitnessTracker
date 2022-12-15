using FitnessTracker.Core.Services.Implementations;
using FitnessTracker.Core.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace FitnessTracker.Core.Tests.Helpers.Builders
{
	public class SettingsServiceBuilder : IBuilder<ISettingsService>
	{
		public IConfigurationService ConfigurationService
		{
			get;
			set;
		}

		public ILogger<SettingsService> Logger
		{
			get;
			set;
		}

		public SettingsServiceBuilder()
		{
			CreateMocks();
		}

		public ISettingsService Build()
		{
			return new SettingsService(ConfigurationService, Logger);
		}

		private void CreateMocks()
		{
			var mock = new Mock<IConfigurationService>();
			mock.Setup(svc => svc.SettingsFileName).Returns(Constants.TEST_SETTINGS_FILENAME);
			ConfigurationService = mock.Object;

			Logger = new Mock<ILogger<SettingsService>>().Object;
		}
	}
}
