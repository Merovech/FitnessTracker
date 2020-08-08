using GalaSoft.MvvmLight.Messaging;

namespace FitnessTracker.Messages
{
	public class NewDataAvailableMessage : NotificationMessage
	{
		public NewDataAvailableMessage() : base(null)
		{
		}
	}
}
