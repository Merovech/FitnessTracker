using System.Text.Json.Serialization;

namespace FitnessTracker.Models
{
	public class SystemSettings
	{
		public SystemSettings()
		{
			WeightUnit = WeightUnit.Pounds;
		}

		public WeightUnit WeightUnit { get; set; }

		public double? WeightGraphMinimum { get; set; }

		public double? WeightGraphMaximum { get; set; }

		public bool IsDarkTheme { get; set; }

		public string ToDebugString()
		{
			var weightMin = WeightGraphMinimum.HasValue ? WeightGraphMinimum.Value.ToString() : "(null)";
			var weightMax = WeightGraphMaximum.HasValue ? WeightGraphMaximum.Value.ToString() : "(null)";
			return $"DarkTheme={IsDarkTheme}, WeightUnit={WeightUnit}, WeightGraphMinimum={weightMin}, WeightGraphMaximum={weightMax}";
		}
	}
}
