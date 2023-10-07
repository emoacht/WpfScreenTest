using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace WpfScreen;

internal class WindowHelper
{
	public static void SetWindowRect(Window window, System.Drawing.Rectangle rect)
	{
		SetWindowRect(window, rect.X, rect.Y, rect.Width, rect.Height);
	}

	public static void SetWindowRect(Window window, int x, int y, int width, int height)
	{
		IntPtr windowHandle = new WindowInteropHelper(window).EnsureHandle();
		GetWindowPlacement(windowHandle, out WINDOWPLACEMENT windowPlacement);

		Debug.WriteLine($"SetWindowRect {x} {y} {width} {height}");

		int left = x;
		int top = y;
		int right = x + width;
		int bottom = y + height;

		if (window.WindowStyle != WindowStyle.None)
		{
			IntPtr monitorHandle = MonitorFromPoint(new POINT(x, y), MONITOR_DEFAULTTO.MONITOR_DEFAULTTONULL);
			if (monitorHandle != IntPtr.Zero)
			{
				if (TryGetMonitorDpi(monitorHandle, out DpiScale dpi))
				{
					const double DefaultExtendedFrameBoundsThickness = 7D;
					int extendedFrameBoundsThickness = (int)(DefaultExtendedFrameBoundsThickness * dpi.PixelsPerDip);

					left -= extendedFrameBoundsThickness;
					right += extendedFrameBoundsThickness;
					bottom += extendedFrameBoundsThickness;
				}
			}
		}

		windowPlacement.rcNormalPosition = new RECT(left, top, right, bottom);
		SetWindowPlacement(windowHandle, ref windowPlacement);
	}

	private static bool TryGetMonitorDpi(IntPtr monitorHandle, out DpiScale dpi)
	{
		const double DefaultPixelsPerInch = 96D;

		if (GetDpiForMonitor(monitorHandle, MONITOR_DPI_TYPE.MDT_Default, out uint dpiX, out uint dpiY) == S_OK)
		{
			dpi = new DpiScale(dpiX / DefaultPixelsPerInch, dpiY / DefaultPixelsPerInch);
			return true;
		}
		dpi = default;
		return false;
	}

	[DllImport("User32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool GetWindowRect(
		IntPtr hWnd,
		out RECT lpRect);

	[DllImport("User32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool GetWindowPlacement(
		IntPtr hWnd,
		out WINDOWPLACEMENT lpwndpl);

	[DllImport("User32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool SetWindowPlacement(
		IntPtr hWnd,
		[In] ref WINDOWPLACEMENT lpwndpl);

	[StructLayout(LayoutKind.Sequential)]
	private struct WINDOWPLACEMENT
	{
		public uint length;
		public uint flags;
		public uint showCmd;
		public POINT ptMinPosition;
		public POINT ptMaxPosition;
		public RECT rcNormalPosition;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct POINT
	{
		public int x;
		public int y;

		public POINT(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct RECT
	{
		public int left;
		public int top;
		public int right;
		public int bottom;

		public RECT(int left, int top, int right, int bottom)
		{
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
		}
	}

	[DllImport("User32.dll")]
	private static extern IntPtr MonitorFromPoint(
		POINT pt,
		MONITOR_DEFAULTTO dwFlags);

	private enum MONITOR_DEFAULTTO : uint
	{
		MONITOR_DEFAULTTONULL = 0x00000000,
		MONITOR_DEFAULTTOPRIMARY = 0x00000001,
		MONITOR_DEFAULTTONEAREST = 0x00000002,
	}

	[DllImport("Shcore.dll", SetLastError = true)]
	private static extern int GetDpiForMonitor(
		IntPtr hmonitor,
		MONITOR_DPI_TYPE dpiType,
		out uint dpiX,
		out uint dpiY);

	private enum MONITOR_DPI_TYPE
	{
		MDT_Effective_DPI = 0,
		MDT_Angular_DPI = 1,
		MDT_Raw_DPI = 2,
		MDT_Default = MDT_Effective_DPI
	}

	private const int S_OK = 0x0;
}