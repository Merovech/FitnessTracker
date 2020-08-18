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

		private const string IMPORT_FILE_FILTER = "Comma-Separated Files|*.csv";

		public MainViewModel(IDataImporterService importerService, IFileDialogService fileDialogService)
		{
			Guard.AgainstNull(importerService, nameof(importerService));
			Guard.AgainstNull(fileDialogService, nameof(fileDialogService));

			_importerService = importerService;
			_fileDialogService = fileDialogService;

			ImportCommand = new RelayCommand(async () => await ImportAsync());
		}

		public RelayCommand ImportCommand { get; }
		public RelayCommand ExitCommand { get; }

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
