using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessTracker.Core.Models;

namespace FitnessTracker.Core.ImportPreparer.Interfaces
{
	public interface IImportPreparer
	{
		public Task<IEnumerable<DailyRecord>> GetRecords(string fileName);
	}
}
