using System;
using System.Collections.Generic;
using System.Linq;
using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;

namespace FitnessTracker.Services.Implementations
{
	public class DataCalculatorService : IDataCalculatorService
	{
		private int _averageWindowInDays = 5;
		public void FillCalculatedDataFields(IEnumerable<DailyRecord> data)
		{
			var dataList = data.ToList();
			FillMovingWeightAverage(dataList);
			FillAverageDistanceMoved(dataList);
		}

		private void FillMovingWeightAverage(List<DailyRecord> data)
		{
			for (int i = data.Count - 1; i >= 0; i--)
			{
				if (i >= _averageWindowInDays)
				{
					var sum = 0.0d;
					for (int j = 0; j < _averageWindowInDays; j++)
					{
						sum += data[i - j].Weight;
					}

					data[i].MovingWeightAverage = Math.Round(sum / _averageWindowInDays, 1);
				}
			}
		}

		private void FillAverageDistanceMoved(List<DailyRecord> data)
		{
			var totalDistanceMoved = 0.0d;
			var nullDistanceDays = 0;

			for (int i = 0; i < data.Count; i++)
			{
				if (data[i].DistanceMoved == null)
				{
					nullDistanceDays++;
				}
				else
				{
					totalDistanceMoved += data[i].DistanceMoved.Value;
					data[i].AverageDistanceMoved = totalDistanceMoved / (i + 1 - nullDistanceDays);
				}
			}
		}
	}
}
