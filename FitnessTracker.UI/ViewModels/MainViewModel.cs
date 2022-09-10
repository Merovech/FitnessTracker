using FitnessTracker.Core;
using FitnessTracker.Core.Services.Interfaces;
using FitnessTracker.UI.Messages;
using FitnessTracker.Utilities;
using GalaSoft.MvvmLight;
using MaterialDesignThemes.Wpf;

namespace FitnessTracker.UI.ViewModels
{
	[DependencyInjectionType(DependencyInjectionType.Other)]
	public class MainViewModel : ViewModelBase
	{
		private readonly ISettingsService _settingsService;
		private bool _isDarkTheme;

		private bool _canShowGraphs;

		public MainViewModel(ISettingsService settingsService)
		{
			Guard.AgainstNull(settingsService, nameof(settingsService));
			_settingsService = settingsService;

			// Settings is tiny and there's no reason to hang on to it for one value, which could destablize any
			// controls that *actually* need to hang on to the settings object.
			var settings = _settingsService.ReadSettings();
			IsDarkTheme = settings.IsDarkTheme;

			MessengerInstance.Register<NotifyDataExistsMessage>(this, msg => CanShowGraphs = msg.Content);
		}

		public bool CanShowGraphs
		{
			get => _canShowGraphs;
			set => Set(nameof(CanShowGraphs), ref _canShowGraphs, value);
		}

		public bool IsDarkTheme
		{

			get => _isDarkTheme;
			set
			{
				Set(nameof(IsDarkTheme), ref _isDarkTheme, value);
				SwapTheme();
			}
		}

		private void SwapTheme()
		{
			// Switch themes
			var themeState = IsDarkTheme ? Theme.Dark : Theme.Light;
			var paletteHelper = new PaletteHelper();
			var theme = paletteHelper.GetTheme();
			theme.SetBaseTheme(themeState);
			paletteHelper.SetTheme(theme);

			// Save the setting so it persists across sessions
			var settings = _settingsService.ReadSettings();
			settings.IsDarkTheme = IsDarkTheme;
			_settingsService.SaveSettings(settings);
		}
	}
}
