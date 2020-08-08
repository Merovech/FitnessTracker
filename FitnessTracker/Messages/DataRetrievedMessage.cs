using System.Collections.Generic;
using FitnessTracker.Models;
using GalaSoft.MvvmLight.Messaging;

namespace FitnessTracker.Messages
{
	public class DataRetrievedMessage : NotificationMessage<IEnumerable<DailyRecord>>
	{
		public DataRetrievedMessage(IEnumerable<DailyRecord> content) : base(content, null)
		{
		}
	}
}
