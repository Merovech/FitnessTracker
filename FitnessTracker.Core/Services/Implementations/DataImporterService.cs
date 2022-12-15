using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FitnessTracker.Core.ImportPreparer.Interfaces;
using FitnessTracker.Core.Models;
using FitnessTracker.Core.Services.Interfaces;
using FitnessTracker.Utilities;
using Microsoft.Extensions.Logging;

namespace FitnessTracker.Core.Services.Implementations
{
	[DependencyInjectionType(DependencyInjectionType.Service)]
	public class DataImporterService : IDataImporterService
	{
		private readonly IDatabaseService _databaseService;
		private readonly IImportPreparerFactory _importPreparerFactory;
		private readonly ILogger _logger;

		public DataImporterService(IDatabaseService databaseService, IImportPreparerFactory importPreparerFactory, ILogger<DataImporterService> logger)
		{
			Guard.AgainstNull(databaseService, nameof(databaseService));
			_databaseService = databaseService;

			Guard.AgainstNull(importPreparerFactory, nameof(importPreparerFactory));
			_importPreparerFactory = importPreparerFactory;

			Guard.AgainstNull(logger, nameof(logger));
			_logger = logger;
		}

		public async Task<int> ImportData(string filePath)
		{
			Guard.AgainstNull(filePath, nameof(filePath));
			Guard.AgainstEmptyString(filePath, nameof(filePath));

			_logger.LogInformation("Importing data from '{file}'", filePath);
			if (!File.Exists(filePath))
			{
				_logger.LogError("File not found.");
				throw new FileNotFoundException($"Cound not find file '{filePath}'.");
			}

			var fileType = GetFileType(filePath);
			var importPreparer = _importPreparerFactory.GetImportPreparer(fileType);
			_logger.LogInformation("File type is {type}", fileType);
			if (importPreparer == null)
			{
				_logger.LogError("Cound not find an appropriate import preparer for '{filePath}'", filePath);
				throw new InvalidOperationException($"Could not find an appropriate import preparer for '{filePath}'.");
			}

			_logger.LogTrace("Preparer type is {type}", importPreparer.GetType().Name);

			var records = await importPreparer.GetRecords(filePath);
			_logger.LogInformation("Found {count} records to import.", records.Count());
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
