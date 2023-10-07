using System.Windows;

namespace WpfScreen.Net7;

public partial class App : Application
{
	protected override void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);

		MainWindow = new MainWindow(isDpiAwarenessSpecified: true) { Title = "Main Window .NET 7.0" };
		MainWindow.Show();
	}
}
