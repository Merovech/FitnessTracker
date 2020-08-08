using System.Collections.Generic;
using FitnessTracker.Models;
using GalaSoft.MvvmLight.Messaging;

namespace FitnessTracker.Messages
{
	public class GenerateStatisticsMessage : NotificationMessage<IEnumerable<DailyRecord>>
	{
		public GenerateStatisticsMessage(IEnumerable<DailyRecord> data) : base(data, "")
		{
		}
	}
}
