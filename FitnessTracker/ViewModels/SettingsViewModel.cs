using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using FitnessTracker.Messages;
using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using FitnessTracker.Utilities;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;

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

			SaveCommand = new RelayCommand(() => SaveSettingsAndExit(), () => ErrorMessages != null && !ErrorMessages.Any());
			ErrorMessages = new ObservableCollection<string>();

			GetSettings();
		}

		public RelayCommand SaveCommand { get; }

		public SystemSettings SystemSettings
		{
			get => _systemSettings ?? new SystemSettings();
			set
			{
				Set(nameof(SystemSettings), ref _systemSettings, value);
				PopulateSettings();
			}
		}

		public bool OverrideWeightGraphMinimum
		{
			get => _overrideWeightGraphMinimum;
			set
			{
				Set(nameof(OverrideWeightGraphMinimum), ref _overrideWeightGraphMinimum, value);
				ValidateForm();
			}
		}

		public bool OverrideWeightGraphMaximum
		{
			get => _overrideWeightGraphMaximum;
			set
			{
				Set(nameof(OverrideWeightGraphMaximum), ref _overrideWeightGraphMaximum, value);
				ValidateForm();
			}
		}

		public bool OverrideDistanceGraphMinimum
		{
			get => _overrideDistanceGraphMinimum;
			set
			{
				Set(nameof(OverrideDistanceGraphMinimum), ref _overrideDistanceGraphMinimum, value);
				ValidateForm();
			}
		}

		public bool OverrideDistanceGraphMaximum
		{
			get => _overrideDistanceGraphMaximum;
			set
			{
				Set(nameof(OverrideDistanceGraphMaximum), ref _overrideDistanceGraphMaximum, value);
				ValidateForm();
			}
		}

		public double WeightGraphMinimum
		{
			get => _weightGraphMinimum;
			set
			{
				Set(nameof(WeightGraphMinimum), ref _weightGraphMinimum, value);
				RaisePropertyChanged(nameof(WeightGraphMaximum));
				ValidateForm();
			}
		}

		public double WeightGraphMaximum
		{
			get => _weightGraphMaximum;
			set
			{
				Set(nameof(WeightGraphMaximum), ref _weightGraphMaximum, value);
				RaisePropertyChanged(nameof(WeightGraphMinimum));
				ValidateForm();
			}
		}

		public double DistanceGraphMinimum
		{
			get => _distanceGraphMinimum;
			set
			{
				Set(nameof(DistanceGraphMinimum), ref _distanceGraphMinimum, value);
				RaisePropertyChanged(nameof(DistanceGraphMaximum));
				ValidateForm();
			}
		}

		public double DistanceGraphMaximum
		{
			get => _distanceGraphMaximum;
			set
			{
				Set(nameof(DistanceGraphMaximum), ref _distanceGraphMaximum, value);
				RaisePropertyChanged(nameof(DistanceGraphMinimum));
				ValidateForm();
			}
		}

		public ObservableCollection<string> ErrorMessages { get; set; }

		public void ValidateForm()
		{
			ErrorMessages.Clear();

			if (OverrideWeightGraphMinimum && OverrideWeightGraphMaximum && WeightGraphMinimum >= WeightGraphMaximum)
			{
				ErrorMessages.Add("Weight Graph: Minimum must be less than maximum.");
			}

			if (OverrideDistanceGraphMinimum && OverrideDistanceGraphMaximum && DistanceGraphMinimum >= DistanceGraphMaximum)
			{
				ErrorMessages.Add("Distance Graph: Minimum must be less than maximum.");
			}

			RaisePropertyChanged(nameof(ErrorMessages));
			SaveCommand.RaiseCanExecuteChanged();
		}

		private void PopulateSettings()
		{
			OverrideWeightGraphMinimum = SystemSettings.WeightGraphMinimum.HasValue;
			OverrideWeightGraphMaximum = SystemSettings.WeightGraphMaximum.HasValue;
			OverrideDistanceGraphMinimum = SystemSettings.DistanceGraphMinimum.HasValue;
			OverrideDistanceGraphMaximum = SystemSettings.DistanceGraphMaximum.HasValue;

			WeightGraphMinimum = SystemSettings.WeightGraphMinimum.GetValueOrDefault();
			WeightGraphMaximum = SystemSettings.WeightGraphMaximum.GetValueOrDefault();
			DistanceGraphMinimum = SystemSettings.DistanceGraphMinimum.GetValueOrDefault();
			DistanceGraphMaximum = SystemSettings.DistanceGraphMaximum.GetValueOrDefault();
		}

		private void SaveSettingsAndExit()
		{
			var settings = new SystemSettings
			{
				WeightUnit = SystemSettings.WeightUnit,
				DistanceUnit = SystemSettings.DistanceUnit,
				WeightGraphMinimum = OverrideWeightGraphMinimum ? (double?)WeightGraphMinimum : null,
				WeightGraphMaximum = OverrideWeightGraphMaximum ? (double?)WeightGraphMaximum : null,
				DistanceGraphMinimum = OverrideDistanceGraphMinimum ? (double?)DistanceGraphMinimum : null,
				DistanceGraphMaximum = OverrideDistanceGraphMaximum ? (double?)DistanceGraphMaximum : null
			};

			_settingsService.SaveSettings(settings);

			// Reread from disk to ensure data integrity in the running app.
			GetSettings();

			DialogHost.CloseDialogCommand.Execute(null, null);
		}

		private void GetSettings()
		{
			SystemSettings = _settingsService.ReadSettings();
			MessengerInstance.Send(new SystemSettingsChangedMessage(SystemSettings));
		}
	}
}
