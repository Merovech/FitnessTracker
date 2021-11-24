using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessTracker.Core.ImportPreparer.Implementations;
using FitnessTracker.Core.ImportPreparer.Interfaces;
using FitnessTracker.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FitnessTracker.Core.Tests.ImportPreparer
{
	[TestClass]
	public class ImportPreparerFactoryTests
	{
		private ImportPreparerFactory _factory;

		[TestInitialize]
		public void InitializeTest()
		{
			_factory = new ImportPreparerFactory();
		}

		[TestMethod]
		[DynamicData(nameof(ValidTestData), DynamicDataSourceType.Property)]
		public void Should_Return_Correct_Preparer_For_Valid_Types(FileType fileType, Type preparerType)
		{
			var result = _factory.GetImportPreparer(fileType);
			Assert.IsInstanceOfType(result, preparerType, $"Wrong preparer found for {fileType} file type.");
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Should_Throw_For_Unknown_File_Type()
		{
			_ = _factory.GetImportPreparer(FileType.Unknown);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Should_Throw_For_Unknown_Invalid_Type()
		{
			_ = _factory.GetImportPreparer((FileType)31);
		}

		private static List<object[]> ValidTestData
		{
			get
			{
				return new List<object[]>
				{
					new object[] { FileType.Csv, typeof(CsvImportPreparer) },
					new object[] { FileType.Sqlite, typeof(SqliteImportPreparer) }
				};
			}
		}
	}
}
