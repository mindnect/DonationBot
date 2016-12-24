using System;
using Comm;
using Comm.Extensions;
using Comm.Packets;

namespace TestClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Client.OnPacket += OnPacket;
            Client.Start();

            Console.ReadKey();
            Client.Stop();
        }

        private static void OnPacket(Packet obj)
        {
            Console.WriteLine(obj.message);
        }

        private static void OnMessage(string packet)
        {
            
        }
    }
}