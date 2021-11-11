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

		private double _weightGraphMinimum;
		private double _weightGraphMaximum;

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
		public ObservableCollection<string> ErrorMessages { get; set; }

		public void ValidateForm()
		{
			ErrorMessages.Clear();

			if (OverrideWeightGraphMinimum && OverrideWeightGraphMaximum && WeightGraphMinimum >= WeightGraphMaximum)
			{
				ErrorMessages.Add("Weight Graph: Minimum must be less than maximum.");
			}

			RaisePropertyChanged(nameof(ErrorMessages));
			SaveCommand.RaiseCanExecuteChanged();
		}

		private void PopulateSettings()
		{
			OverrideWeightGraphMinimum = SystemSettings.WeightGraphMinimum.HasValue;
			OverrideWeightGraphMaximum = SystemSettings.WeightGraphMaximum.HasValue;

			WeightGraphMinimum = SystemSettings.WeightGraphMinimum.GetValueOrDefault();
			WeightGraphMaximum = SystemSettings.WeightGraphMaximum.GetValueOrDefault();
		}

		private void SaveSettingsAndExit()
		{
			var settings = new SystemSettings
			{
				WeightUnit = SystemSettings.WeightUnit,
				WeightGraphMinimum = OverrideWeightGraphMinimum ? WeightGraphMinimum : null,
				WeightGraphMaximum = OverrideWeightGraphMaximum ? WeightGraphMaximum : null,
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
