using FitnessTracker.Core;

namespace FitnessTracker.UI.Services.Interfaces
{
	[DependencyInjectionType(DependencyInjectionType.Interface)]
	public interface IConfigurationService
	{
		public string DatabaseConnectionString { get; }
	}
}
