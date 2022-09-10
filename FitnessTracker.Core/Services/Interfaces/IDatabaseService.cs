using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessTracker.Core.Models;

namespace FitnessTracker.Core.Services.Interfaces
{
	[DependencyInjectionType(DependencyInjectionType.Interface)]
	public interface IDatabaseService
	{
		Task<DailyRecord> GetRecordByDate(DateTime recordDate);

		Task<IEnumerable<DailyRecord>> GetAllRecords();

		Task UpsertRecord(DateTime date, double weight);

		Task UpsertRecords(IEnumerable<DailyRecord> records);

		Task CreateDatabase();
	}
}
