using FitnessTracker.Core.Services.Implementations;
using FitnessTracker.Core.Services.Interfaces;
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

		public SettingsServiceBuilder()
		{
			CreateMocks();
		}

		public ISettingsService Build()
		{
			return new SettingsService(ConfigurationService);
		}

		private void CreateMocks()
		{
			var mock = new Mock<IConfigurationService>();
			mock.Setup(svc => svc.SettingsFilename).Returns(Constants.TEST_SETTINGS_FILENAME);
			ConfigurationService = mock.Object;
		}
	}
}
