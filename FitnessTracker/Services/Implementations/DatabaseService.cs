using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using FitnessTracker.Utilities;

namespace FitnessTracker.Services.Implementations
{
	public class DatabaseService : IDatabaseService
	{
		private DatabaseContext _context;
		private IDataCalculatorService _dataCalculatorService;

		public DatabaseService(DatabaseContext context, IDataCalculatorService dataCalculatorService)
		{
			Guard.AgainstNull(context, nameof(context));
			Guard.AgainstNull(dataCalculatorService, nameof(dataCalculatorService));

			_context = context;
			_dataCalculatorService = dataCalculatorService;
		}

		public async Task BulkAddAsync(IEnumerable<DailyRecord> records)
		{
			await _context.AddRangeAsync(records);
			_context.SaveChanges();
		}

		public void Add(DailyRecord record)
		{
			throw new NotImplementedException();
		}

		public DailyRecord Get(DateTime recordDate)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<DailyRecord>> GetAll()
		{
			var rawData = await Task.Run(() =>_context.Records.OrderBy(item => item.Date).ToList());
			_dataCalculatorService.FillCalculatedDataFields(rawData);
			return rawData;
		}

		public IEnumerable<DailyRecord> GetRange(DateTime startDate, DateTime endDate)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<DailyRecord> GetSince(DateTime startDate)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<DailyRecord> GetTo(DateTime endDate)
		{
			throw new NotImplementedException();
		}

		public void Update(DailyRecord record)
		{
			throw new NotImplementedException();
		}
	}
}
