using System;
using Comm;
using Comm.Packets;

namespace TestServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Server.StartServer();
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
                Server.SendMessage(packet);
            }
            //Server.StopServer();
        }
    }
}