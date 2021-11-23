using System;
using System.Collections.Generic;
using System.Linq;
using FitnessTracker.Core.Models;
using FitnessTracker.Core.Services.Implementations;
using FitnessTracker.Core.Services.Interfaces;
using FitnessTracker.Core.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FitnessTracker.Core.Tests.Services
{
	[TestClass]
	public class DataCalculatorServiceTests
	{
		protected DataCalculatorServiceBuilder Builder
		{
			get;
			set;
		}

		protected IDataCalculatorService Service
		{
			get;
			set;
		}

		[TestInitialize]
		public void InitializeTest()
		{
			Builder = new DataCalculatorServiceBuilder();
			Service = new DataCalculatorService();
		}

		[TestClass]
		public class FillCalculatedDataFieldsTests : DataCalculatorServiceTests
		{
			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public void Should_Fail_On_Null_List()
			{
				Service.FillCalculatedDataFields(null);
			}

			[TestMethod]
			[ExpectedException(typeof(InvalidOperationException))]
			public void Should_Fail_On_Empty_List()
			{
				Service.FillCalculatedDataFields(new List<DailyRecord>());
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

				Service.FillCalculatedDataFields(records);
				Assert.IsTrue(records.Select(r => r.MovingWeightAverage).ToList().SequenceEqual(expectedMovingAverageWeights), "Moving average values were incorrect.");
			}

		}

		[TestClass]
		public class CalculateCummsaryStatisticsTests : DataCalculatorServiceTests
		{
			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public void Should_Fail_On_Null_List()
			{
				_ = Service.CalculateSummaryStatistics(null);
			}

			[TestMethod]
			[ExpectedException(typeof(InvalidOperationException))]
			public void Should_Fail_On_Empty_List()
			{
				_ = Service.CalculateSummaryStatistics(new List<DailyRecord>());
			}

			[TestMethod]
			public void Current_Weight_Should_Be_Last_Record_When_No_Moving_Average_Exists()
			{
				var records = TestDataGenerator.GenerateRandomRecords(4);
				var result = Service.CalculateSummaryStatistics(records);
				Assert.AreEqual(records.Last().Weight, result.CurrentWeight, "Incorrect current weight with no moving average.");
			}

			[TestMethod]
			public void Change_Since_Previous_Should_Be_Null_With_No_Moving_Average()
			{
				var records = TestDataGenerator.GenerateRandomRecords(4);
				var result = Service.CalculateSummaryStatistics(records);
				Assert.IsNull(result.WeightChangeSincePrevious, "Weight change since previous was not null when there was no moving average at all.");
			}

			[TestMethod]
			public void Change_Since_Previous_Should_Be_Null_With_One_Moving_Average()
			{
				var records = TestDataGenerator.GenerateRandomRecords(5);
				Service.FillCalculatedDataFields(records);
				var result = Service.CalculateSummaryStatistics(records);
				Assert.IsNotNull(records.Last().MovingWeightAverage, "Moving weight average was not filled on the fifth record.");
				Assert.IsNull(result.WeightChangeSincePrevious, "Weight change since previous was not null when there was only one moving average value available.");
			}
		}

		protected class DataCalculatorServiceBuilder
		{
			public IDataCalculatorService Build()
			{
				return new DataCalculatorService();
			}
		}
	}
}
