using System;
using System.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;

namespace FitnessTracker.UI.Views
{
	// All of this, just so I can format "100" as "100.0" and to get rid of the percentage value that's in the default.
	// Code taken from lvcharts's website.  I could do this in true MVVM, but there's really no need -- it's all display
	// code, so code-behind makes more sense than creating an entire view model.
	//
	// Guess I'll update the look for it in XAML while I'm at it.
	public partial class CustomTooltip : IChartTooltip
	{
		private TooltipData _data;

		public CustomTooltip()
		{
			InitializeComponent();

			DataContext = this;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public TooltipData Data
		{
			get => _data;
			set
			{
				_data = value;
				OnPropertyChanged(nameof(Data));
				OnPropertyChanged(nameof(Date));
			}
		}

		public string Date
		{
			get => _data != null ? new DateTime((long)_data.SharedValue).ToString("D") : DateTime.Now.ToString("D");
		}

		public TooltipSelectionMode? SelectionMode { get; set; }

		protected virtual void OnPropertyChanged(string propName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}
	}
}
