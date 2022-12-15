using FitnessTracker.Core;
using FitnessTracker.UI.Services.Interfaces;
using Microsoft.Win32;

namespace FitnessTracker.UI.Services.Implementations
{
	[DependencyInjectionType(DependencyInjectionType.Service)]
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
