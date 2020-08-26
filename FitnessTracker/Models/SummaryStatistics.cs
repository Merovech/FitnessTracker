using System;
using System.Collections.Generic;
using System.Text;

namespace FitnessTracker.Models
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

		public double? TotalDistanceMoved { get; set;  }

		public double? LargestDistanceMoved { get; set; }

		public DateTime? LargestDistanceMovedDate { get; set; }

		public double? AverageDistanceMoved { get; set; }
	}
}
