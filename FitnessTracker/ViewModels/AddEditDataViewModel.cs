using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;

namespace FitnessTracker.ViewModels
{
	public class AddEditDataViewModel : ViewModelBase
	{
		private bool _isNewDataPoint;

		public AddEditDataViewModel()
		{
			_isNewDataPoint = false;
		}

		public bool IsNewDataPoint
		{
			get => _isNewDataPoint;
			set => Set(nameof(IsNewDataPoint), ref _isNewDataPoint, value);
		}
	}
}
