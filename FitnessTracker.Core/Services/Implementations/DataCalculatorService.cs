﻿using System;
using System.Collections.Generic;
using System.Linq;
using FitnessTracker.Core.Models;
using FitnessTracker.Core.Services.Interfaces;
using FitnessTracker.Utilities;
using Microsoft.Extensions.Logging;

namespace FitnessTracker.Core.Services.Implementations
{
	[DependencyInjectionType(DependencyInjectionType.Service)]
	public class DataCalculatorService : IDataCalculatorService
	{
		private readonly ILogger<DataCalculatorService> _logger;
		private const int _averageWindowInDays = 5;

		public DataCalculatorService(ILogger<DataCalculatorService> logger)
		{
			Guard.AgainstNull(logger, nameof(logger));
			_logger = logger;
		}

		public void FillCalculatedDataFields(IEnumerable<DailyRecord> data)
		{
			Guard.AgainstNull(data, nameof(data));
			Guard.AgainstEmptyList(data, nameof(data));

			var dataList = data.ToList();
			FillMovingWeightAverage(dataList);
		}

		public SummaryStatistics CalculateSummaryStatistics(IEnumerable<DailyRecord> data)
		{
			_logger.LogDebug($"Calculating summary statistics for {data?.Count()} records.");
			if (data == null || !data.Any())
			{
				return null;
			}

			var retVal = new SummaryStatistics();
			var dataList = data.ToList();
			var startingWeight = dataList.FirstOrDefault().Weight;
			var lowestWeightRecord = new DailyRecord { Id = 0, Weight = 0, MovingWeightAverage = double.MaxValue };
			var highestWeightRecord = new DailyRecord { Id = 0, Weight = 0, MovingWeightAverage = double.MinValue };

			for (int i = 0; i < dataList.Count; i++)
			{
				var item = dataList[i];

				if (item.MovingWeightAverage < lowestWeightRecord.MovingWeightAverage)
				{
					lowestWeightRecord = item;
				}

				if (item.MovingWeightAverage > highestWeightRecord.MovingWeightAverage)
				{
					highestWeightRecord = item;
				}

				// Current weight is the latest Moving Average value.  If there is no moving average yet, get the last current weight.
				if (i == dataList.Count - 1)
				{
					retVal.CurrentWeight = item.MovingWeightAverage ?? item.Weight;
					if (dataList.Count > 1)
					{
						// In the case where there is no moving average yet (<5 days of data), or this is the first moving average, don't fill this in.
						// Note that once data is entered, users cannot skip a record (i.e., have a record with a null Weight), so the average is calculated
						// for every subsequent data point once it hits 5 of them.
						if (dataList[i - 1].MovingWeightAverage.HasValue)
						{
							retVal.WeightChangeSincePrevious = item.MovingWeightAverage.GetValueOrDefault() - dataList[i - 1].MovingWeightAverage.GetValueOrDefault();
						}
					}
				}
			}

			retVal.TotalWeightChange = dataList.Count == 1 ? null : retVal.CurrentWeight - startingWeight;
			if (lowestWeightRecord.MovingWeightAverage.HasValue && lowestWeightRecord.MovingWeightAverage.Value < double.MaxValue)
			{
				retVal.LowestWeight = lowestWeightRecord.MovingWeightAverage.GetValueOrDefault();
				retVal.LowestWeightDate = lowestWeightRecord.Date;
			}

			if (highestWeightRecord.MovingWeightAverage.HasValue && highestWeightRecord.MovingWeightAverage.Value > double.MinValue)
			{
				retVal.HighestWeight = highestWeightRecord.MovingWeightAverage.GetValueOrDefault();
				retVal.HighestWeightDate = highestWeightRecord.Date;
			}

			CleanupCalculatedValues(retVal);

			_logger.LogDebug("Summary calculation complete.  Result: {result}", retVal);
			return retVal;
		}

		private void FillMovingWeightAverage(List<DailyRecord> data)
		{
			for (int i = data.Count - 1; i >= 0; i--)
			{
				if (i >= _averageWindowInDays - 1)
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

		private void CleanupCalculatedValues(SummaryStatistics statistics)
		{
			// Since we're dealing with doubles, the math can give weird answers like "average = 1.000000000000009".
			// So we'll round all weight values to the nearest tenth.
			// For the purposes of this method, "calculated values" are:
			// TotalWeightChange
			// WeightChangeSincePrevious

			if (statistics.TotalWeightChange.HasValue)
			{
				statistics.TotalWeightChange = Math.Round(statistics.TotalWeightChange.Value, 1);
			}

			if (statistics.WeightChangeSincePrevious.HasValue)
			{
				statistics.WeightChangeSincePrevious = Math.Round(statistics.WeightChangeSincePrevious.Value, 1);
			}
		}
	}
}
