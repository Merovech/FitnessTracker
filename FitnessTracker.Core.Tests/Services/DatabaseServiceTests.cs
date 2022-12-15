using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FitnessTracker.Core.Models;
using FitnessTracker.Core.Services.Interfaces;
using FitnessTracker.Core.Tests.Helpers;
using FitnessTracker.Core.Tests.Helpers.Builders;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FitnessTracker.Core.Tests.Services
{
	// Note that we're not going to test that CreateDatabase() works, since it's used in InitializeTest() for everything
	// except for the constructor tests.
	[TestClass]
	public class DatabaseServiceTests : TestBase<IDatabaseService, DatabaseServiceBuilder>
	{
		[TestInitialize]
		public virtual void InitializeTest()
		{
			Builder = new DatabaseServiceBuilder();
			Target = Builder.Build();

			// Recreate a fresh DB each time.
			Task.Run(() => Target.CreateDatabase()).Wait();
		}

		[TestCleanup]
		public void CleanupTest()
		{
			// Delete the test database so it can be recreated fresh.
			if (File.Exists(Constants.TEST_DATABASE_FILENAME))
			{
				// Ensures all cached Sqlite connections (hung on to as an optimization)
				// are removed and that the file can be safely deleted.  See here for more
				// details: https://stackoverflow.com/a/24570408/112829
				SqliteConnection.ClearAllPools();

				File.Delete(Constants.TEST_DATABASE_FILENAME);
			}
		}

		[TestClass]
		public class ConstructorTests : DatabaseServiceTests
		{
			[TestInitialize]
			public override void InitializeTest()
			{
				// Only need a builder here; these tests don't need a database
				Builder = new DatabaseServiceBuilder();
			}

			[TestMethod]
			public void Should_Initialize_Correctly_With_Valid_Arguments()
			{
				var service = Builder.Build();
				Assert.IsNotNull(service);
			}

			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public void Should_Throw_ArgumentNullException_With_Null_Calculator_Service()
			{
				Builder.DataCalculatorService = null;
				_ = Builder.Build();
			}

			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public void Should_Throw_ArgumentNullException_With_Null_Configuration_Service()
			{
				Builder.ConfigurationService = null;
				_ = Builder.Build();
			}

			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public void Should_Throw_ArgumentNullException_With_Null_Logger()
			{
				Builder.Logger = null;
				_ = Builder.Build();
			}

		}

		[TestClass]
		public class GetAllRecordsTests : DatabaseServiceTests
		{
			[TestMethod]
			public async Task Should_Return_All_Records()
			{
				// Pretty redundant with the verification portion of the UpsertRecordsTests, but we still
				// should have a unit test here in case we ever need to change stuff.
				var records = TestDataGenerator.GenerateRandomRecords(5);
				await Target.UpsertRecords(records);
				var result = await Target.GetAllRecords();

				Assert.AreEqual(records.Count, result.Count(), "Retrieved incorrect number of records.");
				Builder.VerifyRecordListsAreEqual(records, result.ToList());
			}

			[TestMethod]
			public async Task Should_Return_Empty_List_For_No_Records()
			{
				var result = await Target.GetAllRecords();

				Assert.IsNotNull(result, "Service returned null when there were no records in the database.");
				Assert.AreEqual(0, result.Count(), "Service returned records when there should have been none.");
			}

			[TestMethod]
			public async Task Should_Call_DataCalculatorService_If_Records_Are_Found()
			{
				var records = TestDataGenerator.GenerateRandomRecords(5);
				await Target.UpsertRecords(records);
				_ = await Target.GetAllRecords();
				Builder.VerifyDataCalculatorServiceWasCalled(1);
			}

			[TestMethod]
			public async Task Should_Not_Call_DataCalculatorService_If_No_Records_Are_Found()
			{
				_ = await Target.GetAllRecords();
				Builder.VerifyDataCalculatorServiceWasCalled(0);
			}
		}

		[TestClass]
		public class GetRecordByDateTests : DatabaseServiceTests
		{
			[TestMethod]
			public async Task Should_Successfully_Get_Record()
			{
				var records = TestDataGenerator.GenerateRandomRecords(10);
				await Target.UpsertRecords(records);

				var foundRecord = await Target.GetRecordByDate(records[2].Date);
				Assert.IsNotNull(foundRecord);
				Assert.AreEqual(records[2].Date, foundRecord.Date, $"Found incorrect record for date '{records[2].Date}");
				Assert.AreEqual(records[2].Weight, foundRecord.Weight, $"Found incorrect record for date '{records[2].Date}");
			}

			[TestMethod]
			public async Task Should_Return_Null_For_Nonexistent_Record()
			{
				var records = TestDataGenerator.GenerateRandomRecords(10);
				await Target.UpsertRecords(records);

				var foundRecord = await Target.GetRecordByDate(records[0].Date.AddDays(-1));
				Assert.IsNull(foundRecord, "Found record that should not exist.");
			}

			[TestMethod]
			public async Task Should_Return_Null_When_Searching_On_Empty_Record_Table()
			{
				var foundRecord = await Target.GetRecordByDate(DateTime.Today);
				Assert.IsNull(foundRecord, "Found record that should not exist.");
			}
		}

		[TestClass]
		public class UpsertRecordTests : DatabaseServiceTests
		{
			[TestMethod]
			public async Task Should_Successfully_Insert_New_Record()
			{
				var records = TestDataGenerator.GenerateRandomRecords(1).First();
				await Target.UpsertRecord(records.Date, records.Weight);
				var insertedRecords = (await Target.GetAllRecords()).ToList();
				Assert.AreEqual(1, insertedRecords.Count);

				var insertedRecord = insertedRecords.First();
				Assert.AreEqual(records.Date, insertedRecord.Date, "Record was inserted with incorrect date.");
				Assert.AreEqual(records.Weight, insertedRecord.Weight, "Record was inserted with incorrect weight.");
			}
		}

		[TestClass]
		public class UpsertRecordsTests : DatabaseServiceTests
		{
			[TestMethod]
			public async Task Should_Successfully_Insert_New_Records()
			{
				var records = TestDataGenerator.GenerateRandomRecords(10);
				await Target.UpsertRecords(records);
				var insertedRecords = (await Target.GetAllRecords()).ToList();
				Builder.VerifyRecordListsAreEqual(records, insertedRecords);
			}

			[TestMethod]
			public async Task Should_Successfully_Overwrite_Existing_Records()
			{
				var records = TestDataGenerator.GenerateRandomRecords(10);
				await Target.UpsertRecords(records);

				records[0].Weight += 1;
				records[2].Weight += 1;
				records[4].Weight += 1;
				records.Add(new DailyRecord { Date = DateTime.Today, Weight = 10 });

				await Target.UpsertRecords(records);
				var insertedRecords = (await Target.GetAllRecords()).ToList();
				Builder.VerifyRecordListsAreEqual(records, insertedRecords);
			}

			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public async Task Should_Throw_On_Null_Records()
			{
				await Target.UpsertRecords(null);
			}

			[TestMethod]
			[ExpectedException(typeof(InvalidOperationException))]
			public async Task Should_Throw_On_Empty_Records()
			{
				await Target.UpsertRecords(new List<DailyRecord>());
			}

		}
	}
}