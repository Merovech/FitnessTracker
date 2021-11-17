using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessTracker.Models;

namespace FitnessTracker.Services.Interfaces
{
	public interface IDatabaseService
	{
		Task<DailyRecord> GetRecordByDate(DateTime recordDate);

		Task<IEnumerable<DailyRecord>> GetAllRecords();

		Task UpsertRecord(DateTime date, double weight);

		Task UpsertRecords(IEnumerable<DailyRecord> records);

		Task CreateDatabase();
	}
}
