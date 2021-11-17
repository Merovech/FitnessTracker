using System.Collections.Generic;
using FitnessTracker.Core.Models;
using GalaSoft.MvvmLight.Messaging;

namespace FitnessTracker.UI.Messages
{
	public class DataRetrievedMessage : NotificationMessage<IEnumerable<DailyRecord>>
	{
		public DataRetrievedMessage(IEnumerable<DailyRecord> content) : base(content, null)
		{
		}
	}
}
