using System.Threading.Tasks;

namespace FitnessTracker.Core.Services.Interfaces
{
	public interface IDataImporterService
	{
		Task<int> ImportData(string filePath);
	}
}
