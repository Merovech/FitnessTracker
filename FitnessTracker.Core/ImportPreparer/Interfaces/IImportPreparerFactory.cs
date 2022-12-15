using FitnessTracker.Core.Models;

namespace FitnessTracker.Core.ImportPreparer.Interfaces
{
	[DependencyInjectionType(DependencyInjectionType.Interface)]
	public interface IImportPreparerFactory
	{
		IImportPreparer GetImportPreparer(FileType fileType);
	}
}
