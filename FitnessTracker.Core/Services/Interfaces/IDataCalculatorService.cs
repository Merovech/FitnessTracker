using System.Collections.Generic;
using FitnessTracker.Core.Models;

namespace FitnessTracker.Core.Services.Interfaces
{
	[DependencyInjectionType(DependencyInjectionType.Interface)]
	public interface IDataCalculatorService
	{
		void FillCalculatedDataFields(IEnumerable<DailyRecord> data);

		SummaryStatistics CalculateSummaryStatistics(IEnumerable<DailyRecord> data);
	}
}
