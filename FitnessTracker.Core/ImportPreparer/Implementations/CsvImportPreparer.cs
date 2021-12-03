using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using FitnessTracker.Core.ImportPreparer.Interfaces;
using FitnessTracker.Core.Models;
using FitnessTracker.Core.Utilities;
using FitnessTracker.Utilities;

namespace FitnessTracker.Core.ImportPreparer.Implementations
{
	public class CsvImportPreparer : IImportPreparer
	{
		public Task<IEnumerable<DailyRecord>> GetRecords(string fileName)
		{
			Guard.AgainstNull(fileName, nameof(fileName));
			Guard.AgainstEmptyList(fileName, nameof(fileName));

			if (!File.Exists(fileName))
			{
				throw new FileNotFoundException($"File '{fileName}' does not exist.");
			}

			var config = new CsvConfiguration(CultureInfo.CurrentCulture)
			{
				HasHeaderRecord = false
			};

			using (var streamReader = new StreamReader(fileName))
			{
				using (var csvReader = new CsvReader(streamReader, config))
				{
					csvReader.Context.RegisterClassMap<DailyRecordCsvMap>();
					
					var records = csvReader.GetRecords<DailyRecord>();
					return Task.FromResult((IEnumerable<DailyRecord>)records.ToList());
				}
			}
		}
	}
}
