using FitnessTracker.Core.ImportPreparer.Interfaces;
using FitnessTracker.Core.Models;
using System;

namespace FitnessTracker.Core.ImportPreparer.Implementations
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
				_ => throw new InvalidOperationException($"Unable to find an import preparer for file of type '{fileType}'.")
			};
		}
	}
}
