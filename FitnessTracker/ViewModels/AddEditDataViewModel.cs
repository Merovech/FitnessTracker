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
using System.ComponentModel;

namespace FitnessTracker.ViewModels
{
	public class AddEditDataViewModel : ViewModelBase, IDataErrorInfo
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

		public double? Weight
		{
			// This is only a double so it can be properly validated in the edge case where the user tries to enter
			// no value at all.
			get => _weight;
			set => Set(nameof(Weight), ref _weight, value ?? 0);
		}

		public double? Distance
		{
			get => _distance;
			set => Set(nameof(Distance), ref _distance, value);
		}

		public bool IsIdle
		{
			get => _isIdle;
			set => Set(nameof(IsIdle), ref _isIdle, value);
		}

		public string Error => string.Empty;

		public string this[string columnName]
		{
			get
			{
				// We have to use this method of data validation (as oposed to ValidateRules) for Distance because
				// Distance can be empty while weight cannot.  If we use the same approach here as we do in Weight,
				// then typing a value and deleting it yields "Cannot convert value ''".
				switch (columnName)
				{
					case "Distance":
						if (Distance.HasValue && Distance < 0)
						{
							return "Minimum 0\nCan be blank";
						}

						break;
				}

				return string.Empty;
			}
		}

		private async Task RetrieveRecord()
		{
			IsIdle = false;
			var record = await _databaseService.Get(_date);
			Weight = record == null ? 0 : record.Weight;
			Distance = record?.DistanceMoved;

			IsIdle = true;
		}

		private async Task Upsert()
		{
			await _databaseService.Upsert(Date, Weight.Value, Distance);
			MessengerInstance.Send(new NewDataAvailableMessage());
		}
	}
}
