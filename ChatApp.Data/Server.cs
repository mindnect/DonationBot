using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Comm.Extensions;
using Comm.Packets;
using WebSocketSharp.Server;

namespace Comm
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

        public static void SendMessage(Packet packet)
        {
            Console.WriteLine(packet.ToString());
            foreach (var echoService in _broadServices)
                echoService.SendMessage(packet.Serialize());
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