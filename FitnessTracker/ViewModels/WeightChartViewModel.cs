using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using FitnessTracker.Messages;
using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using FitnessTracker.Utilities;
using GalaSoft.MvvmLight;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;

namespace FitnessTracker.ViewModels
{
	public class WeightChartViewModel : ViewModelBase
	{
		private readonly ISettingsService _settingsService;

		private LineSeries _currentWeightSeries;
		private LineSeries _averageWeightSeries;
		private SystemSettings _systemSettings;

		public WeightChartViewModel(ISettingsService settingsService)
		{
			Guard.AgainstNull(settingsService, nameof(settingsService));
			_settingsService = settingsService;

			InitializeWeightSeries();

			MessengerInstance.Register<DataRetrievedMessage>(this, msg => UpdateData(msg.Content.ToList()));
			MessengerInstance.Register<SystemSettingsChangedMessage>(this, msg => SystemSettings = msg.Content);

			SystemSettings = _settingsService.ReadSettings();
		}

		public SeriesCollection SeriesData { get; private set; }

		public SystemSettings SystemSettings
		{
			get =>_systemSettings ?? new SystemSettings();
			set => Set(nameof(SystemSettings), ref _systemSettings, value);
		}

		public Func<double, string> Formatter { get; set; }

		private void UpdateData(List<DailyRecord> data)
		{
			_currentWeightSeries.Values.Clear();
			_averageWeightSeries.Values.Clear();

			for (int i = 0; i < data.Count; i++)
			{
				_currentWeightSeries.Values.Add(new DateSeriesValue { DateTime = data[i].Date, Value = data[i].Weight });
				_averageWeightSeries.Values.Add(new DateSeriesValue { DateTime = data[i].Date, Value = data[i].MovingWeightAverage });
			}
		}

		private void InitializeWeightSeries()
		{
			_currentWeightSeries = new LineSeries
			{
				Title = "Daily",
				Values = new ChartValues<DateSeriesValue>(),
				PointGeometry = DefaultGeometries.Circle,
				PointGeometrySize = 5,
				LineSmoothness = 0,
				PointForeground = Brushes.CornflowerBlue,
				Fill = Brushes.Transparent
			};

			_averageWeightSeries = new LineSeries
			{
				Title = "Average",
				Values = new ChartValues<DateSeriesValue>(),
				PointGeometry = null,
				Fill = Brushes.Transparent,
				Stroke = Brushes.Red
			};

			var config = Mappers.Xy<DateSeriesValue>()
				.X(model => (double)model.DateTime.Ticks / TimeSpan.FromDays(1).Ticks)
				.Y(model => model.Value ?? double.NaN);

			SeriesData = new SeriesCollection(config) { _currentWeightSeries, _averageWeightSeries };
			Formatter = value => new DateTime((long)(value * TimeSpan.FromDays(1).Ticks)).ToString("d");
		}
	}
}
