using System.IO;
using System.Threading.Tasks;
using FitnessTracker.Core.ImportPreparer.Implementations;
using FitnessTracker.Core.ImportPreparer.Interfaces;
using Microsoft.Data.Sqlite;

namespace FitnessTracker.Core.Tests.Helpers.Builders
{
	public class SqliteImportPreparerBuilder : IBuilder<IImportPreparer>
	{
		public IImportPreparer Build()
		{
			return new SqliteImportPreparer();
		}

		public async Task CreateInvalidDatabaseTables()
		{
			DeleteExistingDatabaseIfExists();
			using (var conn = new SqliteConnection($"Data Source={Constants.IMPORT_DATABASE_FILENAME}"))
			{
				var command = new SqliteCommand { Connection = conn };
				await conn.OpenAsync();

				command.CommandText = "PRAGMA journal_mode=DELETE";
				await command.ExecuteNonQueryAsync();

				command.CommandText = @"CREATE TABLE ""SomeTable"" (
											""Id"" INTEGER NOT NULL CONSTRAINT ""PK_Records"" PRIMARY KEY AUTOINCREMENT,
											""Date"" TEXT NOT NULL,
											""Weight"" REAL NOT NULL
										)";

				await command.ExecuteNonQueryAsync();
			}
		}

		public async Task CreateInvalidDatabaseColumns()
		{
			DeleteExistingDatabaseIfExists();
			using (var conn = new SqliteConnection($"Data Source={Constants.IMPORT_DATABASE_FILENAME}"))
			{
				var command = new SqliteCommand { Connection = conn };
				await conn.OpenAsync();

				command.CommandText = "PRAGMA journal_mode=DELETE";
				await command.ExecuteNonQueryAsync();

				command.CommandText = @"CREATE TABLE ""Records"" (
											""Id"" INTEGER NOT NULL CONSTRAINT ""PK_Records"" PRIMARY KEY AUTOINCREMENT,
											""SomeColumn"" TEXT NOT NULL,
											""SomeOtherColumn"" REAL NOT NULL
										)";

				await command.ExecuteNonQueryAsync();
			}
		}

		public async Task CreateInvalidDatabaseDataTypesWithData()
		{
			DeleteExistingDatabaseIfExists();
			using (var conn = new SqliteConnection($"Data Source={Constants.IMPORT_DATABASE_FILENAME}"))
			{
				var command = new SqliteCommand { Connection = conn };
				await conn.OpenAsync();

				command.CommandText = "PRAGMA journal_mode=DELETE";
				await command.ExecuteNonQueryAsync();

				command.CommandText = @"CREATE TABLE ""Records"" (
											""Id"" INTEGER NOT NULL CONSTRAINT ""PK_Records"" PRIMARY KEY AUTOINCREMENT,
											""Date"" INTEGER NOT NULL,
											""Weight"" TEXT NOT NULL
										)";

				await command.ExecuteNonQueryAsync();

				command.CommandText = "INSERT INTO Records (Date, Weight) VALUES (12, 'some text')";
				await command.ExecuteNonQueryAsync();
			}
		}

		private void DeleteExistingDatabaseIfExists()
		{
			if (File.Exists(Constants.IMPORT_DATABASE_FILENAME))
			{
				SqliteConnection.ClearAllPools();
				File.Delete(Constants.IMPORT_DATABASE_FILENAME);
			}
		}
	}
}
