using FitnessTracker.Models;

namespace FitnessTracker.Services.Interfaces
{
	public interface ISettingsService
	{
		void SaveSettings(SystemSettings settings);

		SystemSettings ReadSettings();
	}
}
