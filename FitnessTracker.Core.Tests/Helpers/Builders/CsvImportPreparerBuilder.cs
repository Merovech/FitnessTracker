using System.Collections.Generic;
using System.IO;
using FitnessTracker.Core.ImportPreparer.Implementations;
using FitnessTracker.Core.ImportPreparer.Interfaces;
using FitnessTracker.Core.Models;

namespace FitnessTracker.Core.Tests.Helpers.Builders
{
	public class CsvImportPreparerBuilder : IBuilder<IImportPreparer>
	{
		public IImportPreparer Build()
		{
			return new CsvImportPreparer();
		}

		public void CreateValidData(List<DailyRecord> data)
		{
			var lines = new List<string>();
			foreach (var item in data)
			{
				lines.Add($"{item.Date},{item.Weight}");
			}

			File.WriteAllLines(Constants.IMPORT_CSV_FILENAME, lines);
		}

		public void CreateInvalidData()
		{
			File.WriteAllText(Constants.IMPORT_CSV_FILENAME, "Lorem ipsum dolor sit amet.");
		}

		public void CreateUnparseableData()
		{
			File.WriteAllText(Constants.IMPORT_CSV_FILENAME, "abc,lorem ipsum dolor sit amet");
		}
	}
}
