using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.Core.Models
{
	public class DailyRecord
	{
		[Key]
		public int Id
		{
			get; set;
		}

		public DateTime Date
		{
			get; set;
		}

		public double Weight
		{
			get; set;
		}

		public double? MovingWeightAverage
		{
			get; set;
		}
	}
}
