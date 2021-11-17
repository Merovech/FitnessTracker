using System;

namespace FitnessTracker.Core.Models
{
	public class SummaryStatistics
	{
		public double? CurrentWeight { get; set; }

		public double? TotalWeightChange { get; set; }

		public double? LowestWeight { get; set; }

		public DateTime? LowestWeightDate { get; set; }

		public double? HighestWeight { get; set; }
		
		public DateTime? HighestWeightDate { get; set; }

		public double? WeightChangeSincePrevious { get; set; }
	}
}
