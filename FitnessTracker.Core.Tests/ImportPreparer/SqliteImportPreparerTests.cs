﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FitnessTracker.Core.ImportPreparer.Interfaces;
using FitnessTracker.Core.Services.Implementations;
using FitnessTracker.Core.Services.Interfaces;
using FitnessTracker.Core.Tests.Helpers;
using FitnessTracker.Core.Tests.Helpers.Builders;
using FitnessTracker.Services.Implementations;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FitnessTracker.Core.Tests.ImportPreparer
{
	[TestClass]
	public class SqliteImportPreparerTests : TestBase<IImportPreparer, SqliteImportPreparerBuilder>
	{
		private DataCalculatorServiceBuilder _dataCalculatorServiceBuilder = new();

		protected IDatabaseService DatabaseService
		{
			get;
		}

		public SqliteImportPreparerTests()
		{
			var configMock = new Mock<ConfigurationService>() { CallBase = true };
			configMock.Setup(c => c.DatabaseConnectionString).Returns($"Data Source={Constants.IMPORT_DATABASE_FILENAME}");

			DatabaseService = new DatabaseService(_dataCalculatorServiceBuilder.Build(), configMock.Object, new Mock<ILogger<DatabaseService>>().Object);
		}

		[TestInitialize]
		public virtual void InitializeTest()
		{
			Builder = new SqliteImportPreparerBuilder();
			Target = Builder.Build();

			// Recreate a fresh DB each time.
			Task.Run(() => DatabaseService.CreateDatabase()).Wait();
		}

		[TestCleanup]
		public void CleanupTest()
		{
			// Delete the test database so it can be recreated fresh.
			if (File.Exists(Constants.IMPORT_DATABASE_FILENAME))
			{
				// Ensures all cached Sqlite connections (hung on to as an optimization)
				// are removed and that the file can be safely deleted.  See here for more
				// details: https://stackoverflow.com/a/24570408/112829
				SqliteConnection.ClearAllPools();

				File.Delete(Constants.IMPORT_DATABASE_FILENAME);
			}
		}

		[TestClass]
		public class GetRecordsTests : SqliteImportPreparerTests
		{
			[TestMethod]
			public async Task Should_Prepare_Records_Successfully()
			{
				var records = TestDataGenerator.GenerateRandomRecords(100);
				await DatabaseService.UpsertRecords(records);
				var results = (await Target.GetRecords(Constants.IMPORT_DATABASE_FILENAME)).ToList();

				Assert.IsTrue(results.SequenceEqual(records, new DailyRecordEqualityComparer()), "Records were not prepared accurately.");
			}

			[TestMethod]
			public async Task Should_Return_Empty_List_With_No_Rows_Found()
			{
				var results = await Target.GetRecords(Constants.IMPORT_DATABASE_FILENAME);
				Assert.AreEqual(0, results.Count(), "Non-empty list returned when no records were found.");
			}

			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public async Task Should_Fail_With_Null_FileName()
			{
				_ = await Target.GetRecords(null);
			}

			[TestMethod]
			[ExpectedException(typeof(InvalidOperationException))]
			public async Task Should_Fail_With_Empty_FileName()
			{
				_ = await Target.GetRecords(string.Empty);
			}

			[TestMethod]
			[ExpectedException(typeof(FileNotFoundException))]
			public async Task Should_Fail_With_Nonexistent_File()
			{
				_ = await Target.GetRecords("file-does-not-exist.dat");
			}

			[TestMethod]
			[ExpectedException(typeof(InvalidOperationException))]
			public async Task Should_Fail_With_No_Records_Table()
			{
				await Builder.CreateInvalidDatabaseTables();
				_ = await Target.GetRecords(Constants.IMPORT_DATABASE_FILENAME);
			}

			[TestMethod]
			[ExpectedException(typeof(InvalidOperationException))]
			public async Task Should_Fail_With_Invalid_Structure()
			{
				await Builder.CreateInvalidDatabaseColumns();
				_ = await Target.GetRecords(Constants.IMPORT_DATABASE_FILENAME);
			}

			[TestMethod]
			[ExpectedException(typeof(InvalidOperationException))]
			public async Task Should_Fail_With_Invalid_Data_Types()
			{
				await Builder.CreateInvalidDatabaseDataTypesWithData();
				_ = await Target.GetRecords(Constants.IMPORT_DATABASE_FILENAME);
			}
		}
	}
}
