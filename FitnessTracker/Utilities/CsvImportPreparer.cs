using System;
using System.Collections.Generic;
using System.IO;
using FitnessTracker.Models;
using FitnessTracker.Utilities.ImportPreparer.Interfaces;

namespace FitnessTracker.Utilities
{
	public class CsvImportPreparer : IImportPreparer
	{
		public IEnumerable<DailyRecord> GetRecords(string fileName)
		{
			var allLines = File.ReadAllLines(fileName);
			var records = new List<DailyRecord>();

			foreach (var line in allLines)
			{
				records.Add(ConvertToDailyRecord(line.Split(',')));
			}

			return records;
		}

		private DailyRecord ConvertToDailyRecord(string[] rawData)
		{
			if (!DateTime.TryParse(rawData[0], out var date))
			{
				throw new InvalidOperationException($"Invalid data for parsing date: [{rawData[0]}].");
			}

			// Convert empty strings to zeroes
			if (string.IsNullOrEmpty(rawData[1]))
			{
				rawData[1] = "0";
			}

			if (string.IsNullOrEmpty(rawData[2]))
			{
				rawData[2] = "0";
			}

			if (!double.TryParse(rawData[1], out var weight))
			{
				throw new InvalidOperationException($"Invalid data for parsing weight: [{rawData[1]}].");
			}

			return new DailyRecord
			{
				Date = date,
				Weight = weight,
			};
		}
	}
}
