using MSHTML;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace JSAssist
{
	internal class Win32
	{
		public const int WM_NCLBUTTONDOWN = 161;

		public const int HT_CAPTION = 2;

		public const int SMTO_ABORTIFHUNG = 2;

		public static Guid IID_IHTMLDocument;

		static Win32()
		{
			Win32.IID_IHTMLDocument = new Guid("626FC520-A41E-11CF-A731-00A0C9082637");
		}

		public Win32()
		{
		}

		[DllImport("user32.Dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool EnumChildWindows(IntPtr parentHandle, Win32.Win32Callback callback, IntPtr lParam);

		private static bool EnumWindow(IntPtr handle, IntPtr pointer)
		{
			List<IntPtr> target = GCHandle.FromIntPtr(pointer).Target as List<IntPtr>;
			if (target == null)
			{
				throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
			}
			target.Add(handle);
			return true;
		}

		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr FindWindowEx(IntPtr hWnd1, IntPtr hWnd2, string lpsz1, string lpsz2);

		public static List<IntPtr> GetChildWindows(IntPtr parent)
		{
			List<IntPtr> intPtrs = new List<IntPtr>();
			GCHandle gCHandle = GCHandle.Alloc(intPtrs);
			try
			{
				Win32.Win32Callback win32Callback = new Win32.Win32Callback(Win32.EnumWindow);
				Win32.EnumChildWindows(parent, win32Callback, GCHandle.ToIntPtr(gCHandle));
			}
			finally
			{
				if (gCHandle.IsAllocated)
				{
					gCHandle.Free();
				}
			}
			return intPtrs;
		}

		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		public static extern IntPtr GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		public static string GetWinClass(IntPtr hwnd)
		{
			if (hwnd == IntPtr.Zero)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder(100);
			if (Win32.GetClassName(hwnd, stringBuilder, stringBuilder.Capacity) == IntPtr.Zero)
			{
				return null;
			}
			return stringBuilder.ToString();
		}

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

		[DllImport("OLEACC.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int ObjectFromLresult(int lResult, ref Guid riid, int wParam, ref IHTMLDocument2 ppvObject);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="RegisterWindowMessageA", ExactSpelling=false)]
		public static extern int RegisterWindowMessage(string lpString);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool ReleaseCapture();

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="SendMessageTimeoutA", ExactSpelling=false)]
		public static extern int SendMessageTimeout(IntPtr hwnd, int msg, int wParam, int lParam, int fuFlags, int uTimeout, out int lpdwResult);

		public enum GetWindow_Cmd : uint
		{
			GW_HWNDFIRST,
			GW_HWNDLAST,
			GW_HWNDNEXT,
			GW_HWNDPREV,
			GW_OWNER,
			GW_CHILD,
			GW_ENABLEDPOPUP
		}

		public delegate bool Win32Callback(IntPtr hwnd, IntPtr lParam);
	}
}