using FitnessTracker.Core.Models;

namespace FitnessTracker.Core.Services.Interfaces
{
	[DependencyInjectionType(DependencyInjectionType.Interface)]
	public interface ISettingsService
	{
		void SaveSettings(SystemSettings settings);

		SystemSettings ReadSettings();
	}
}
