using System;
using System.Linq;
using System.Windows;
using System.Windows.Interop;

namespace WpfScreen;

public class ScreenHelper
{
	private static int _index = 0;

	public static System.Windows.Forms.Screen GetOtherScreen(Window window)
	{
		IntPtr windowHandle = new WindowInteropHelper(window).Handle;
		System.Windows.Forms.Screen[] allScreens = System.Windows.Forms.Screen.AllScreens;

		if (WindowHelper.GetWindowRect(windowHandle, out WindowHelper.RECT rect))
		{
			System.Drawing.Point windowLocation = new System.Drawing.Point(rect.left, rect.top);
			System.Windows.Forms.Screen[] otherScreens = allScreens.Where(x => !x.Bounds.Contains(windowLocation)).ToArray();

			if (otherScreens.Length > 0)
			{
				int index = _index++ % otherScreens.Length;
				return otherScreens[index];
			}
		}
		return null;
	}
}