using System.Configuration;
using FitnessTracker.Core;
using FitnessTracker.UI.Services.Interfaces;

namespace FitnessTracker.Services.Implementations
{
	[DependencyInjectionType(DependencyInjectionType.Service)]
	public class ConfigurationService : IConfigurationService
	{
		private const string CONNECTIONSTRING_KEY = "DataFileConnectionString";

		public string DatabaseConnectionString => ConfigurationManager.ConnectionStrings[CONNECTIONSTRING_KEY].ConnectionString;
	}
}
