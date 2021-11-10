using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using FitnessTracker.Utilities;
using Microsoft.Data.Sqlite;

namespace FitnessTracker.Services.Implementations
{
	public class DatabaseService : IDatabaseService
	{
		private const string GET_ALL_QUERY = "SELECT * FROM Records";
		private const string GET_SINGLE_QUERY = "SELECT * FROM Records WHERE Date=@Date";
		private const string UPDATE_QUERY = "UPDATE Records SET Weight=@Weight WHERE Date=@Date";
		private const string INSERT_QUERY = "INSERT INTO Records (Date, Weight) VALUES (@Date, @Weight)";
		private IConfigurationService _configService;

		public DatabaseService(IConfigurationService configService)
		{
			Guard.AgainstNull(configService, nameof(configService));
			_configService = configService;
		}

		public async Task<IEnumerable<DailyRecord>> GetAllRecords()
		{
			using (SqliteConnection conn = new SqliteConnection(_configService.DatabaseConnectionString))
			{
				var command = new SqliteCommand(GET_ALL_QUERY, conn);

				await conn.OpenAsync();
				var reader = await command.ExecuteReaderAsync();

				var records = new List<DailyRecord>();
				if (!reader.HasRows)
				{
					return new List<DailyRecord>();
				}

				while (await reader.ReadAsync())
				{
					var record = new DailyRecord()
					{
						Id = reader.GetInt32(0),
						Date = reader.GetDateTime(1),
						Weight = reader.GetDouble(2)
					};

					records.Add(record);
				}

				return records;
			}
		}

		public async Task<DailyRecord> GetRecordByDate(DateTime recordDate)
		{
			using (SqliteConnection conn = new SqliteConnection(_configService.DatabaseConnectionString))
			{
				var command = new SqliteCommand(GET_SINGLE_QUERY, conn);
				command.Parameters.AddWithValue("@Date", recordDate);

				await conn.OpenAsync();
				var reader = await command.ExecuteReaderAsync();
				
				if (!reader.HasRows)
				{
					return null;
				}

				await reader.ReadAsync();
				return new DailyRecord { Id = reader.GetInt32(0), Date = reader.GetDateTime(1), Weight = reader.GetDouble(2) };
			}
		}

		public async Task UpsertRecord(DateTime date, double weight)
		{
			var existingRecord = await GetRecordByDate(date);
			using (SqliteConnection conn = new SqliteConnection(_configService.DatabaseConnectionString))
			{
				var cmdText = string.Empty;
				if (existingRecord == null)
				{
					cmdText = INSERT_QUERY;
				}
				else
				{
					cmdText = UPDATE_QUERY;
				}

				var command = new SqliteCommand(cmdText, conn);
				command.Parameters.AddWithValue("@Date", date);
				command.Parameters.AddWithValue("@Weight", weight);
				
				await conn.OpenAsync();
				await command.ExecuteNonQueryAsync();
			}
		}

		public async Task AddRecordsAsync(IEnumerable<DailyRecord> records)
		{
			using (SqliteConnection conn = new SqliteConnection(_configService.DatabaseConnectionString))
			{
				await conn.OpenAsync();
				var transaction = conn.BeginTransaction();
				try
				{
					foreach (var record in records)
					{
						var command = new SqliteCommand(INSERT_QUERY, conn, transaction);
						command.Parameters.AddWithValue("@Date", record.Date);
						command.Parameters.AddWithValue("@Weight", record.Weight);
						await command.ExecuteNonQueryAsync();
					}

					await transaction.CommitAsync();
				}
				catch (Exception)
				{
					await transaction.RollbackAsync();
				}
			}
		}

		public async Task CreateDatabase()
		{
			using (SqliteConnection conn = new SqliteConnection(_configService.DatabaseConnectionString))
			{		
				var command = new SqliteCommand();
				command.Connection = conn;
				await conn.OpenAsync();

				command.CommandText = "PRAGMA journal_mode=DELETE";
				await command.ExecuteNonQueryAsync();

				command.CommandText = @"CREATE TABLE ""Records"" (
											""Id"" INTEGER NOT NULL CONSTRAINT ""PK_Records"" PRIMARY KEY AUTOINCREMENT,
											""Date"" TEXT NOT NULL,
											""Weight"" REAL NOT NULL
										)";
				
				await command.ExecuteNonQueryAsync();
			}
		}
	}
}
