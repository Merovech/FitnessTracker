using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using FitnessTracker.Core.Models;
using FitnessTracker.Core.Services.Interfaces;
using FitnessTracker.Utilities;

namespace FitnessTracker.Core.Services.Implementations
{
	[DependencyInjectionType(DependencyInjectionType.Service)]
	public class SettingsService : ISettingsService
	{
		private readonly string _filename;

		private readonly JsonSerializerOptions _serializationOptions;

		public SettingsService(IConfigurationService configurationService)
		{
			Guard.AgainstNull(configurationService, nameof(configurationService));
			_filename = configurationService.SettingsFileName;

			_serializationOptions = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				WriteIndented = true,
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
				Converters = { new JsonStringEnumConverter() }
			};
		}

		public SystemSettings ReadSettings()
		{
			SystemSettings returnSettings;
			if (!File.Exists(_filename))
			{
				// No settings file, so create one with some basic defaults
				returnSettings = CreateDefaultSettings();
			}
			else
			{
				var fileContents = File.ReadAllText(_filename);
				returnSettings = JsonSerializer.Deserialize<SystemSettings>(fileContents, _serializationOptions);
			}

			Debug.WriteLine(returnSettings.ToDebugString());
			return returnSettings;
		}

		public void SaveSettings(SystemSettings settings)
		{
			Guard.AgainstNull(settings, nameof(settings));

			var json = JsonSerializer.Serialize(settings, _serializationOptions);
			File.WriteAllText(_filename, json);
		}

		private SystemSettings CreateDefaultSettings()
		{
			var settings = new SystemSettings();
			SaveSettings(settings);

			return settings;
		}
	}
}
