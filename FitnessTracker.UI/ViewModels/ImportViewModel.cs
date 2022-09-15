using System.IO;
using System.Threading.Tasks;
using FitnessTracker.Core;
using FitnessTracker.Core.Services.Interfaces;
using FitnessTracker.UI.Messages;
using FitnessTracker.UI.Services.Interfaces;
using FitnessTracker.Utilities;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;

namespace FitnessTracker.UI.ViewModels
{
	[DependencyInjectionType(DependencyInjectionType.Other)]
	public class ImportViewModel : ViewModelBase
	{
		private const string IMPORT_FILE_FILTER = "Comma-Separated Files|*.csv|FitnessTracker Data Files|*.ft|FitnessTracker Legacy Data Files|*.dat";

		private readonly IDataImporterService _dataImporterService;
		private readonly IFileDialogService _fileDialogService;
		private readonly ILogger<ImportViewModel> _logger;
		private string _fileName;
		private string _statusMessage;
		private bool _isImporting;

		public ImportViewModel(IDataImporterService dataImporterService, IFileDialogService fileDialogService, ILogger<ImportViewModel> logger)
		{
			Guard.AgainstNull(dataImporterService, nameof(dataImporterService));
			_dataImporterService = dataImporterService;

			Guard.AgainstNull(fileDialogService, nameof(fileDialogService));
			_fileDialogService = fileDialogService;

			Guard.AgainstNull(logger, nameof(logger));
			_logger = logger;

			ImportCommand = new RelayCommand(async () => await ImportData(), () => !string.IsNullOrEmpty(FileName) && !IsImporting);
			SelectFileCommand = new RelayCommand(() => OpenFileDialog(), () => !IsImporting);
			CloseDialogCommand = new RelayCommand(() => CloseDialog(), () => !IsImporting);

			StatusMessage = "Ready.";
			FileType = GetFileType();
		}

		public RelayCommand ImportCommand { get; }

		public RelayCommand SelectFileCommand { get; }

		public RelayCommand CloseDialogCommand { get; }

		public string FileType { get; set; }

		public  bool IsImporting
		{
			get => _isImporting;
			set
			{
				Set(nameof(IsImporting), ref _isImporting, value);
				RaisePropertyChanged(nameof(IsImporting));
				RaiseCanExecuteChangedEvents();
			}
		}

		public string FileName
		{
			get => _fileName;
			set
			{
				Set(nameof(FileName), ref _fileName, value);
				RaisePropertyChanged(nameof(FileName));
				FileType = GetFileType();
				RaisePropertyChanged(nameof(FileType));
				_logger.LogTrace("File name for import: {file} (Type={type})", value, FileType);
			}
		}

		public string StatusMessage
		{
			get => _statusMessage;
			set
			{
				Set(nameof(StatusMessage), ref _statusMessage, value);
				RaisePropertyChanged(nameof(StatusMessage));
			}
		}

		private void OpenFileDialog()
		{
			FileName = _fileDialogService.OpenFileDialog(IMPORT_FILE_FILTER).Trim();
			RaiseCanExecuteChangedEvents();
		}

		private async Task ImportData()
		{
			IsImporting = true;
			StatusMessage = "Importing records...";
			var recordCount = await _dataImporterService.ImportData(FileName);
			StatusMessage = $"Complete.  Records imported: {recordCount}.";
			_logger.LogDebug("Imported {count} records from file {file}", recordCount, FileName);
			IsImporting = false;

			MessengerInstance.Send(new NewDataAvailableMessage());
		}

		private void CloseDialog()
		{
			FileName = string.Empty;
			StatusMessage = "Ready.";
			DialogHost.CloseDialogCommand.Execute(null, null);
		}

		private string GetFileType()
		{
			if (string.IsNullOrEmpty(FileName))
			{
				return "(no file selected)";
			}

			return Path.GetExtension(FileName) switch
			{
				".csv" => "Comma-Separated Value",
				".dat" => "FitnessTracker Legacy Data",
				".ft" => "FitnessTracker Data",
				_ => "Unknown",
			};
		}

		private void RaiseCanExecuteChangedEvents()
		{
			ImportCommand.RaiseCanExecuteChanged();
			CloseDialogCommand.RaiseCanExecuteChanged();
			SelectFileCommand.RaiseCanExecuteChanged();
		}
	}
}
