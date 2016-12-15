using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using mshtml;

namespace AlcoholV
{
    internal class Win32
    {
        public delegate bool Win32Callback(IntPtr hwnd, IntPtr lParam);

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

        public const int WM_NCLBUTTONDOWN = 161;

        public const int HT_CAPTION = 2;

        public const int SMTO_ABORTIFHUNG = 2;

        public static Guid IID_IHTMLDocument = new Guid("626FC520-A41E-11CF-A731-00A0C9082637");

        [DllImport("user32.Dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr parentHandle, Win32Callback callback, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll", EntryPoint = "RegisterWindowMessageA")]
        public static extern int RegisterWindowMessage(string lpString);

        [DllImport("user32.dll", EntryPoint = "SendMessageTimeoutA")]
        public static extern int SendMessageTimeout(IntPtr hwnd, int msg, int wParam, int lParam, int fuFlags, int uTimeout, out int lpdwResult);

        [DllImport("OLEACC.dll")]
        public static extern int ObjectFromLresult(int lResult, ref Guid riid, int wParam, ref IHTMLDocument2 ppvObject);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            var expr_13 = GCHandle.FromIntPtr(pointer).Target as List<IntPtr>;
            if (expr_13 == null)
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            expr_13.Add(handle);
            return true;
        }

        public static List<IntPtr> GetChildWindows(IntPtr parent)
        {
            var list = new List<IntPtr>();
            var value = GCHandle.Alloc(list);
            try
            {
                Win32Callback callback = EnumWindow;
                EnumChildWindows(parent, callback, GCHandle.ToIntPtr(value));
            }
            finally
            {
                if (value.IsAllocated)
                    value.Free();
            }
            return list;
        }

        public static string GetWinClass(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                return null;
            var stringBuilder = new StringBuilder(100);
            if (GetClassName(hwnd, stringBuilder, stringBuilder.Capacity) != IntPtr.Zero)
                return stringBuilder.ToString();
            return null;
        }
    }
}