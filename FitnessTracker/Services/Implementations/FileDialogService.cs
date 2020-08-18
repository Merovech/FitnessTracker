using System;
using System.Collections.Generic;
using System.Text;
using FitnessTracker.Services.Interfaces;
using Microsoft.Win32;

namespace FitnessTracker.Services.Implementations
{
	public class FileDialogService : IFileDialogService
	{
		public string OpenFileDialog(string fileTypeFilter)
		{
			var dlg = new OpenFileDialog
			{
				Filter = fileTypeFilter,
				Multiselect = false
			};

			if (dlg.ShowDialog() == true)
			{
				return dlg.FileName;
			}

			return string.Empty;
		}
	}
}
