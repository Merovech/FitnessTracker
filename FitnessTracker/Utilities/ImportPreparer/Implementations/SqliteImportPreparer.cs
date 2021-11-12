using System.Collections.Generic;
using FitnessTracker.Models;
using FitnessTracker.Utilities.ImportPreparer.Interfaces;

namespace FitnessTracker.Utilities.ImportPreparer.Implementations
{
	public class SqliteImportPreparer : IImportPreparer
	{
		public IEnumerable<DailyRecord> GetRecords(string fileName)
		{
			return null;
		}
	}
}
