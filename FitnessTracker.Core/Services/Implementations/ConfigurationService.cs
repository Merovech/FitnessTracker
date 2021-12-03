using FitnessTracker.Core.Services.Interfaces;

namespace FitnessTracker.Services.Implementations
{
	public class ConfigurationService : IConfigurationService
	{
		// Virtual so test classes can override it.
		public virtual string DataFileName => "FitnessData.ft";

		public string DatabaseConnectionString => $"Data Source={DataFileName}";

		public string SettingsFileName => "settings.json";
	}
}
