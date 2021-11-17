using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessTracker.Models;

namespace FitnessTracker.Utilities.ImportPreparer.Interfaces
{
	public interface IImportPreparer
	{
		public Task<IEnumerable<DailyRecord>> GetRecords(string fileName);
	}
}
