namespace FitnessTracker.Utilities.ImportPreparer.Interfaces
{
	public interface IImportPreparerFactory
	{
		IImportPreparer GetImportPreparer(FileType fileType);
	}
}
