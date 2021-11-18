using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FitnessTracker.Core.Models;
using FitnessTracker.Core.Services.Implementations;
using FitnessTracker.Core.Services.Interfaces;
using FitnessTracker.Core.Tests.Helpers;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FitnessTracker.Core.Tests.Services
{
	[TestClass]
	public class DatabaseServiceTests
	{
		protected DatabaseServiceTestsBuilder Builder
		{
			get; 
			set;
		}

		[TestInitialize]
		public virtual void InitializeTest()
		{
			Builder = new DatabaseServiceTestsBuilder();
		}

		[TestClass]
		public  class ConstructorTests : DatabaseServiceTests
		{
			[TestMethod]
			public void Should_Initialize_Correctly_With_Valid_Arguments()
			{
				try
				{
					var service = Builder.Build();
					Assert.IsNotNull(service);
				}
				catch (Exception)
				{
					Assert.Fail("Service failed to initialize with valid arguments.");
				}
			}

			[TestMethod]
			public void Should_Throw_ArgumentNullException_With_Null_Calculator_Service()
			{
				try
				{
					Builder.DataCalculatorService = null;
					_ = Builder.Build();
					Assert.Fail("Calculator service was null, but service constructor did not fail.");
				}
				catch (ArgumentNullException)
				{
					// Do nothing, test passed.
				}
			}

			[TestMethod]
			public void Should_Throw_ArgumentNullException_With_Null_Configuration_Service()
			{
				try
				{
					Builder.ConfigurationService = null;
					_ = Builder.Build();
					Assert.Fail("Configuration service was null, but service constructor did not fail.");
				}
				catch (ArgumentNullException)
				{
					// Do nothing, test passed.
				}
			}
		}

		[TestClass]
		public class UpsertRecordsTests : DatabaseServiceTests
		{
			private IDatabaseService _service;

			[TestInitialize]
			public override void InitializeTest()
			{
				base.InitializeTest();
				_service = Builder.Build();

				// Recreate a fresh DB each time.
				Task.Run(() => _service.CreateDatabase()).Wait();
			}

			[TestCleanup]
			public virtual void TestCleanup()
			{
				// Delete the test database so it can be recreated fresh.
				if (File.Exists("test_data.dat"))
				{
					// Ensures all cached Sqlite connections (hung on to as an optimization)
					// are removed and that the file can be safely deleted.  See here for more
					// details: https://stackoverflow.com/a/24570408/112829
					SqliteConnection.ClearAllPools();

					File.Delete("test_data.dat");
				}
			}

			[TestMethod]
			public async Task Should_Successfully_Insert_New_Records()
			{
				var records = Builder.GenerateRandomRecords(10);
				await _service.UpsertRecords(records);
				var insertedRecords = (await _service.GetAllRecords()).ToList();
				CompareRetrievedRecords(records, insertedRecords);
			}

			[TestMethod]
			public async Task Should_Successfully_Overwrite_Existing_Records()
			{
				var records = Builder.GenerateRandomRecords(10);
				await _service.UpsertRecords(records);

				records[0].Weight += 1;
				records[2].Weight += 1;
				records[4].Weight += 1;
				records.Add(new DailyRecord { Date = DateTime.Today, Weight = 10 });

				await _service.UpsertRecords(records);
				var insertedRecords = (await _service.GetAllRecords()).ToList();
				CompareRetrievedRecords(records, insertedRecords);
			}

			private void CompareRetrievedRecords(List<DailyRecord> expected, List<DailyRecord> actual)
			{
				Assert.IsTrue(expected.SequenceEqual(actual, new DailyRecordEqualityComparer()), "Actual and expected lists were not equal.");
			}
		}
		
		protected class DatabaseServiceTestsBuilder
		{
			public IConfigurationService ConfigurationService
			{
				get;
				set;
			}

			public IDataCalculatorService DataCalculatorService
			{
				get; 
				set; 
			}

			public DatabaseServiceTestsBuilder()
			{
				SetupMocks();
			}

			public IDatabaseService Build()
			{
				return new DatabaseService(DataCalculatorService, ConfigurationService);
			}

			public List<DailyRecord> GenerateRandomRecords(int count)
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

			private void SetupMocks()
			{
				var configMock = new Mock<IConfigurationService>();
				configMock.Setup(svc => svc.DatabaseConnectionString).Returns("Data Source=test_data.dat");
				ConfigurationService = configMock.Object;

				var calcMock = new Mock<IDataCalculatorService>();
				// TODO: Mock the actual methods
				DataCalculatorService = calcMock.Object;
			}
		}
	}
}
