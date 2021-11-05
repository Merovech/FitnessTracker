using FitnessTracker.Services.Interfaces;
using FitnessTracker.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FitnessTracker.Models
{
	public class DatabaseContext : DbContext
	{
		public static readonly ILoggerFactory SqlLogger = LoggerFactory.Create(builder => builder.AddDebug());
		private readonly IConfigurationService _configurationService;

		public DatabaseContext(IConfigurationService configurationService)
		{
			Guard.AgainstNull(configurationService, nameof(configurationService));
			_configurationService = configurationService;
		}

		public DbSet<DailyRecord> Records { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			if (_configurationService.LogEntitySQLToDebugWindow)
			{
				options.EnableSensitiveDataLogging();
				options.UseLoggerFactory(SqlLogger);
			}

			options.UseSqlite(_configurationService.DatabaseConnectionString);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<DailyRecord>().Ignore(dr => dr.MovingWeightAverage);
		}
	}
}
