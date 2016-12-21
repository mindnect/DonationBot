using System;
using System.Threading;
using Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp;

namespace TestClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var ws = new WebSocket("ws://localhost:14416/Echo"))
            {
                ws.OnMessage += OnMessage;
                ws.Connect();
                Console.ReadKey(true);
                ws.Close();
            }
        }

        private static void OnMessage(object sender, MessageEventArgs e)
        {
            var obj = JsonConvert.DeserializeObject(e.Data,DataExtension.JsonSerializerSettings);
            Console.WriteLine(obj.GetType());
            Console.WriteLine(e.Data);
        }
    }
}