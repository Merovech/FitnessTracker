using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FitnessTracker.Converters
{
	// Code taken from the below post.  I like this as a solution much better than creating a simple inverted converter using the built-in
	// BooleanToVisibilityConverter in WPF.  I don't know why Microsoft didn't just include a negated version by default.  It's a common
	// enough pattern.
	//
	// https://stackoverflow.com/questions/534575/how-do-i-invert-booleantovisibilityconverter
	public class BooleanConverter<T> : IValueConverter
	{
		public BooleanConverter(T trueValue, T falseValue)
		{
			True = trueValue;
			False = falseValue;
		}

		public T True { get; set; }

		public T False { get; set; }

		public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is bool && ((bool)value) ? True : False;
		}

		public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is T && EqualityComparer<T>.Default.Equals((T)value, True);
		}
	}

	public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility>
	{
		public BooleanToVisibilityConverter() : base(Visibility.Visible, Visibility.Collapsed)
		{
		}
	}
}