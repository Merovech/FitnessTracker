using System;
using System.Collections.Generic;
using System.Linq;
using FitnessTracker.Core.Models;
using FitnessTracker.Core.Services.Interfaces;
using FitnessTracker.Core.Tests.Helpers;
using FitnessTracker.Core.Tests.Helpers.Builders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FitnessTracker.Core.Tests.Services
{
	[TestClass]
	public class DataCalculatorServiceTests : TestBase<IDataCalculatorService, DataCalculatorServiceBuilder>
	{
		[TestInitialize]
		public void InitializeTest()
		{
			Builder = new DataCalculatorServiceBuilder();
			Target = Builder.Build();
		}

		[TestClass]
		public class FillCalculatedDataFieldsTests : DataCalculatorServiceTests
		{
			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public void Should_Fail_On_Null_List()
			{
				Target.FillCalculatedDataFields(null);
			}

			[TestMethod]
			[ExpectedException(typeof(InvalidOperationException))]
			public void Should_Fail_On_Empty_List()
			{
				Target.FillCalculatedDataFields(new List<DailyRecord>());
			}

			[TestMethod]
			public void Should_Not_Fill_Moving_Average_For_Less_Than_Five_Days()
			{
				var records = TestDataGenerator.GenerateRandomRecords(4);
				foreach (var record in records)
				{
					Assert.IsNull(record.MovingWeightAverage, "Moving average was generated for less than 5 records.");
				}
			}

			[TestMethod]
			public void Should_Fill_Moving_Average_Starting_At_Day_Five()
			{
				var records = TestDataGenerator.GenerateRandomRecords(10);
				for (var i = 5; i < records.Count; i++)
				{
					Assert.IsNotNull(records[i], "Moving average was not generated for records starting at day 5.");
				}
			}

			[TestMethod]
			public void Should_Fill_Moving_Average_Correctly()
			{
				var records = new List<DailyRecord>()
				{
					{ new DailyRecord { Date=new DateTime(2020, 1, 1), Weight=100 } },
					{ new DailyRecord { Date=new DateTime(2020, 1, 2), Weight=110 } },
					{ new DailyRecord { Date=new DateTime(2020, 1, 3), Weight=120 } },
					{ new DailyRecord { Date=new DateTime(2020, 1, 4), Weight=130 } },
					{ new DailyRecord { Date=new DateTime(2020, 1, 5), Weight=140 } },
					{ new DailyRecord { Date=new DateTime(2020, 1, 6), Weight=150 } },
					{ new DailyRecord { Date=new DateTime(2020, 1, 7), Weight=160 } },
					{ new DailyRecord { Date=new DateTime(2020, 1, 8), Weight=170 } },
					{ new DailyRecord { Date=new DateTime(2020, 1, 9), Weight=180 } },
					{ new DailyRecord { Date=new DateTime(2020, 1, 10), Weight=190 } }
				};

				var expectedMovingAverageWeights = new List<double?>
				{
					null,
					null,
					null,
					null,
					120,
					130,
					140,
					150,
					160,
					170
				};

				Target.FillCalculatedDataFields(records);
				Assert.IsTrue(records.Select(r => r.MovingWeightAverage).ToList().SequenceEqual(expectedMovingAverageWeights), "Moving average values were incorrect.");
			}

		}

		[TestClass]
		public class CalculateCummsaryStatisticsTests : DataCalculatorServiceTests
		{
			[TestMethod]
			public void Should_Return_Null_On_Null_List()
			{
				var result = Target.CalculateSummaryStatistics(null);
				Assert.IsNull(result, "Null list yielded non-null summary statistics.");
			}

			[TestMethod]
			public void Should_Return_Null_On_Empty_List()
			{
				var result = Target.CalculateSummaryStatistics(new List<DailyRecord>());
				Assert.IsNull(result, "Null list yielded non-null summary statistics.");
			}

			[TestMethod]
			public void Current_Weight_Should_Be_Last_Record_When_No_Moving_Average_Exists()
			{
				var records = TestDataGenerator.GenerateRandomRecords(4);
				var result = Target.CalculateSummaryStatistics(records);
				Assert.AreEqual(records.Last().Weight, result.CurrentWeight, "Incorrect current weight with no moving average.");
			}

			[TestMethod]
			public void Change_Since_Previous_Should_Be_Null_With_No_Moving_Average()
			{
				var records = TestDataGenerator.GenerateRandomRecords(4);
				var result = Target.CalculateSummaryStatistics(records);
				Assert.IsNull(result.WeightChangeSincePrevious, "Weight change since previous was not null when there was no moving average at all.");
			}

			[TestMethod]
			public void Change_Since_Previous_Should_Be_Null_With_One_Moving_Average()
			{
				var records = TestDataGenerator.GenerateRandomRecords(5);
				Target.FillCalculatedDataFields(records);
				var result = Target.CalculateSummaryStatistics(records);
				Assert.IsNotNull(records.Last().MovingWeightAverage, "Moving weight average was not filled on the fifth record.");
				Assert.IsNull(result.WeightChangeSincePrevious, "Weight change since previous was not null when there was only one moving average value available.");
			}

			[TestMethod]
			public void Lowest_Weight_Should_Be_Null_With_No_Moving_Average()
			{
				var records = TestDataGenerator.GenerateRandomRecords(4);
				var result = Target.CalculateSummaryStatistics(records);
				Assert.IsNull(result.LowestWeight, "Lowest weight was not null when there was no moving average at all.");
				Assert.IsNull(result.LowestWeightDate, "Lowest weight date was not null when there was no moving average at all.");
			}

			[TestMethod]
			public void Highest_Weight_Should_Be_Null_With_No_Moving_Average()
			{
				var records = TestDataGenerator.GenerateRandomRecords(4);
				var result = Target.CalculateSummaryStatistics(records);
				Assert.IsNull(result.HighestWeight, "Highest weight was not null when there was no moving average at all.");
				Assert.IsNull(result.HighestWeightDate, "Highest weight date was not null when there was no moving average at all.");
			}

			[TestMethod]
			public void Should_Calculate_Lowest_Weight_Correctly()
			{
				var records = TestDataGenerator.GenerateRandomRecords(100);
				Target.FillCalculatedDataFields(records);
				var lowest = records.Where(r => r.MovingWeightAverage != null).Min(r => r.MovingWeightAverage);
				var lowestRecord = records.First(r => r.MovingWeightAverage == lowest);
				var result = Target.CalculateSummaryStatistics(records);

				Assert.AreEqual(lowestRecord.MovingWeightAverage, result.LowestWeight, "Lowest weight was calculated incorrectly.");
				Assert.AreEqual(lowestRecord.Date, result.LowestWeightDate, "Lowest weight was calculated incorrectly.");
			}

			[TestMethod]
			public void Should_Calculate_Highest_Weight_Correctly()
			{
				var records = TestDataGenerator.GenerateRandomRecords(100);
				Target.FillCalculatedDataFields(records);
				var highest = records.Max(r => r.MovingWeightAverage);
				var highestRecord = records.First(r => r.MovingWeightAverage == highest);
				var result = Target.CalculateSummaryStatistics(records);

				Assert.AreEqual(highestRecord.MovingWeightAverage, result.HighestWeight, "Highest weight was calculated incorrectly.");
				Assert.AreEqual(highestRecord.Date, result.HighestWeightDate, "Highest weight was calculated incorrectly.");
			}

			[TestMethod]
			public void Total_Weight_Change_Should_Be_Null_For_List_With_One_Record()
			{
				var records = TestDataGenerator.GenerateRandomRecords(1);
				var result = Target.CalculateSummaryStatistics(records);

				Assert.IsNull(result.TotalWeightChange, "Total weight change was calculated for a one-record list when it should not be.");
			}

			[TestMethod]
			public void Total_Weight_Change_Should_Be_Calculated_Correctly()
			{
				var records = TestDataGenerator.GenerateRandomRecords(100);
				var result = Target.CalculateSummaryStatistics(records);
				var expected = Math.Round(records.Last().Weight - records.First().Weight, 1);

				Assert.AreEqual(expected, result.TotalWeightChange, "Total weight change was calculated incorrectly.");
			}
		}
	}
}
