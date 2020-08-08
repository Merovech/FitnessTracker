using System.Linq;
using FitnessTracker.Messages;
using FitnessTracker.Services.Interfaces;
using FitnessTracker.Utilities;
using GalaSoft.MvvmLight;

namespace FitnessTracker.ViewModels
{
	public class DebugViewModel : ViewModelBase
	{
		private readonly IDatabaseService _databaseService;
		private int _recordCount;

		public DebugViewModel(IDatabaseService databaseService)
		{
			Guard.AgainstNull(databaseService, nameof(databaseService));
			_databaseService = databaseService;

			MessengerInstance.Register<DataRetrievedMessage>(this, (msg) => RecordCount = msg.Content.Count());
		}

		public int RecordCount
		{
			get => _recordCount;
			set => Set(ref _recordCount, value);
		}
	}
}
