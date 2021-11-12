using System.Collections.Generic;
using FitnessTracker.Models;

namespace FitnessTracker.Utilities.ImportPreparer.Interfaces
{
	public interface IImportPreparer
	{
		public IEnumerable<DailyRecord> GetRecords(string fileName);
	}
}
