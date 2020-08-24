using FitnessTracker.Models;
using GalaSoft.MvvmLight.Messaging;

namespace FitnessTracker.Messages
{
	public class SystemSettingsChangedMessage : NotificationMessage<SystemSettings>
	{
		public SystemSettingsChangedMessage(SystemSettings settings) : base(settings, null)
		{
		}
	}
}
