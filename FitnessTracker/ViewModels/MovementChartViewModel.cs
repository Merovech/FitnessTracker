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
	public class MovementChartViewModel : ViewModelBase
	{
		private readonly ISettingsService _settingsService;
		private LineSeries _currentDistanceSeries;
		private LineSeries _averageDistanceSeries;
		private SystemSettings _systemSettings;

		public MovementChartViewModel(ISettingsService settingsService)
		{
			Guard.AgainstNull(settingsService, nameof(settingsService));
			_settingsService = settingsService;

			InitializeWeightSeries();

			MessengerInstance.Register<DataRetrievedMessage>(this, msg => UpdateData(msg.Content.ToList()));
			MessengerInstance.Register<SystemSettingsChangedMessage>(this, msg => SystemSettings = msg.Content);

			// Due to weirdness in how the views are initially instantiated, we need to retrieve the settings once here.
			// Otherwise, the settings view model seems to instantiate, retrieve settings, and send out its message before
			// this view model exists to retrieve the message.
			SystemSettings = _settingsService.ReadSettings();
		}

		public bool CanShowGraphs => _currentDistanceSeries.Values.Count > 0 && _averageDistanceSeries.Values.Count > 0;

		public SystemSettings SystemSettings
		{
			get => _systemSettings ?? new SystemSettings();
			set => Set(nameof(SystemSettings), ref _systemSettings, value);
		}

		public SeriesCollection SeriesData { get; private set; }

		public Func<double, string> Formatter { get; set; }

		private void UpdateData(List<DailyRecord> data)
		{
			_currentDistanceSeries.Values.Clear();
			_averageDistanceSeries.Values.Clear();

			for (int i = 0; i < data.Count; i++)
			{
				_currentDistanceSeries.Values.Add(new DateSeriesValue { DateTime = data[i].Date, Value = data[i].DistanceMoved });
				_averageDistanceSeries.Values.Add(new DateSeriesValue { DateTime = data[i].Date, Value = data[i].AverageDistanceMoved});
			}

			RaisePropertyChanged(nameof(CanShowGraphs));
		}

		private void InitializeWeightSeries()
		{
			_currentDistanceSeries = new LineSeries
			{
				Title = "Daily",
				Values = new ChartValues<DateSeriesValue>(),
				PointGeometry = DefaultGeometries.Circle,
				PointGeometrySize = 5,
				LineSmoothness = 0,
				PointForeground = Brushes.CornflowerBlue,
				Fill = Brushes.Transparent
			};

			_averageDistanceSeries = new LineSeries
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

			SeriesData = new SeriesCollection(config) { _currentDistanceSeries, _averageDistanceSeries };
			Formatter = value => new DateTime((long)(value * TimeSpan.FromDays(1).Ticks)).ToString("d");
		}
	}
}
