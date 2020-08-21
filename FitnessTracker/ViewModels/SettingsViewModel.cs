using System;
using System.Collections.Generic;
using System.Text;
using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using FitnessTracker.Utilities;
using GalaSoft.MvvmLight;

namespace FitnessTracker.ViewModels
{
	public class SettingsViewModel : ViewModelBase
	{
		private readonly ISettingsService _settingsService;
		private SystemSettings _systemSettings;

		// Due to nullability, it's easier to track the graph settings via separate properties
		private bool _overrideWeightGraphMinimum;
		private bool _overrideWeightGraphMaximum;
		private bool _overrideDistanceGraphMinimum;
		private bool _overrideDistanceGraphMaximum;

		private double _weightGraphMinimum;
		private double _weightGraphMaximum;
		private double _distanceGraphMinimum;
		private double _distanceGraphMaximum;

		public SettingsViewModel(ISettingsService settingsService)
		{
			Guard.AgainstNull(settingsService, nameof(settingsService));
			_settingsService = settingsService;
		}

		public SystemSettings SystemSettings
		{
			get => _systemSettings ?? new SystemSettings();
			set => Set(nameof(SystemSettings), ref _systemSettings, value);
		}

		public bool OverrideWeightGraphMinimum
		{
			get => _overrideWeightGraphMinimum;
			set => Set(nameof(OverrideWeightGraphMinimum), ref _overrideWeightGraphMinimum, value);
		}

		public bool OverrideWeightGraphMaximum
		{
			get => _overrideWeightGraphMaximum;
			set => Set(nameof(OverrideWeightGraphMaximum), ref _overrideWeightGraphMaximum, value);

		}

		public bool OverrideDistanceGraphMinimum
		{
			get => _overrideDistanceGraphMinimum;
			set => Set(nameof(OverrideDistanceGraphMinimum), ref _overrideDistanceGraphMinimum, value);

		}

		public bool OverrideDistanceGraphMaximum
		{
			get => _overrideDistanceGraphMinimum;
			set => Set(nameof(OverrideDistanceGraphMaximum), ref _overrideDistanceGraphMinimum, value);
		}

		public double WeightGraphMinimum
		{
			get => _weightGraphMinimum;
			set => Set(nameof(WeightGraphMinimum), ref _weightGraphMinimum, value);
		}

		public double WeightGraphMaximum
		{
			get => _weightGraphMaximum;
			set => Set(nameof(WeightGraphMaximum), ref _weightGraphMaximum, value);
		}

		public double DistanceGraphMinimum
		{
			get => _distanceGraphMinimum;
			set => Set(nameof(DistanceGraphMinimum), ref _distanceGraphMinimum, value);
		}

		public double DistanceGraphMaximum
		{
			get => _distanceGraphMaximum;
			set => Set(nameof(DistanceGraphMaximum), ref _distanceGraphMaximum, value);
		}
	}
}
