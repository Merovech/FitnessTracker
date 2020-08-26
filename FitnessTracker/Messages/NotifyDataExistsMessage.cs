using GalaSoft.MvvmLight.Messaging;

namespace FitnessTracker.Messages
{
	/// <summary>
	/// Used to alert listeners that data may or may not exist for cases when listeners
	/// only need to have a yes/no answer and not a copy of the entire data set.
	/// </summary>
	public class NotifyDataExistsMessage : NotificationMessage<bool>
	{
		public NotifyDataExistsMessage(bool dataExists) : base(dataExists, null)
		{
		}
	}
}
