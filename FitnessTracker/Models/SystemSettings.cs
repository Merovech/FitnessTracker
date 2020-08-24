using System.Text.Json.Serialization;

namespace FitnessTracker.Models
{
	public class SystemSettings
	{
		public SystemSettings()
		{
			WeightUnit = WeightUnit.Pounds;
			DistanceUnit = DistanceUnit.Miles;
			DistanceGraphMinimum = 0;
		}

		public WeightUnit WeightUnit { get; set; }

		public DistanceUnit DistanceUnit { get; set; }

		public double? WeightGraphMinimum { get; set; }

		public double? WeightGraphMaximum { get; set; }

		public double? DistanceGraphMinimum { get; set; }

		public double? DistanceGraphMaximum { get; set; }

		public string ToDebugString()
		{
			var weightMin = WeightGraphMinimum.HasValue ? WeightGraphMinimum.Value.ToString() : "(null)";
			var weightMax = WeightGraphMaximum.HasValue ? WeightGraphMaximum.Value.ToString() : "(null)";
			var distMin = DistanceGraphMinimum.HasValue ? DistanceGraphMinimum.Value.ToString() : "(null)";
			var distMax = DistanceGraphMaximum.HasValue ? DistanceGraphMaximum.Value.ToString() : "(null)";
			return $"WeightUnit={WeightUnit}, DistanceUnit={DistanceUnit}, WeightGraphMinimum={weightMin}, WeightGraphMaximum={weightMax}, DistanceGraphMinimum={distMin}, DistanceGraphMaximum={distMax}";
		}
	}
}
