using System.Windows;

namespace WpfScreen.NotSpecified;

public partial class App : Application
{
	protected override void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);

		MainWindow = new MainWindow(isDpiAwarenessSpecified: false) { Title = "Main Window Not Specified" };
		MainWindow.Show();
	}
}
