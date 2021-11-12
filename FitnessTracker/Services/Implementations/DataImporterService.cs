using System;
using System.IO;
using System.Threading.Tasks;
using FitnessTracker.Services.Interfaces;
using FitnessTracker.Utilities;
using FitnessTracker.Utilities.ImportPreparer;
using FitnessTracker.Utilities.ImportPreparer.Interfaces;

namespace FitnessTracker.Services.Implementations
{
	public class DataImporterService : IDataImporterService
	{
		private readonly IDatabaseService _databaseService;
		private readonly IImportPreparerFactory _importPreparerFactory;

		public DataImporterService(IDatabaseService databaseService, IImportPreparerFactory importPreparerFactory)
		{
			Guard.AgainstNull(databaseService, nameof(databaseService));
			_databaseService = databaseService;

			Guard.AgainstNull(importPreparerFactory, nameof(importPreparerFactory));
			_importPreparerFactory = importPreparerFactory;
		}

		public async Task ImportData(string filePath)
		{
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException($"Cound not find file '{filePath}'.");
			}

			var importPreparer = _importPreparerFactory.GetImportPreparer(GetFileType(filePath));
			if (importPreparer == null)
			{
				throw new InvalidOperationException($"Could not find an appropriate import preparer for '{filePath}'.");
			}

			var records = importPreparer.GetRecords(filePath);
			await _databaseService.UpsertRecords(records);
		}

		private FileType GetFileType(string filePath)
		{
			var extension = Path.GetExtension(filePath);
			return extension switch
			{
				".csv" => FileType.Csv,
				".dat" => FileType.Sqlite,
				_ => FileType.Unknown,
			};
		}
	}
}
