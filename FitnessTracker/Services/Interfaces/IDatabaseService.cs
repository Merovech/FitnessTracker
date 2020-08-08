using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessTracker.Models;
using FitnessTracker.Services.Implementations;

namespace FitnessTracker.Services.Interfaces
{
	public interface IDatabaseService
	{
		DailyRecord Get(DateTime recordDate);

		IEnumerable<DailyRecord> GetRange(DateTime startDate, DateTime endDate);

		IEnumerable<DailyRecord> GetTo(DateTime endDate);

		IEnumerable<DailyRecord> GetSince(DateTime startDate);

		Task<IEnumerable<DailyRecord>> GetAll();

		void Add(DailyRecord record);

		Task BulkAddAsync(IEnumerable<DailyRecord> records);

		void Update(DailyRecord record);
	}
}
