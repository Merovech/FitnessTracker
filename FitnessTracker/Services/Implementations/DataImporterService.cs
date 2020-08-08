using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using FitnessTracker.Utilities;

namespace FitnessTracker.Services.Implementations
{
	public class DataImporterService : IDataImporterService
	{
		private readonly IDatabaseService _databaseService;

		public DataImporterService(IDatabaseService databaseService)
		{
			Guard.AgainstNull(databaseService, nameof(databaseService));
			_databaseService = databaseService;
		}

		public async Task ImportData(string filePath)
		{
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException($"Cound not find file '{filePath}'.");
			}

			var allLines = File.ReadAllLines(filePath);
			var recordsToAdd = new List<DailyRecord>();

			foreach (var line in allLines)
			{
				recordsToAdd.Add(ConvertToDailyRecord(line.Split(',')));
			}

			await _databaseService.BulkAddAsync(recordsToAdd);
		}

		private DailyRecord ConvertToDailyRecord(string[] rawData)
		{
			if (!DateTime.TryParse(rawData[0], out var date))
			{
				throw new InvalidOperationException($"Invalid data for parsing date: [{rawData[0]}].");
			}

			if (!double.TryParse(rawData[1], out double weight))
			{
				throw new InvalidOperationException($"Invalid data for parsing weight: [{rawData[1]}].");
			}

			if (!Helpers.TryParse(rawData[2], double.TryParse, out double? distanceMoved))
			{
				throw new InvalidOperationException($"Invalid data for parsing distance moved: [{rawData[3]}].");
			}

			return new DailyRecord
			{
				Date = date,
				Weight = weight,
				DistanceMoved = distanceMoved
			};
		}
	}
}
