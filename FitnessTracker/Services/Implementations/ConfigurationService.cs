using System.Configuration;
using FitnessTracker.Services.Interfaces;

namespace FitnessTracker.Services.Implementations
{
	public class ConfigurationService : IConfigurationService
	{
		private const string CONNECTIONSTRING_KEY = "DataFileConnectionString";
		private const string LOGENTITYSQLTODEBUGWINDOW_KEY = "LogEntitySQLToDebugWindow";

		public string DatabaseConnectionString => ConfigurationManager.ConnectionStrings[CONNECTIONSTRING_KEY].ConnectionString;

		public bool LogEntitySQLToDebugWindow => bool.Parse(ConfigurationManager.AppSettings[LOGENTITYSQLTODEBUGWINDOW_KEY]);
	}
}
