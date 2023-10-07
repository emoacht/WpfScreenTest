using System.Windows;

namespace WpfScreen.Net48;

public partial class App : Application
{
	protected override void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);

		MainWindow = new MainWindow(isDpiAwarenessSpecified: true) { Title = "Main Window .NET Framework 4.8" };
		MainWindow.Show();
	}
}
