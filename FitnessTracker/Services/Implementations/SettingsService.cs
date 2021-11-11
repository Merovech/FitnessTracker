using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;

namespace FitnessTracker.Services.Implementations
{
	public class SettingsService : ISettingsService
	{
		private const string FILENAME = "settings.json";

		private JsonSerializerOptions _serializationOptions;

		public SettingsService()
		{
			_serializationOptions = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				WriteIndented = true,
				Converters = { new JsonStringEnumConverter() }
			};
		}

		public SystemSettings ReadSettings()
		{
			SystemSettings returnSettings;
			if (!File.Exists(FILENAME))
			{
				// No settings file, so create one with some basic defaults
				returnSettings = CreateDefaultSettings();
			}
			else
			{
				var fileContents = File.ReadAllText(FILENAME);
				returnSettings = JsonSerializer.Deserialize<SystemSettings>(fileContents, _serializationOptions);
			}

			Debug.WriteLine(returnSettings.ToDebugString());
			return returnSettings;
		}

		public void SaveSettings(SystemSettings settings)
		{
			var json = JsonSerializer.Serialize(settings, _serializationOptions);
			File.WriteAllText(FILENAME, json);
		}

		private SystemSettings CreateDefaultSettings()
		{
			var settings = new SystemSettings();
			SaveSettings(settings);

			return settings;
		}
	}
}
