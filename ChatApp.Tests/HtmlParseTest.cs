using System;
using mshtml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChatApp.Tests
{
    [TestClass]
    public class HtmlParseTest
    {
        [TestMethod]
        public void HtmlParseTest1()
        {
            var strExit = "<div parsed=\"true\"><div class=\"txt_notice area_space\"><span class=\"screen_out\" style=\"text-indent: 0px;\">알림</span>TachyBrid(bangs****)님이 퇴장하셨습니다.</div>\n\n</div>";
            var strChat = "<div parsed=\"true\"><div class=\"area_chat\"><strong class=\"tit_name\">라링 (seop****)<span class=\"txt_time\">21:17</span>\n</strong><div class=\"info_words\" style=\"color: rgb(51, 51, 51); font-family: 맑은 고딕; font-size: 12px; font-weight: normal;\">헤이콕 끼고 하는팀이라니</div>\n</div>\n\n</div>";


            var doc = new HTMLDocument();
            ((IHTMLDocument2)doc).write(strChat);
            var t =  doc.body.document.firstChild;

        }
    }
}