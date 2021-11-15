using System.Threading.Tasks;
using FitnessTracker.Messages;
using FitnessTracker.Services.Interfaces;
using FitnessTracker.Utilities;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;

namespace FitnessTracker.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
		private readonly IDataImporterService _importerService;
		private readonly IFileDialogService _fileDialogService;
		private readonly ISettingsService _settingsService;
		private bool _isDarkTheme;

		private const string IMPORT_FILE_FILTER = "Comma-Separated Files|*.csv|FitnessTracker Files|*.dat";

		private bool _canShowGraphs;

		public MainViewModel(IDataImporterService importerService, IFileDialogService fileDialogService, ISettingsService settingsService)
		{
			Guard.AgainstNull(importerService, nameof(importerService));
			Guard.AgainstNull(fileDialogService, nameof(fileDialogService));
			Guard.AgainstNull(settingsService, nameof(settingsService));

			_importerService = importerService;
			_fileDialogService = fileDialogService;
			_settingsService = settingsService;

			// Settings is tiny and there's no reason to hang on to it for one value, which could destablize any
			// controls that *actually* need to hang on to the settings object.
			var settings = _settingsService.ReadSettings();
			IsDarkTheme = settings.IsDarkTheme;

			MessengerInstance.Register<NotifyDataExistsMessage>(this, msg => CanShowGraphs = msg.Content);

			ImportCommand = new RelayCommand(async () => await ImportAsync());
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

		public RelayCommand ImportCommand { get; }

		private async Task ImportAsync()
		{
			var filePath = _fileDialogService.OpenFileDialog(IMPORT_FILE_FILTER);
			if (!string.IsNullOrEmpty(filePath))
			{
				await _importerService.ImportData(filePath);
				MessengerInstance.Send(new NewDataAvailableMessage());
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
