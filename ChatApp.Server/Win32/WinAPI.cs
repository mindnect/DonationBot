using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using mshtml;

namespace ChatApp.Server.Win32
{
    public class WinAPI
    {
        #region Delegates

        public delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

        #endregion

        // ReSharper disable once InconsistentNaming
        private const int SMTO_ABORTIFHUNG = 0x2;
        // ReSharper disable once InconsistentNaming
        private static Guid IID_IHTMLDocument = new Guid("626FC520-A41E-11CF-A731-00A0C9082637");

        public static List<IntPtr> GetChildsFromParent(IntPtr hwnd)
        {
            var childHandles = new List<IntPtr>();

            var gcChildhandlesList = GCHandle.Alloc(childHandles);
            var pointerChildHandlesList = GCHandle.ToIntPtr(gcChildhandlesList);

            try
            {
                var childProc = new EnumWindowProc(EnumWindow);
                EnumChildWindows(hwnd, childProc, pointerChildHandlesList);
            }
            finally
            {
                gcChildhandlesList.Free();
            }

            return childHandles;
        }

        public static Dictionary<IntPtr, string> FindAllWindowsWithClassNames(params string[] classNames)
        {
            var ret = new Dictionary<IntPtr, string>();
            var hwnd = IntPtr.Zero;
            foreach (var clasName in classNames)
                do
                {
                    hwnd = FindWindowEx(IntPtr.Zero, hwnd, clasName, null);
                    if (hwnd == IntPtr.Zero) break;
                    var stringBuilder = new StringBuilder(128);
                    GetWindowText(hwnd, stringBuilder, 128);
                    ret.Add(hwnd, stringBuilder.ToString());
                } while (hwnd != IntPtr.Zero);
            return ret;
        }

        public static Dictionary<IntPtr, string> FindAllWindowsWithCaption(string caption)
        {
            var ret = new Dictionary<IntPtr, string>();
            var hwnd = IntPtr.Zero;
            do
            {
                hwnd = FindWindowEx(IntPtr.Zero, hwnd, null, caption);
                if (hwnd == IntPtr.Zero) break;
                var stringBuilder = new StringBuilder(128);
                GetWindowText(hwnd, stringBuilder, 128);
                ret.Add(hwnd, stringBuilder.ToString());
            } while (hwnd != IntPtr.Zero);
            return ret;
        }

        public static string GetCpationOfWnd(IntPtr hwnd)
        {
            var caption = "";
            try
            {
                var stringBuilder = new StringBuilder("", 128);
                if (GetWindowText(hwnd, stringBuilder, stringBuilder.Capacity) != 0)
                    caption = stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                caption = ex.Message;
            }
            return caption;
        }


        public static string GetClassNameWnd(IntPtr hwnd)
        {
            var className = "";
            try
            {
                var stringBuilder = new StringBuilder("", 128);
                if (GetClassName(hwnd, stringBuilder, stringBuilder.Capacity) != IntPtr.Zero)
                    className = stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                className = ex.Message;
            }
            return className;
        }

        public static List<IntPtr> GetChildsWithClassName(IntPtr parent, string className)
        {
            var ret = new List<IntPtr>();
            foreach (var childWindow in GetChildsFromParent(parent))
                if (GetClassNameWnd(childWindow) == className)
                    ret.Add(childWindow);
            return ret;
        }


        public static HTMLDocument GetHtmlDocumentOfWnd(IntPtr hWnd)
        {
            HTMLDocument document = null;
            var iMsg = 0;
            var iRes = 0;

            iMsg = RegisterWindowMessage("WM_HTML_GETOBJECT");

            if (iMsg != 0)
            {
                SendMessageTimeout(hWnd, iMsg, 0, 0, SMTO_ABORTIFHUNG, 1000, ref iRes);
                if (iRes != 0)
                    ObjectFromLresult(iRes, ref IID_IHTMLDocument, 0, ref document);
            }
            return document;
        }

        public static int GetWndInstance(IntPtr hwnd)
        {
            return GetWindowLong(hwnd, -6);
        }


        [DllImport("user32.Dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr parentHandle, EnumWindowProc callback, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll", EntryPoint = "RegisterWindowMessageA")]
        private static extern int RegisterWindowMessage(string lpString);

        [DllImport("user32.dll", EntryPoint = "SendMessageTimeoutA")]
        private static extern int SendMessageTimeout(IntPtr hwnd, int msg, int wParam, int lParam, int fuFlags, int uTimeout, ref int lpdwResult);

        [DllImport("OLEACC.dll")]
        private static extern int ObjectFromLresult(int lResult, ref Guid riid, int wParam, ref HTMLDocument ppvObject);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private static bool EnumWindow(IntPtr hWnd, IntPtr lParam)
        {
            var gcChildhandlesList = GCHandle.FromIntPtr(lParam);
            if (gcChildhandlesList.Target == null) return false;
            var childHandles = gcChildhandlesList.Target as List<IntPtr>;
            childHandles.Add(hWnd);
            return true;
        }
    }
}