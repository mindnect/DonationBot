using System;
using ChatAppLib.Data;
using ChatAppLib;

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
                var packet  = new CommandPacket(PacketType.Donation, new User()
                {
                    nickname = "테스터",
                    platform = "테스트",
                    rank = "PD"
                }, str, "테스트 메세지");

                Server.SendMessage(packet);
            }
            //Server.StopServer();
        }
    }
}