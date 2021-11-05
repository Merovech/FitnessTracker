using System;
using System.Threading.Tasks;
using FitnessTracker.Messages;
using FitnessTracker.Services.Interfaces;
using FitnessTracker.Utilities;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace FitnessTracker.ViewModels
{
	public class AddEditDataViewModel : ViewModelBase
	{
		private readonly IDatabaseService _databaseService;

		private bool _isIdle;
		private DateTime _date;
		private double _weight;

		public AddEditDataViewModel(IDatabaseService databaseService)
		{
			Guard.AgainstNull(databaseService, nameof(databaseService));
			_databaseService = databaseService;

			UpsertCommand = new RelayCommand(async () => await Upsert());

			_date = DateTime.Today;

			Task.Run(async () => await RetrieveRecord());
		}

		public RelayCommand UpsertCommand { get; set; }

		public DateTime Date
		{
			get => _date;
			set
			{
				Set(nameof(Date), ref _date, value);
				Task.Run(async () => await RetrieveRecord());
			}
		}

		public double? Weight
		{
			// This is only a double so it can be properly validated in the edge case where the user tries to enter
			// no value at all.
			get => _weight;
			set => Set(nameof(Weight), ref _weight, value ?? 0);
		}

		public bool IsIdle
		{
			get => _isIdle;
			set => Set(nameof(IsIdle), ref _isIdle, value);
		}

		public string Error => string.Empty;

		private async Task RetrieveRecord()
		{
			IsIdle = false;
			var record = await _databaseService.GetRecordByDate(_date);
			Weight = record == null ? 0 : record.Weight;
			IsIdle = true;
		}

		private async Task Upsert()
		{
			await _databaseService.UpsertRecord(Date, Weight.Value);
			MessengerInstance.Send(new NewDataAvailableMessage());
		}
	}
}
