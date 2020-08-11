using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessTracker.Messages;
using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using FitnessTracker.Utilities;
using GalaSoft.MvvmLight;

namespace FitnessTracker.ViewModels
{
	public class SummaryViewModel : ViewModelBase
	{
		private readonly IDataCalculatorService _dataCalculatorService;
		private SummaryStatistics _summaryStatistics;

		public SummaryViewModel(IDataCalculatorService dataCalculatorService)
		{
			Guard.AgainstNull(dataCalculatorService, nameof(dataCalculatorService));
			_dataCalculatorService = dataCalculatorService;

			MessengerInstance.Register<DataRetrievedMessage>(this, async msg => await GetSummaryStatistics(msg.Content));
		}

		public SummaryStatistics SummaryData
		{
			get => _summaryStatistics;
			set => Set(nameof(SummaryData), ref _summaryStatistics, value);
		}

		private async Task GetSummaryStatistics(IEnumerable<DailyRecord> data)
		{
			SummaryData = await Task.Run(() => _dataCalculatorService.CalculateSummaryStatistics(data));
		}
	}
}
