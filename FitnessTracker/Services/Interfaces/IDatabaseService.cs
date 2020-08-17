using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessTracker.Models;

namespace FitnessTracker.Services.Interfaces
{
	public interface IDatabaseService
	{
		Task<DailyRecord> Get(DateTime recordDate);

		IEnumerable<DailyRecord> GetRange(DateTime startDate, DateTime endDate);

		IEnumerable<DailyRecord> GetTo(DateTime endDate);

		IEnumerable<DailyRecord> GetSince(DateTime startDate);

		Task<IEnumerable<DailyRecord>> GetAll();

		Task Upsert(DateTime date, double weight, double? distance);

		Task BulkAddAsync(IEnumerable<DailyRecord> records);
	}
}
