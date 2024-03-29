﻿using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessTracker.Core;
using FitnessTracker.Core.Models;
using FitnessTracker.Core.Services.Interfaces;
using FitnessTracker.UI.Messages;
using FitnessTracker.Utilities;
using GalaSoft.MvvmLight;

namespace FitnessTracker.UI.ViewModels
{
	[DependencyInjectionType(DependencyInjectionType.Other)]
	public class SummaryViewModel : ViewModelBase
	{
		private readonly IDataCalculatorService _dataCalculatorService;
		private readonly ISettingsService _settingsService;

		private SummaryStatistics _summaryStatistics;
		private SystemSettings _systemSettings;

		public SummaryViewModel(IDataCalculatorService dataCalculatorService, ISettingsService settingsService)
		{
			Guard.AgainstNull(dataCalculatorService, nameof(dataCalculatorService));
			Guard.AgainstNull(settingsService, nameof(settingsService));


			_dataCalculatorService = dataCalculatorService;
			_settingsService = settingsService;

			MessengerInstance.Register<DataRetrievedMessage>(this, async msg => await GetSummaryStatistics(msg.Content));
			MessengerInstance.Register<SystemSettingsChangedMessage>(this, msg => SystemSettings = msg.Content);

			// Due to weirdness in how the views are initially instantiated, we need to retrieve the settings once here.
			// Otherwise, the settings view model seems to instantiate, retrieve settings, and send out its message before
			// this view model exists to retrieve the message.
			SystemSettings = _settingsService.ReadSettings();
		}

		public SummaryStatistics SummaryData
		{
			get => _summaryStatistics;
			set
			{
				Set(nameof(SummaryData), ref _summaryStatistics, value);
				RaisePropertyChanged(nameof(CurrentWeight));
				RaisePropertyChanged(nameof(TotalWeightChange));
				RaisePropertyChanged(nameof(WeightChangeSinceLast));
				RaisePropertyChanged(nameof(LowestWeight));
				RaisePropertyChanged(nameof(HighestWeight));
			}
		}

		public double? CurrentWeight => _summaryStatistics?.CurrentWeight;

		public double? TotalWeightChange => _summaryStatistics?.TotalWeightChange;

		public double? WeightChangeSinceLast => _summaryStatistics?.WeightChangeSincePrevious;

		public double? LowestWeight => _summaryStatistics?.LowestWeight;

		public double? HighestWeight => _summaryStatistics?.HighestWeight;

		public SystemSettings SystemSettings
		{
			get => _systemSettings;
			set => Set(nameof(SystemSettings), ref _systemSettings, value);
		}

		private async Task GetSummaryStatistics(IEnumerable<DailyRecord> data)
		{
			SummaryData = await Task.Run(() => _dataCalculatorService.CalculateSummaryStatistics(data));
		}
	}
}
