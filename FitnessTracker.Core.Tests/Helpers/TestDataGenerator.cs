using System;
using System.Collections.Generic;
using FitnessTracker.Core.Models;

namespace FitnessTracker.Core.Tests.Helpers
{
	internal static class TestDataGenerator
	{
		public static List<DailyRecord> GenerateRandomRecords(int count)
		{
			var startDate = new DateTime(2020, 1, 1);
			var returnList = new List<DailyRecord>();
			var rng = new Random();

			for (int i = 0; i < count; i++)
			{
				returnList.Add(new DailyRecord { Date = startDate.AddDays(i), Weight = rng.NextDouble() * 200 });
			}

			return returnList;
		}
	}
}
