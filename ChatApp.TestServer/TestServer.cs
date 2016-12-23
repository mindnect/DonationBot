using System;
using Comm.Packets;
using Comm.Server;

namespace TestServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SocketServer.StartServer();
            while (true)
            {
                var str = Console.ReadLine();
                Packet packet = new Donation
                {
                    amount = str,
                    message = "테스트 메세지",
                    user = new User
                    {
                        nickname = "테스터",
                        platform = "다음"
                    }
                };
                SocketServer.SendMessage(packet);
            }
            //SocketServer.StopServer();
        }
    }
}