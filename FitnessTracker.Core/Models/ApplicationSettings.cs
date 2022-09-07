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
	}
}
