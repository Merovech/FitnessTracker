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

		private string _status;

		public MainViewModel(IDataImporterService importerService)
		{
			Guard.AgainstNull(importerService, nameof(importerService));

			_importerService = importerService;
			ImportCommand = new RelayCommand(async () => await ImportAsync());

			_ = ImportAsync();
		}

		public RelayCommand ImportCommand { get; }
		public RelayCommand ExitCommand { get; }

		public string Status
		{
			get => _status;
			set => Set(ref _status, value);
		}

		private async Task ImportAsync()
		{
			Status = "Importing data...";
			await _importerService.ImportData("importdata.csv");
			MessengerInstance.Send(new NewDataAvailableMessage());

			Status = "";
		}
	}
}
