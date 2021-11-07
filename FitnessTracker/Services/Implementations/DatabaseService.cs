using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using Microsoft.Data.Sqlite;

namespace FitnessTracker.Services.Implementations
{
	// TODO: Inject config service so that we get a connection string

	public class DatabaseService : IDatabaseService
	{
		private const string GET_ALL_QUERY = "SELECT * FROM Records";
		private const string GET_SINGLE_QUERY = "SELECT * FROM Records WHERE Date=@Date";
		private const string UPDATE_QUERY = "UPDATE Records SET Weight=@Weight WHERE Date=@Date";
		private const string INSERT_QUERY = "INSERT INTO Records (Date, Weight) VALUES (@Date, @Weight)";

		public async Task<IEnumerable<DailyRecord>> GetAllRecords()
		{
			var command = new SqliteCommand(GET_ALL_QUERY);
			var reader = await ExecuteReaderCommand(command);
			var records = new List<DailyRecord>();
			while (reader.Read())
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

		public async Task<DailyRecord> GetRecordByDate(DateTime recordDate)
		{
			var command = new SqliteCommand(GET_SINGLE_QUERY);
			command.Parameters.AddWithValue("@Date", recordDate);
			var reader = await ExecuteReaderCommand(command);
			reader.Read();
			return new DailyRecord { Id = reader.GetInt32(0), Date = reader.GetDateTime(1), Weight = reader.GetDouble(2) };
		}

		public async Task UpsertRecord(DateTime date, double weight)
		{
			var existingRecord = GetRecordByDate(date);
			var command = new SqliteCommand();
			if (existingRecord == null)
			{
				command.CommandText = INSERT_QUERY;
			}
			else
			{
				command.CommandText = UPDATE_QUERY;				
			}

			command.Parameters.AddWithValue("@Date", date);
			command.Parameters.AddWithValue("@Weight", weight);
			await ExecuteNonReaderCommand(command);

		}

		public async Task AddRecordsAsync(IEnumerable<DailyRecord> records)
		{
			using (SqliteConnection conn = new SqliteConnection())
			{
				conn.Open();
				var transaction = conn.BeginTransaction();
				try
				{
					foreach (var record in records)
					{
						var command = new SqliteCommand(INSERT_QUERY);
						command.Parameters.AddWithValue("@Date", record.Date);
						command.Parameters.AddWithValue("@Weight", record.Weight);
						await command.ExecuteNonQueryAsync();
						await transaction.CommitAsync();
					}
				}
				catch (Exception ex)
				{
					await transaction.RollbackAsync();
				}
				finally
				{
					await conn.CloseAsync();
				}
			}
		}

		private async Task<SqliteDataReader> ExecuteReaderCommand(SqliteCommand command)
		{
			using (SqliteConnection conn = new SqliteConnection())
			{
				conn.Open();
				var result = await command.ExecuteReaderAsync();
				await conn.CloseAsync();

				return result;
			}
		}

		private async Task ExecuteNonReaderCommand(SqliteCommand command)
		{
			using (SqliteConnection conn = new SqliteConnection())
			{
				conn.Open();
				await command.ExecuteNonQueryAsync();
				await conn.CloseAsync();
			}
		}
	}
}
