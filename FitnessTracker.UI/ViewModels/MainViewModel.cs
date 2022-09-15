using FitnessTracker.Core;
using FitnessTracker.Core.Services.Interfaces;
using FitnessTracker.UI.Messages;
using FitnessTracker.Utilities;
using GalaSoft.MvvmLight;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;

namespace FitnessTracker.UI.ViewModels
{
	[DependencyInjectionType(DependencyInjectionType.Other)]
	public class MainViewModel : ViewModelBase
	{
		private readonly ISettingsService _settingsService;
		private readonly ILogger<MainViewModel> _logger;
		private bool _isDarkTheme;
		private bool _canShowGraphs;

		public MainViewModel(ISettingsService settingsService, ILogger<MainViewModel> logger)
		{
			Guard.AgainstNull(settingsService, nameof(settingsService));
			_settingsService = settingsService;

			Guard.AgainstNull(logger, nameof(logger));
			_logger = logger;

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
			// IsDarkTheme is set by the UI, so whatever it is now is what it's getting set to, not
			// what it's original value was.
			if (IsDarkTheme)
			{
				_logger.LogDebug("Swapping theme from light to dark.");
			}
			else
			{
				_logger.LogDebug("Swapping theme from dark to light.");
			}

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
