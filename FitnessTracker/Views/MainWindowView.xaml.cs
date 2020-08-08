using System.Windows;

namespace FitnessTracker.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindowView : Window
	{
		public MainWindowView()
		{
			InitializeComponent();
		}

		private void OnExit(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}
