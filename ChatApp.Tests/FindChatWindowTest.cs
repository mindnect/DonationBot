using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ChatAppLib.Parsers;
using ChatAppLib.Win32;
using mshtml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SHDocVw;

namespace ChatApp.Tests
{
    [TestClass]
    public class FindWindowTest
    {
        [TestMethod]
        public void Find_Chat_Window_Test()
        {
            var tmp = new KakaoParser();
            var dic = tmp.FindAllPlayer();
            foreach (var c in dic)
            {
                Console.WriteLine(c.Key + "," + c.Value);
            }
        }
    }
}