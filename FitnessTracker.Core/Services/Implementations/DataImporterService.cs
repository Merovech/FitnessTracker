using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FitnessTracker.Core.ImportPreparer.Interfaces;
using FitnessTracker.Core.Models;
using FitnessTracker.Core.Services.Interfaces;
using FitnessTracker.Utilities;

namespace FitnessTracker.Core.Services.Implementations
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

		public async Task<int> ImportData(string filePath)
		{
			Guard.AgainstNull(filePath, nameof(filePath));
			Guard.AgainstEmptyString(filePath, nameof(filePath));

			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException($"Cound not find file '{filePath}'.");
			}

			var importPreparer = _importPreparerFactory.GetImportPreparer(GetFileType(filePath));
			if (importPreparer == null)
			{
				throw new InvalidOperationException($"Could not find an appropriate import preparer for '{filePath}'.");
			}

			var records = await importPreparer.GetRecords(filePath);
			await _databaseService.UpsertRecords(records);

			return records.Count();
		}

		private FileType GetFileType(string filePath)
		{
			var extension = Path.GetExtension(filePath);
			return extension switch
			{
				".csv" => FileType.Csv,
				".dat" or ".ft" => FileType.Sqlite,
				_ => FileType.Unknown,
			};
		}
	}
}
