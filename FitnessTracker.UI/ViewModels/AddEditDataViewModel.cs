using System;
using System.Threading.Tasks;
using FitnessTracker.Core;
using FitnessTracker.Core.Services.Interfaces;
using FitnessTracker.UI.Messages;
using FitnessTracker.Utilities;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Extensions.Logging;

namespace FitnessTracker.UI.ViewModels
{
	[DependencyInjectionType(DependencyInjectionType.Other)]
	public class AddEditDataViewModel : ViewModelBase
	{
		private readonly IDatabaseService _databaseService;
		private readonly ILogger<AddEditDataViewModel> _logger;
		private bool _isIdle;
		private DateTime _date;
		private double _weight;

		public AddEditDataViewModel(IDatabaseService databaseService, ILogger<AddEditDataViewModel> logger)
		{
			Guard.AgainstNull(databaseService, nameof(databaseService));
			_databaseService = databaseService;

			Guard.AgainstNull(logger, nameof(logger));
			_logger = logger;

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
			_logger.LogTrace($"Found{(record == null ? " no" : string.Empty)} record for date {_date.ToShortDateString()}.");
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
