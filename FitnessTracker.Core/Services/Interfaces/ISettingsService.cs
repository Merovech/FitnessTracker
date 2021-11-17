using FitnessTracker.Core.Models;

namespace FitnessTracker.Core.Services.Interfaces
{
	public interface ISettingsService
	{
		void SaveSettings(SystemSettings settings);

		SystemSettings ReadSettings();
	}
}
