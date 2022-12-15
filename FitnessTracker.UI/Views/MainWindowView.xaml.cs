using System.Windows;
using FitnessTracker.Core;

namespace FitnessTracker.UI.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	[DependencyInjectionType(DependencyInjectionType.Singleton)]
	public partial class MainWindowView : Window
	{
		public MainWindowView()
		{
			InitializeComponent();
		}
	}
}
