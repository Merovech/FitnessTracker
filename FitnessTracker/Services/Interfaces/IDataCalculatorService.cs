using System.Collections.Generic;
using FitnessTracker.Models;

namespace FitnessTracker.Services.Interfaces
{
	public interface IDataCalculatorService
	{
		void FillCalculatedDataFields(IEnumerable<DailyRecord> data);

		SummaryStatistics CalculateSummaryStatistics(IEnumerable<DailyRecord> data);
	}
}
