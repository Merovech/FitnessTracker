using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessTracker.Core.Models;
using FitnessTracker.Core.ImportPreparer.Interfaces;
using Microsoft.Data.Sqlite;
using FitnessTracker.Utilities;
using System.IO;

namespace FitnessTracker.Core.ImportPreparer.Implementations
{
	public class SqliteImportPreparer : IImportPreparer
	{
		public async Task<IEnumerable<DailyRecord>> GetRecords(string fileName)
		{
			Guard.AgainstNull(fileName, nameof(fileName));
			Guard.AgainstEmptyString(fileName, nameof(fileName));

			if (!File.Exists(fileName))
			{
				throw new FileNotFoundException($"File '{fileName}' does not exist.");
			}

			var connectionString = $"Data Source={fileName};";

			using (var conn = new SqliteConnection(connectionString))
			{
				try
				{
					var returnList = new List<DailyRecord>();
					var command = new SqliteCommand("SELECT * FROM Records ORDER BY Date", conn);
					await conn.OpenAsync();
					var reader = await command.ExecuteReaderAsync();
					while (reader.Read())
					{
						if (DateTime.TryParse(reader["Date"].ToString(), out var date) && double.TryParse(reader["Weight"].ToString(), out var weight))
						{
							returnList.Add(new DailyRecord { Date = date, Weight = weight });
						}
						else
						{
							throw new InvalidOperationException("Data format in the row was invalid.  Unable to parse either date or weight value.");
						}
					}

					return returnList;
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException("File format is invalid.", ex);
				}
			}
		}
	}
}
