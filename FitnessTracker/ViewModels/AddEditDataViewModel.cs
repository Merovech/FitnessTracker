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
		private DateTime _date;
		private double _weight;
		private double? _distance;

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

		public double Weight
		{
			get => _weight;
			set => Set(nameof(Weight), ref _weight, value);
		}

		public double? Distance
		{
			get => _distance;
			set => Set(nameof(Weight), ref _distance, value);
		}

		public bool IsIdle
		{
			get => _isIdle;
			set => Set(nameof(IsIdle), ref _isIdle, value);
		}

		private async Task RetrieveRecord()
		{
			IsIdle = false;
			var record = await _databaseService.Get(_date);
			if (record != null)
			{
				Weight = record.Weight;
				Distance = record.DistanceMoved;
			}

			IsIdle = true;
		}

		private async Task Upsert()
		{
			await _databaseService.Upsert(Date, Weight, Distance);
			MessengerInstance.Send(new NewDataAvailableMessage());
		}
	}
}
