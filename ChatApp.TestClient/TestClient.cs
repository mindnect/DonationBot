using System;
using Comm.Client;
using Comm.Extensions;
using Comm.Packets;

namespace TestClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SocketClient.OnMessage += OnMessage;
            SocketClient.Connect();

            Console.ReadKey();
            SocketClient.Close();
        }

        private static void OnMessage(Packet obj)
        {
            Console.WriteLine(obj.message);
        }

        private static void OnMessage(string packet)
        {
            
        }
    }
}