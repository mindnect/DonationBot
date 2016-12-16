using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace ChatTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            TestParse();
            Console.ReadKey();
        }


        private static void TestParse()
        {
            var tests = new Dictionary<string, string>();
            //tests.Add("테스트 입장", @"<div class=""txt_notice""><span class=""screen_out"" style=""text-indent: 0px;"">알림</span>알콜V(mind****)님이 입장하셨습니다.</div>");
            //tests.Add("퇴장", "");
            //tests.Add("매니저 임명", "");
            //tests.Add("매니저 해임", "");
            //tests.Add("닉네임 변경", "");
            tests.Add("테스트 후원", @"<div class=""txt_notice area_space box_sticker""><span class=""screen_out"" style=""text-indent: 0px;"">후원금액</span><span class=""area_sticker type_01""><img class=""img_sticker"" alt="""" src=""http://t1.daumcdn.net/tvpot/service/2015/emoticon/sticker_01.png""><span class=""txt_money txt_type01"">1,000</span></span><p class=""txt_message box_type01""><span class=""ico_label ico_tail01""></span>테스트 후원!</p><p class=""txt_spon"">라임보이님이 후원하셨습니다.</p></div>");
            //tests.Add("귀속말", "");
            //tests.Add("채팅", "");
            //tests.Add("AD채팅", "");

            foreach (var test in tests)
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(test.Value);
                Parse(htmlDoc);
            }
        }

        private static void Parse(HtmlDocument doc)
        {
            var node = doc.DocumentNode.FirstChild;
            var attrStr = node.Attributes["class"].Value;

            if (attrStr.Contains("txt_notice"))
                if (attrStr.Equals("txt_notice area_space box_sticker"))
                {
                    var amount = node.SelectSingleNode("span[@class='area_sticker type_01']").InnerText.Replace(",", "");
                    var message = node.SelectSingleNode("p[@class='txt_message box_type01']").InnerText.Trim();
                    var nickname = node.SelectSingleNode("p[@class='txt_spon']").InnerText.Split('님')[0];
                    Console.WriteLine(nickname + "\t" + message + "\t" + amount);
                }

                else
                {
                    var msg = node.SelectSingleNode("text()").InnerText;
                    Console.WriteLine(msg);
                }


            else if (attrStr.Contains("area_chat"))
                if (attrStr.Equals("area_chat area_space"))
                {
                }
                else
                {
                    var nickname = node.SelectSingleNode("strong/text()").InnerText.Split()[0];
                    var rank = node.SelectSingleNode("strong/span").InnerText;
                    var message = node.SelectSingleNode("div").InnerText.Trim();

                    Console.WriteLine(rank + "\t" + nickname + "\t" + message + "\t");
                }
        }
    }
}