using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessTracker.Core.Services.Implementations;
using FitnessTracker.Core.Services.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FitnessTracker.Core.Tests.ImportPreparer
{
	[TestClass]
	public class SqliteImportPreparerTests
	{
		private readonly string DATA_FILE_NAME = "sql_import_test_data.dat";
		private readonly IDatabaseService _databaseService;

		public SqliteImportPreparerTests()
		{
			var configMock = new Mock<IConfigurationService>();
			configMock.Setup(c => c.DatabaseConnectionString).Returns($"Source={DATA_FILE_NAME}");

			_databaseService = new DatabaseService(new DataCalculatorService(), configMock.Object);

		}

		[TestInitialize]
		public virtual void InitializeTest()
		{
			// Recreate a fresh DB each time.
			Task.Run(() => _databaseService.CreateDatabase()).Wait();
		}

		[TestCleanup]
		public void CleanupTest()
		{
			// Delete the test database so it can be recreated fresh.
			if (File.Exists(DATA_FILE_NAME))
			{
				// Ensures all cached Sqlite connections (hung on to as an optimization)
				// are removed and that the file can be safely deleted.  See here for more
				// details: https://stackoverflow.com/a/24570408/112829
				SqliteConnection.ClearAllPools();

				File.Delete(DATA_FILE_NAME);
			}
		}
	}
}
