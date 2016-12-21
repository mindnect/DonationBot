using System;
using System.Collections.Generic;
using Data;
using WebSocketSharp.Server;

namespace ChatApp.AppServer
{
    internal static class Server
    {
        private static readonly List<EchoService> ServiceList;
        private static WebSocketServer _wssv;

        static Server()
        {
            ServiceList = new List<EchoService>();
        }

        public static void RegisterService(EchoService server)
        {
            ServiceList.Add(server);
        }

        public static void SendMessage(MessageEntity message)
        {
            Console.WriteLine(message.ToString());
            foreach (var echoService in ServiceList)
                echoService.SendMessage(message.ToSerialize());
        }

        public static void StartServer()
        {
            _wssv = new WebSocketServer(14416);
            _wssv.AddWebSocketService<EchoService>("/Echo");
            try
            {
                _wssv.Start();
            }
            catch
            {
                // ignored
            }
        }

        public static void StopServer()
        {
            _wssv.Stop();
        }

        public static void UnregisterService(EchoService server)
        {
            ServiceList.Remove(server);
        }
    }
}