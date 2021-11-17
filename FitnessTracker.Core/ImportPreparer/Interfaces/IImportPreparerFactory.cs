using FitnessTracker.Core.Models;

namespace FitnessTracker.Core.ImportPreparer.Interfaces
{
	public interface IImportPreparerFactory
	{
		IImportPreparer GetImportPreparer(FileType fileType);
	}
}
