using System;
using System.Threading;
using ChatAppLib;
using ChatAppLib.Models;

namespace TestServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Server.StartServer();
            var rand = new Random();
            
            while (true)
            {
                Thread.Sleep(100);
                ////100 @공격
                //var strs = Console.ReadLine().Split(new []{' '},2);

                ////PacketType.COMMAND |
                //var packet1 = new SponPacket( PacketType.SPON, new User
                //{
                //    Nickname = "테스터",
                //    Platform = "테스트",
                //    Rank = "PD"
                //}, message: strs[1], amount: strs[0]);


                //var str = Console.ReadLine();
                //var packet2 = new MessagePacket(PacketType.COMMAND| PacketType.MESSAGE, new User()
                //{
                //    nickname = "닉네임",
                //    platform = "tvpot",
                //    rank = "PD"
                //}, str);9

                var testMessage = new MessageModel(PacketType.MESSAGE, new UserModel(), "기본 메세지입니당");

                Server.SendMessage(testMessage);
            }
            //Server.StopServer();
        }
    }
}