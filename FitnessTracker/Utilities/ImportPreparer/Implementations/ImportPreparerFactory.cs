using FitnessTracker.Utilities.ImportPreparer.Interfaces;
using System;

namespace FitnessTracker.Utilities.ImportPreparer.Implementations
{
	public class ImportPreparerFactory : IImportPreparerFactory
	{
		public IImportPreparer GetImportPreparer(FileType fileType)
		{
			return fileType switch
			{
				FileType.Csv => new CsvImportPreparer(),
				FileType.Sqlite => new SqliteImportPreparer(),
				FileType.Unknown => throw new InvalidOperationException($"Unable to find an import preparer for file of type '{fileType}'."),
				_ => null,
			};
		}
	}
}
