using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using LiveCharts.Wpf;

namespace FitnessTracker.UI.Converters
{
	public class SharedValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not TooltipData v) return null;
			return new DateTime((long)(v.SharedValue.GetValueOrDefault() * TimeSpan.FromDays(1).Ticks)).ToString("D");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
