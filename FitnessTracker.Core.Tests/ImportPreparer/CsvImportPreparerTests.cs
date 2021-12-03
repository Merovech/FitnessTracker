using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using FitnessTracker.Core.ImportPreparer.Interfaces;
using FitnessTracker.Core.Tests.Helpers;
using FitnessTracker.Core.Tests.Helpers.Builders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FitnessTracker.Core.Tests.ImportPreparer
{
	public class CsvImportPreparerTests : TestBase<IImportPreparer, CsvImportPreparerBuilder>
	{
		[TestInitialize]
		public void InitializeTest()
		{
			Builder = new CsvImportPreparerBuilder();
			Target = Builder.Build();
			File.WriteAllText(Constants.IMPORT_CSV_FILENAME, string.Empty);
		}

		[TestCleanup]
		public void CleanupTest()
		{
			if (File.Exists(Constants.IMPORT_CSV_FILENAME))
			{
				File.Delete(Constants.IMPORT_CSV_FILENAME);
			}
		}

		[TestClass]
		public class GetRecordsTests : CsvImportPreparerTests
		{

			[TestMethod]
			public async Task Should_Return_Empty_List_On_Empty_File()
			{
				var result = await Target.GetRecords(Constants.IMPORT_CSV_FILENAME);
				Assert.AreEqual(0, result.Count(), "Non-empty list returned from empty file.");
			}

			[TestMethod]
			public async Task Should_Read_Records_Correctly()
			{
				var records = TestDataGenerator.GenerateRandomRecords(100);
				Builder.CreateValidData(records);
				var result = await Target.GetRecords(Constants.IMPORT_CSV_FILENAME);
				Assert.IsTrue(result.SequenceEqual(records, new DailyRecordEqualityComparer()), "Records were not read correctly.");
			}

			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public async Task Should_Fail_On_Null_Filename()
			{
				_ = await Target.GetRecords(null);
			}

			[TestMethod]
			[ExpectedException(typeof(InvalidOperationException))]
			public async Task Should_Fail_On_Empty_Filename()
			{
				_ = await Target.GetRecords(string.Empty);
			}

			[TestMethod]
			[ExpectedException(typeof(FileNotFoundException))]
			public async Task Should_Fail_On_Nonexistent_File()
			{
				_ = await Target.GetRecords("file-does-not-exist.csv");
			}

			// Switching to a CSV library should render these next two obsolete.
			[TestMethod]
			[ExpectedException(typeof(ReaderException))]
			public async Task Should_Fail_On_Invalid_Data()
			{
				Builder.CreateInvalidData();
				_ = await Target.GetRecords(Constants.IMPORT_CSV_FILENAME);
			}

			[TestMethod]
			[ExpectedException(typeof(ReaderException))]
			public async Task Should_Fail_On_Invalid_Data_Content()
			{
				Builder.CreateUnparseableData();
				_ = await Target.GetRecords(Constants.IMPORT_CSV_FILENAME);
			}
		}
	}
}
