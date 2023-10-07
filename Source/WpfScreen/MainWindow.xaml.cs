using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace WpfScreen;

public partial class MainWindow : Window
{
	private bool IsDpiAwarenessSpecified { get; }

	public MainWindow(bool isDpiAwarenessSpecified)
	{
		InitializeComponent();
		this.IsDpiAwarenessSpecified = isDpiAwarenessSpecified;
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		foreach (var oldWindow in Application.Current.Windows.OfType<SubWindow>())
			oldWindow.Close();

		var otherScreen = ScreenHelper.GetOtherScreen(this);
		if (otherScreen is null)
			return;

		Debug.WriteLine($"Show {otherScreen.WorkingArea.Left} {otherScreen.WorkingArea.Top} {otherScreen.WorkingArea.Width} {otherScreen.WorkingArea.Height}");

		var newWindow = new SubWindow { Owner = this };
		WindowHelper.SetWindowRect(newWindow, otherScreen.WorkingArea); // 1st call
		if (IsDpiAwarenessSpecified)
		{
			newWindow.Loaded += OnLoaded;
		}
		newWindow.Show();

		void OnLoaded(object sender, RoutedEventArgs e)
		{
			newWindow.Loaded -= OnLoaded;
			WindowHelper.SetWindowRect(newWindow, otherScreen.WorkingArea); // 2nd call
		}
	}
}
