using System.Text.Json.Serialization;

namespace FitnessTracker.Core.Models
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

		public override string ToString()
		{
			return $"DarkTheme={IsDarkTheme}, WeightUnit={WeightUnit}, WeightGraphMinimum={WeightGraphMinimum}, WeightGraphMaximum={WeightGraphMaximum}";
		}
	}
}
