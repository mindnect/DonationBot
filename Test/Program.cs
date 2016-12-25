using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Xml;
using WebSocketSharp;

namespace Test
{
    internal class Program
    {
        private static void LoopIp(string startStr, string endStr)
        {
            var s = startStr.Split('.');
            var e = endStr.Split('.');

            var start = new int[4];
            var end = new int[4];

            for (var i = 0; i < 4; i++)
            {
                start[i] = int.Parse(s[i]);
                end[i] = int.Parse(e[i]);
            }

            for (var a = start[0]; a <= end[0]; a++)
                for (var b = start[1]; b <= end[1]; b++)
                    for (var c = start[2]; c <= end[2]; c++)
                        for (var d = start[3]; d <= end[3]; d++)
                        {
                            var addr = string.Format($"{a}.{b}.{c}.{d}");
                            try
                            {
                                new Thread(() =>
                                {
                                    var p = new Ping();
                                    if (p.Send(addr).Status != IPStatus.Success) return;
                                    using (var webSocket = new WebSocket("ws://" + addr + ":4649/JSAssistChatServer") {Log = {Output = (data, t) => { }}})
                                    {
                                        webSocket.OnOpen += (sender, eventArgs) => { Console.WriteLine(addr); };
                                        new Thread(o => webSocket.Connect()).Start();
                                    }
                                    ;
                                }).Start();
                            }

                            catch (OutOfMemoryException t)
                            {
                                Thread.Sleep(1000);
                            }
                        }
        }


        private static void Main(string[] args)
        {
            var url = "../../address.xml";
            try
            {
                var idx = 0;
                var xml = new XmlDocument();
                xml.Load(url);
                var list = xml.SelectNodes("iplist/ipv4");
                Console.WriteLine(list.Count);
                foreach (XmlNode node in list)
                {
                    Console.WriteLine(idx++);
                    var start = node["sno"].InnerText;
                    var end = node["eno"].InnerText;
                    LoopIp(start, end);
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}

//var platform = "youtube";
//var username = "이히ㅣㅣ";


//while (WebSocket.IsAlive)
//{
//    var message = Console.ReadLine();
//    string str = $"\"platform\" : \"{platform}\", \"username\" : \"{username}\", \"message\" : \"{message}\", \"type\" : \"chat_message\"";
//    WebSocket.Send(str);
//}