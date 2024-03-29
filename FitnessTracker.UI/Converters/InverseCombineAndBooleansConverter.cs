﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace FitnessTracker.UI.Converters
{
	public class InverseCombineAndBooleansConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length > 0)
			{
				foreach (var v in values)
				{
					if (v is bool boolVal && boolVal)
					{
						return false;
					}
				}
			}

			return true;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
