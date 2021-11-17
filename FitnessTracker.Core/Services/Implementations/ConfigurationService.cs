using FitnessTracker.Core.Services.Interfaces;

namespace FitnessTracker.Services.Implementations
{
	public class ConfigurationService : IConfigurationService
	{
		public string DatabaseConnectionString => "Data Source=data.dat";
	}
}
