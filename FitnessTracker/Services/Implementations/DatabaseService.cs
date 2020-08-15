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

		public async Task Add(DailyRecord record)
		{
			_context.Records.Add(record);
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

		public async Task Update(DailyRecord record)
		{
			_context.Update(record);
			await _context.SaveChangesAsync();
		}
	}
}
