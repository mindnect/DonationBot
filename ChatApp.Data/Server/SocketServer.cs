using System;
using System.Collections.Generic;
using Comm.Extensions;
using Comm.Packets;
using Comm.Service;
using WebSocketSharp.Server;

namespace Comm.Server
{
    public static class SocketServer
    {
        private const int Port = 14416;
        private static readonly List<BroadService> BroadServices;
        private static WebSocketServer _wssv;

        static SocketServer()
        {
            BroadServices = new List<BroadService>();
        }

        public static void RegisterService(BroadService server)
        {
            BroadServices.Add(server);
        }

        public static void SendMessage(Packet packet)
        {
            Console.WriteLine(packet.ToString());
            foreach (var echoService in BroadServices)
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
            BroadServices.Remove(server);
        }
    }
}