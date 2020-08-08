using System.Threading.Tasks;

namespace FitnessTracker.Services.Interfaces
{
	public interface IDataImporterService
	{
		Task ImportData(string filePath);
	}
}
