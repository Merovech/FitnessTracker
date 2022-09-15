using System.Text.Json.Serialization;

namespace FitnessTracker.Core.Models
{
	public class ApplicationSettings
	{
		[JsonPropertyName("dataFileName")]
		public string DataFileName
		{
			get; set;
		}

		[JsonPropertyName("logLevel")]
		public NLog.LogLevel LogLevel
		{
			get; 
			set; 
		} = NLog.LogLevel.Debug;

		[JsonPropertyName("enableLogging")]
		public bool EnableLogging
		{
			get;
			set;
		}
	}
}
