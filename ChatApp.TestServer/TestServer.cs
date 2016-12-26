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
                var packet1  = new SponBasePacket(PacketType.SPON, new User()
                {
                    nickname = "테스터",
                    platform = "테스트",
                    rank = "PD"
                }, "테스트 메세지", str);


                var packet2 = new CommandBasePacket(PacketType.COMMAND, new User()
                {
                    nickname = "테스터",
                    platform = "테스트",
                    rank = "PD"
                }, "테스트 메세지", str);

                Server.SendMessage(packet2);
            }
            //Server.StopServer();
        }
    }
}