using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FitnessTracker.Models;
using GalaSoft.MvvmLight;
using FitnessTracker.Utilities;
using FitnessTracker.Services.Interfaces;
using GalaSoft.MvvmLight.Command;
using FitnessTracker.Messages;

namespace FitnessTracker.ViewModels
{
	public class AddEditDataViewModel : ViewModelBase
	{
		private readonly IDatabaseService _databaseService;

		private bool _isIdle;
		private DailyRecord _currentRecord;

		public AddEditDataViewModel(IDatabaseService databaseService)
		{
			Guard.AgainstNull(databaseService, nameof(databaseService));
			_databaseService = databaseService;

			UpsertCommand = new RelayCommand(async () => await Upsert());

			CurrentRecord = new DailyRecord { Id = 0, Date = DateTime.Today };

			Task.Run(async () => await RetrieveRecord());
		}

		public RelayCommand UpsertCommand { get; set; }

		public bool IsNewDataPoint
		{
			get => CurrentRecord?.Id == 0;
		}

		public bool IsIdle
		{
			get => _isIdle;
			set => Set(nameof(IsIdle), ref _isIdle, value);
		}

		public DailyRecord CurrentRecord
		{
			get => _currentRecord;
			set
			{
				_currentRecord = value;
				RaisePropertyChanged(nameof(IsNewDataPoint));
				RaisePropertyChanged(nameof(CurrentRecord));
			}
		}

		public DateTime CurrentRecordDate
		{
			get => CurrentRecord.Date;
			set
			{
				CurrentRecord.Date = value;
				RaisePropertyChanged(nameof(CurrentRecordDate));
				Task.Run(async () => await RetrieveRecord());
			}
		}

		private async Task RetrieveRecord()
		{
			IsIdle = false;
			var record = await _databaseService.Get(CurrentRecordDate.Date);
			if (record == null)
			{
				record = new DailyRecord { Date = CurrentRecord.Date };
			}

			CurrentRecord = record;
			IsIdle = true;
		}

		private async Task Upsert()
		{
			if (IsNewDataPoint)
			{
				await _databaseService.Add(CurrentRecord);
			}
			else
			{
				await _databaseService.Update(CurrentRecord);
			}

			MessengerInstance.Send(new NewDataAvailableMessage());
		}
	}
}
