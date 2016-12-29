using System;
using ChatAppLib;
using ChatAppLib.Extensions;
using ChatAppLib.Models;

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

        private static void OnPacket(MessageModel packet)
        {
            Console.WriteLine(packet.Message);
        }

        private static void OnMessage(string packet)
        {
            
        }
    }
}