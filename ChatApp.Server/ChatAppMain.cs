using System;
using System.Threading;
using ChatApp.Server.Models;
using ChatApp.Server.Parsers;

namespace ChatApp.Server
{
    public class ChatAppMain
    {
        public static void Main(string[] args)
        {
            Server.StartServer();


            if (args.Length == 0)
            {
                var kakoHook = new KakaoParser();
                do
                {
                    while (!Console.KeyAvailable)
                    {
                        Thread.Sleep(100);
                        kakoHook.Update();
                    }
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            }


            else if (args[0] == "test")
            {
                var rand = new Random(Guid.NewGuid().GetHashCode());
                var packets = new[]
                {
                    //Packet.CreateChat(new User("테스터", Platform.kakao), "테스트 메세지입니다."),
                    //Packet.CreateChat(new User("매니저1", Platform.kakao, "AD"), "@습격 명령어 채팅입니다."),
                    //Packet.CreateChat(new User("매니저2", Platform.kakao, "AD"), "@습격/공병 명령어 채팅입니다."),
                    //Packet.CreateChat(new User("매니저3", Platform.kakao, "AD"), "@습격/2000 명령어 채팅입니다."),
                    //Packet.CreateSpon(new User("후원자1", Platform.kakao), "일반 후원 입니다.", "1000"),
                    //Packet.CreateSpon(new User("후원자2", Platform.kakao), "@습격 명령어 후원입니다.", "1000"),
//                    Packet.CreateSpon(new User("후원자3", Platform.kakao), "@습격/공병 명령어 후원입니다.", "1000"),
                    Packet.CreateSpon(new User("후원자2", Platform.kakao), "@수면병 명령어 후원입니다.", "3000")
                    //Packet.CreateEnter(new User("방문객", Platform.kakao)),
                    //Packet.CreateExit(new User("방문객", Platform.kakao)),
                    //Packet.CreateWhisper(new User("귓말러",Platform.kakao),"테스팅 메세지" ),
                };

                var Count = packets.Length;
                do
                {
                    while (!Console.KeyAvailable)
                    {
                        Packet packet = null;

                        var index = rand.Next(Count);
                        packet = packets[index];
                        Console.WriteLine(packet);
                        Server.SendMessage(packet);
                        Console.ReadKey(true);
                    }
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            }


            Server.StopServer();
        }
    }
}