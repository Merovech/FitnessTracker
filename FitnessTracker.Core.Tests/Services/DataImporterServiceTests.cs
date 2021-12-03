using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessTracker.Core.Services.Interfaces;
using FitnessTracker.Core.Tests.Helpers.Builders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FitnessTracker.Core.Tests.Services
{
	// This class doesn't need to test as much as others, because it basically exists to call
	// the combination of ImportPreparerFactory, ImportPreparer, and DatabaseService.  All three
	// of those are more comprehensively tested elsewhere.
	public class DataImporterServiceTests : TestBase<IDataImporterService, DataImporterServiceBuilder>
	{
		[TestInitialize]
		public virtual void InitializeTest()
		{
			Builder = new DataImporterServiceBuilder();
			Target = Builder.Build();
		}

		[TestClass]
		public class ConstructorTests : DataImporterServiceTests
		{
			[TestInitialize]
			public override void InitializeTest()
			{
				// Only need a builder here; these tests don't need a database
				Builder = new DataImporterServiceBuilder();
			}

			[TestMethod]
			public void Should_Initialize_Correctly()
			{
				_ = Builder.Build();
			}

			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public void Should_Fail_With_Null_Database_Service()
			{
				Builder.DatabaseService = null;
				_ = Builder.Build();
			}

			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public void Should_Fail_With_Null_Import_Preparer_Factory()
			{
				Builder.ImportPreparerFactory = null;
				_ = Builder.Build();
			}
		}

		[TestClass]
		public class ImportDataTests : DataImporterServiceTests
		{
			private const string FILE_NAME = "some-file.txt";

			[TestMethod]
			public async Task Should_Import_Successfully()
			{
				// Make sure we pass the File.Exists check.  The actual file doesn't matter since
				// it's mocked.
				File.WriteAllText(FILE_NAME, "Lorem ipsum dolor sit amet.");
				var result = await Target.ImportData(FILE_NAME);
				File.Delete(FILE_NAME);

				Assert.AreEqual(DataImporterServiceBuilder.ImportedRecordsCount, result, "Returned the wrong count of records.");
				Builder.VerifyGetImporterFactoryIsCalled(Times.Once());
				Builder.VerifyGetRecordsIsCalled(Times.Once());
				Builder.VerifyUpsertIsCalled(Times.Once());
			}

			[TestMethod]
			[ExpectedException(typeof(InvalidOperationException))]
			public async Task Should_Fail_On_Empty_FileName()
			{
				_ = await Target.ImportData(string.Empty);
			}

			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public async Task Should_Fail_On_Null_FileName()
			{
				_ = await Target.ImportData(null);
			}

			[TestMethod]
			[ExpectedException(typeof(FileNotFoundException))]
			public async Task Should_Fail_On_Non_Existent_File()
			{
				_ = await Target.ImportData(FILE_NAME);
			}

			[TestMethod]
			[ExpectedException(typeof(InvalidOperationException))]
			public async Task Should_Fail_On_No_Preparer_Found()
			{
				Target = Builder.BuildWithNoImportPreparer();

				try
				{
					File.WriteAllText(FILE_NAME, "Lorem ipsum dolor sit amet.");
					_ = await Target.ImportData(FILE_NAME);
				}
				finally
				{
					File.Delete(FILE_NAME);
				}
			}
		}
	}
}
