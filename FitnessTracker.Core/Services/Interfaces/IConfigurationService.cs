namespace FitnessTracker.Core.Services.Interfaces
{
	[DependencyInjectionType(DependencyInjectionType.Interface)]
	public interface IConfigurationService
	{
		public string DatabaseConnectionString { get; }

		public string SettingsFileName { get; }
	}
}
