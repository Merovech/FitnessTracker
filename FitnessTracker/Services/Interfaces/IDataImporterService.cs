using System.Threading.Tasks;

namespace FitnessTracker.Services.Interfaces
{
	public interface IDataImporterService
	{
		Task<int> ImportData(string filePath);
	}
}
