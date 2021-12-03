using CsvHelper.Configuration;
using FitnessTracker.Core.Models;

namespace FitnessTracker.Core.Utilities
{
	internal sealed class DailyRecordCsvMap : ClassMap<DailyRecord>
	{
		public DailyRecordCsvMap()
		{
			Map(m => m.Date).Index(0);
			Map(m => m.Weight).Index(1);
		}
	}
}
