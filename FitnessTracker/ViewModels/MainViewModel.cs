using System.Threading.Tasks;
using FitnessTracker.Messages;
using FitnessTracker.Services.Interfaces;
using FitnessTracker.Utilities;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace FitnessTracker.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
		private readonly IDataImporterService _importerService;
		private readonly IFileDialogService _fileDialogService;
		private readonly ISettingsService _settingsService;

		private const string IMPORT_FILE_FILTER = "Comma-Separated Files|*.csv";

		private bool _canShowGraphs;

		public MainViewModel(IDataImporterService importerService, IFileDialogService fileDialogService, ISettingsService settingsService)
		{
			Guard.AgainstNull(importerService, nameof(importerService));
			Guard.AgainstNull(fileDialogService, nameof(fileDialogService));
			Guard.AgainstNull(settingsService, nameof(settingsService));

			_importerService = importerService;
			_fileDialogService = fileDialogService;
			_settingsService = settingsService;

			MessengerInstance.Register<NotifyDataExistsMessage>(this, msg => CanShowGraphs = msg.Content);

			ImportCommand = new RelayCommand(async () => await ImportAsync());
		}

		public bool CanShowGraphs
		{
			get => _canShowGraphs;
			set => Set(nameof(CanShowGraphs), ref _canShowGraphs, value);
		}

		public RelayCommand ImportCommand { get; }

		private async Task ImportAsync()
		{
			var filePath = _fileDialogService.OpenFileDialog(IMPORT_FILE_FILTER);
			if (!string.IsNullOrEmpty(filePath))
			{
				await _importerService.ImportData("importdata.csv");
				MessengerInstance.Send(new NewDataAvailableMessage());
			}
		}
	}
}
