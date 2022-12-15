using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FitnessTracker.Core;
using FitnessTracker.Core.Models;
using FitnessTracker.Core.Services.Interfaces;
using FitnessTracker.UI.Messages;
using FitnessTracker.Utilities;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Logging;

namespace FitnessTracker.UI.ViewModels
{
	[DependencyInjectionType(DependencyInjectionType.Other)]
	public class RawDataViewModel : ViewModelBase
	{
		private readonly IDatabaseService _databaseService;
		private readonly ILogger<RawDataViewModel> _logger;
		private ObservableCollection<DailyRecord> _data;

		public RawDataViewModel(IDatabaseService databaseService, ILogger<RawDataViewModel> logger)
		{
			Guard.AgainstNull(databaseService, nameof(databaseService));
			_databaseService = databaseService;

			Guard.AgainstNull(logger, nameof(logger));
			_logger = logger;

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
			_logger.LogDebug("Refreshing data.");
			var newData = await _databaseService.GetAllRecords();
			_logger.LogDebug("Found {count} records.", newData.Count());
			Data = new ObservableCollection<DailyRecord>(newData);

			MessengerInstance.Send(new DataRetrievedMessage(newData));
			MessengerInstance.Send(new NotifyDataExistsMessage(newData.Any()));
		}
	}
}
