using FitnessTracker.Core.Models;
using GalaSoft.MvvmLight.Messaging;

namespace FitnessTracker.UI.Messages
{
	public class SystemSettingsChangedMessage : NotificationMessage<SystemSettings>
	{
		public SystemSettingsChangedMessage(SystemSettings settings) : base(settings, null)
		{
		}
	}
}
