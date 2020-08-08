using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using FitnessTracker.Messages;
using FitnessTracker.Models;
using GalaSoft.MvvmLight;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;

namespace FitnessTracker.ViewModels
{
	public class WeightChartViewModel : ViewModelBase
	{
		private LineSeries _currentWeightSeries;
		private LineSeries _averageWeightSeries;

		public WeightChartViewModel()
		{
			InitializeWeightSeries();

			MessengerInstance.Register<DataRetrievedMessage>(this, msg => UpdateData(msg.Content.ToList()));
		}

		public SeriesCollection SeriesData { get; private set; }

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
				PointGeometrySize = 6,
				LineSmoothness = 0,
				PointForeground = Brushes.Blue,
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
