using System;
using System.Collections.Generic;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace TestServer
{
    public class Program
    {
        public static List<EchoServer> server;

        public static void Main(string[] args)
        {
            var wssv = new WebSocketServer(7287);
            wssv.Log.Level = LogLevel.Fatal;
            wssv.AddWebSocketService<EchoServer>("/Echo");
            wssv.Start();
            Console.ReadKey(true);
            wssv.Stop();
        }
    }
}