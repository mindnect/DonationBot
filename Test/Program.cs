using System;
using System.Diagnostics;

namespace Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var shellWindows = new SHDocVw.ShellWindows();
            foreach (SHDocVw.WebBrowser wb in shellWindows)
                    Console.WriteLine(wb);

            Console.ReadKey();
        }

        private static SHDocVw.WebBrowser FindIE(string url)
        {
            var uri = new Uri(url);
            
            var shellWindows = new SHDocVw.ShellWindows();
            foreach (SHDocVw.WebBrowser wb in shellWindows)
                if (!string.IsNullOrEmpty(wb.LocationURL))
                {
                    var wbUri = new Uri(wb.LocationURL);
                    Debug.WriteLine(wbUri);
                    if (wbUri.Equals(uri))
                        return wb;
                }
            return null;
        }
    }
}