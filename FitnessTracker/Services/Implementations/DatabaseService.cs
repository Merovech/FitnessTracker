using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using FitnessTracker.Utilities;
using Microsoft.EntityFrameworkCore;

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

		public async Task Upsert(DateTime date, double weight)
		{
			var existingRecord = await _context.Records.FirstOrDefaultAsync(x => x.Date == date);
			if (existingRecord == null)
			{
				_context.Records.Add(new DailyRecord { Date = date, Weight = weight});
			}
			else
			{
				var entry = _context.Entry(existingRecord);
				entry.Entity.Weight = weight;
				entry.State = EntityState.Modified;
			}

			await _context.SaveChangesAsync();
		}

		public async Task<DailyRecord> Get(DateTime recordDate)
		{
			return await Task.Run(() => _context.Records.Where(r => r.Date == recordDate).FirstOrDefault());
		}

		public async Task<IEnumerable<DailyRecord>> GetAll()
		{
			var rawData = await _context.Records.AsNoTracking().ToListAsync();
			rawData = rawData.OrderBy(item => item.Date).ToList();
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
	}
}
