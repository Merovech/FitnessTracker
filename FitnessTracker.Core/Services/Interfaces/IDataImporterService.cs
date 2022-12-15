using System.Threading.Tasks;

namespace FitnessTracker.Core.Services.Interfaces
{
	[DependencyInjectionType(DependencyInjectionType.Interface)]
	public interface IDataImporterService
	{
		Task<int> ImportData(string filePath);
	}
}
