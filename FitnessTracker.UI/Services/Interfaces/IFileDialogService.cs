using FitnessTracker.Core;

namespace FitnessTracker.UI.Services.Interfaces
{
	[DependencyInjectionType(DependencyInjectionType.Interface)]
	public interface IFileDialogService
	{
		public string OpenFileDialog(string fileTypeFilter);
	}
}
