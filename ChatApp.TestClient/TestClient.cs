using System;
using ChatAppLib.Data;
using ChatAppLib;
using ChatAppLib.Extensions;

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

        private static void OnPacket(BasePacket obj)
        {
            Console.WriteLine(obj.message);
        }

        private static void OnMessage(string packet)
        {
            
        }
    }
}