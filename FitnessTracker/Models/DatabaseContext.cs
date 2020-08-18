using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FitnessTracker.Models
{
	public class DatabaseContext : DbContext
	{
		public static readonly ILoggerFactory SqlLogger = LoggerFactory.Create(builder => builder.AddDebug());

		public DbSet<DailyRecord> Records { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			options.EnableSensitiveDataLogging();
			options.UseLoggerFactory(SqlLogger);
			options.UseSqlite("Data Source=data.dat");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<DailyRecord>().Ignore(dr => dr.MovingWeightAverage);
			modelBuilder.Entity<DailyRecord>().Ignore(dr => dr.AverageDistanceMoved);
		}
	}
}
