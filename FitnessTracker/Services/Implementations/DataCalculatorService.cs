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

		public SummaryStatistics CalculateSummaryStatistics(IEnumerable<DailyRecord> data)
		{
			var retVal = new SummaryStatistics();
			if (data == null || !data.Any())
			{
				return retVal;
			}

			var dataList = data.ToList();
			var startingWeight = dataList.FirstOrDefault().Weight;
			var maxDistanceRecord = new DailyRecord { Id = 0, Weight = 0, DistanceMoved = double.MinValue };
			var lowestWeightRecord = new DailyRecord { Id = 0, Weight = 0, MovingWeightAverage = double.MaxValue };
			var highestWeightRecord = new DailyRecord { Id = 0, Weight = 0, MovingWeightAverage = double.MinValue };

			for (int i = 0; i < dataList.Count; i++)
			{
				var item = dataList[i];
				var distanceMoved = item.DistanceMoved.GetValueOrDefault();
				if (distanceMoved > maxDistanceRecord.DistanceMoved.GetValueOrDefault())
				{
					maxDistanceRecord = item;
				}

				if (item.MovingWeightAverage < lowestWeightRecord.MovingWeightAverage)
				{
					lowestWeightRecord = item;
				}

				if (item.MovingWeightAverage > highestWeightRecord.MovingWeightAverage)
				{
					highestWeightRecord = item;
				}

				// Since Average Distance Moved is cumulative (the value being the average of all previous values), the latest one that isn't null
				// is the target summary value.  The easiest way to do this is just to keep track of the latest one that isn't null as we iterate.
				if (item.AverageDistanceMoved != null)
				{
					retVal.AverageDistanceMoved = item.AverageDistanceMoved.GetValueOrDefault();
				}

				// Current weight is the latest Moving Average value.
				if (i == dataList.Count - 1)
				{
					retVal.CurrentWeight = item.MovingWeightAverage.GetValueOrDefault();
					if (dataList.Count > 1)
					{
						retVal.WeightChangeSincePrevious = item.MovingWeightAverage.GetValueOrDefault() - dataList[i - 1].MovingWeightAverage.GetValueOrDefault();
					}
				}

				retVal.TotalDistanceMoved += distanceMoved;
			}

			retVal.TotalWeightChange = retVal.CurrentWeight - startingWeight;
			retVal.LowestWeight = lowestWeightRecord.MovingWeightAverage.GetValueOrDefault();
			retVal.LowestWeightDate = lowestWeightRecord.Date;
			retVal.HighestWeight = highestWeightRecord.MovingWeightAverage.GetValueOrDefault();
			retVal.HighestWeightDate = highestWeightRecord.Date;
			retVal.LargestDistanceMoved = maxDistanceRecord.DistanceMoved.GetValueOrDefault();
			retVal.LargestDistanceMovedDate = maxDistanceRecord.Date;

			CleanupCalculatedValues(retVal);

			return retVal;
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
					data[i].AverageDistanceMoved = Math.Round(totalDistanceMoved / (i + 1 - nullDistanceDays), 2);
				}
			}
		}

		private void CleanupCalculatedValues(SummaryStatistics statistics)
		{
			// Since we're dealing with doubles, the math can give weird answers like "average = 1.000000000000009".
			// So we'll round all weight values to the nearest tenth and distances to the nearest hundredth for calculated fields.
			// For the purposes of this method, "calculated values" are:
			// TotalWeightChange
			// TotalDistanceMoved
			// WeightChangeSincePrevious

			statistics.TotalWeightChange = Math.Round(statistics.TotalWeightChange, 1);
			statistics.WeightChangeSincePrevious = Math.Round(statistics.WeightChangeSincePrevious, 1);
			statistics.TotalDistanceMoved = Math.Round(statistics.TotalDistanceMoved, 2);
		}
	}
}
