using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Models
{
	public class DatabaseContext : DbContext
	{
		public DbSet<DailyRecord> Records { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			options.UseSqlite("Data Source=data.dat");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<DailyRecord>().Ignore(dr => dr.MovingWeightAverage);
			modelBuilder.Entity<DailyRecord>().Ignore(dr => dr.AverageDistanceMoved);
		}
	}
}
