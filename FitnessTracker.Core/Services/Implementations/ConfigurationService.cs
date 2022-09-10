using FitnessTracker.Core;
using FitnessTracker.Core.Models;
using FitnessTracker.Core.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace FitnessTracker.Services.Implementations
{
	[DependencyInjectionType(DependencyInjectionType.Service)]
	public class ConfigurationService : IConfigurationService
	{
		private readonly ApplicationSettings _settings;

		// Exists for mocks
		public ConfigurationService()
		{
		}

		public ConfigurationService(IOptions<ApplicationSettings> options)
		{
			_settings = options.Value;
		}

		// Virtual so test classes can override it.
		public virtual string DatabaseConnectionString => $"Data Source={_settings.DataFileName}";

		public string SettingsFileName => "settings.json";
	}
}
