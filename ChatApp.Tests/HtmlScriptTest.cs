using System.Diagnostics;
using ChatApp.Server.Parsers;
using mshtml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SHDocVw;

namespace ChatApp.Tests
{
    [TestClass]
    public class HtmlScriptTest
    {
        [TestMethod]
        public void HtmlScriptTest1()
        {
            var parser = new KakaoParser();
            parser.Init();
        }
    }
}