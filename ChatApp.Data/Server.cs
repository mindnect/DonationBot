using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ChatAppLib.Data;
using ChatAppLib.Extensions;
using WebSocketSharp.Server;

namespace ChatAppLib
{
    public class Server
    {
        private const int Port = 14416;
        private static readonly List<BroadService> _broadServices;
        private static WebSocketServer _wssv;

        static Server()
        {
            _broadServices = new List<BroadService>();
        }

        public static void RegisterService(BroadService server)
        {
            _broadServices.Add(server);
        }

        public static void SendMessage(BasePacket basePacket)
        {
            foreach (var echoService in _broadServices)
                echoService.SendMessage(basePacket.Serialize());
        }

        public static void StartServer()
        {
            _wssv = new WebSocketServer(Port);
            _wssv.AddWebSocketService<BroadService>(BroadService.Path);
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

        public static void UnregisterService(BroadService server)
        {
            _broadServices.Remove(server);
        }
    }
}