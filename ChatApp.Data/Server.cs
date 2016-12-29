#region

using System.Collections.Generic;
using ChatAppLib.Extensions;
using ChatAppLib.Models;
using WebSocketSharp.Server;

#endregion

namespace ChatAppLib
{
    public class Server
    {
        private const int Port = 14416;
        private static readonly List<BroadService> BroadServices;
        private static WebSocketServer _wssv;

        static Server()
        {
            BroadServices = new List<BroadService>();
        }

        public static void RegisterService(BroadService server)
        {
            BroadServices.Add(server);
        }

        public static void SendMessage(Packet @base)
        {
            foreach (var echoService in BroadServices)
                echoService.SendMessage(@base.Serialize());
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