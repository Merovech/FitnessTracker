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
			var fileContents = File.ReadAllText(FILENAME);
			var model = JsonSerializer.Deserialize<SystemSettings>(fileContents, _serializationOptions);
			Debug.WriteLine(model.ToDebugString());

			return model;
		}

		public void SaveSettings(SystemSettings settings)
		{
			var json = JsonSerializer.Serialize(settings, _serializationOptions);
			File.WriteAllText(FILENAME, json);
		}
	}
}
