namespace FitnessTracker.Services.Interfaces
{
	public interface IConfigurationService
	{
		public string DatabaseConnectionString { get; }

		public bool LogEntitySQLToDebugWindow { get; }
	}
}
