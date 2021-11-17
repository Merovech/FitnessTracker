using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FitnessTracker.Core.Models;
using FitnessTracker.Core.Services.Interfaces;
using FitnessTracker.UI.Messages;
using FitnessTracker.Utilities;
using GalaSoft.MvvmLight;

namespace FitnessTracker.UI.ViewModels
{
	public class RawDataViewModel : ViewModelBase
	{
		private readonly IDatabaseService _databaseService;
		private ObservableCollection<DailyRecord> _data;

		public RawDataViewModel(IDatabaseService databaseService)
		{
			Guard.AgainstNull(databaseService, nameof(databaseService));
			_databaseService = databaseService;
			_data = new ObservableCollection<DailyRecord>();

			MessengerInstance.Register<NewDataAvailableMessage>(this, async (msg) => await RefreshData());
			Task.Run(async () => await RefreshData());
		}

		public ObservableCollection<DailyRecord> Data
		{
			get => _data;
			set => Set(ref _data, value);
		}

		private async Task RefreshData()
		{
			var newData = await _databaseService.GetAllRecords();
			Data = new ObservableCollection<DailyRecord>(newData);

			MessengerInstance.Send(new DataRetrievedMessage(newData));
			MessengerInstance.Send(new NotifyDataExistsMessage(newData.Any()));
		}
	}
}
