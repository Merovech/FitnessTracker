﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FitnessTracker.Core.Models;
using FitnessTracker.Core.Services.Interfaces;
using FitnessTracker.Utilities;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace FitnessTracker.Core.Services.Implementations
{
	[DependencyInjectionType(DependencyInjectionType.Service)]
	public class DatabaseService : IDatabaseService
	{
		private const string GET_ALL_QUERY = "SELECT * FROM Records";
		private const string GET_SINGLE_QUERY = "SELECT * FROM Records WHERE Date=@Date";
		private const string UPDATE_QUERY = "UPDATE Records SET Weight=@Weight WHERE Date=@Date";
		private const string INSERT_QUERY = "INSERT INTO Records (Date, Weight) VALUES (@Date, @Weight)";
		private readonly IConfigurationService _configService;
		private readonly IDataCalculatorService _dataCalculatorService;
		private readonly ILogger<DatabaseService> _logger;

		public DatabaseService(IDataCalculatorService dataCalculatorService, IConfigurationService configService, ILogger<DatabaseService> logger)
		{
			Guard.AgainstNull(configService, nameof(configService));
			_configService = configService;

			Guard.AgainstNull(dataCalculatorService, nameof(dataCalculatorService));
			_dataCalculatorService = dataCalculatorService;

			Guard.AgainstNull(logger, nameof(logger));
			_logger = logger;
		}

		public async Task<IEnumerable<DailyRecord>> GetAllRecords()
		{
			_logger.LogInformation("Retrieving all records.");

			using (var conn = new SqliteConnection(_configService.DatabaseConnectionString))
			{
				var command = new SqliteCommand(GET_ALL_QUERY, conn);

				await conn.OpenAsync();
				var reader = await command.ExecuteReaderAsync();

				var records = new List<DailyRecord>();
				if (!reader.HasRows)
				{
					reader.Close();
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

				reader.Close();

				if (records.Any())
				{
					_dataCalculatorService.FillCalculatedDataFields(records);
				}

				_logger.LogInformation("Records found: {recordCount}", records.Count);
				return records;
			}
		}

		public async Task<DailyRecord> GetRecordByDate(DateTime recordDate)
		{
			_logger.LogInformation("Attempting to find record with date {date}", recordDate);

			using (var conn = new SqliteConnection(_configService.DatabaseConnectionString))
			{
				var command = new SqliteCommand(GET_SINGLE_QUERY, conn);
				command.Parameters.AddWithValue("@Date", recordDate);

				await conn.OpenAsync();
				var reader = await command.ExecuteReaderAsync();

				if (!reader.HasRows)
				{
					_logger.LogInformation("No record found.");
					return null;
				}

				_logger.LogInformation("Record found.");
				await reader.ReadAsync();
				var returnRecord = new DailyRecord { Id = reader.GetInt32(0), Date = reader.GetDateTime(1), Weight = reader.GetDouble(2) };
				reader.Close();
				return returnRecord;
			}
		}

		public async Task UpsertRecord(DateTime date, double weight)
		{
			_logger.LogInformation("Upserting record for date {date}", date);

			var existingRecord = await GetRecordByDate(date);
			using (var conn = new SqliteConnection(_configService.DatabaseConnectionString))
			{
				var cmdText = string.Empty;
				if (existingRecord == null)
				{
					_logger.LogInformation("No record found.  Inserting as new record.");
					cmdText = INSERT_QUERY;
				}
				else
				{
					_logger.LogInformation("Record found.  Updating existing record.");
					cmdText = UPDATE_QUERY;
				}

				var command = new SqliteCommand(cmdText, conn);
				command.Parameters.AddWithValue("@Date", date);
				command.Parameters.AddWithValue("@Weight", weight);

				await conn.OpenAsync();
				await command.ExecuteNonQueryAsync();
			}
		}

		public async Task UpsertRecords(IEnumerable<DailyRecord> records)
		{
			_logger.LogInformation("Upserting {count} records.", records.Count());

			Guard.AgainstNull(records, nameof(records));
			Guard.AgainstEmptyList(records, nameof(records));

			using (var conn = new SqliteConnection(_configService.DatabaseConnectionString))
			{
				await conn.OpenAsync();
				var transaction = conn.BeginTransaction();
				
				try
				{
					var command = new SqliteCommand(string.Empty, conn, transaction);
					foreach (var record in records)
					{
						command.Parameters.Clear();
						command.CommandText = await DoesRecordExist(record, command) ? UPDATE_QUERY : INSERT_QUERY;
						command.Parameters.Clear();

						command.Parameters.AddWithValue("@Date", record.Date);
						command.Parameters.AddWithValue("@Weight", record.Weight);

						command.ExecuteNonQuery();
					}

					transaction.Commit();
				}
				catch (Exception ex)
				{
					Debug.WriteLine("ERROR: " + ex.Message);
					Debug.WriteLine("Rolling back transaction.");
					transaction.Rollback();
					throw;
				}
			}
		}

		public async Task CreateDatabase()
		{
			_logger.LogInformation("Creating new database.");

			using (var conn = new SqliteConnection(_configService.DatabaseConnectionString))
			{
				var command = new SqliteCommand { Connection = conn };
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

		private async Task<bool> DoesRecordExist(DailyRecord record, SqliteCommand command)
		{
			command.CommandText = GET_SINGLE_QUERY;
			command.Parameters.AddWithValue("@Date", record.Date);
			var result = await command.ExecuteScalarAsync();
			return result != null;
		}
	}
}
