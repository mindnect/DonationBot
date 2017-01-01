using System;
using System.Threading;
using ChatApp.Server.Parsers;

namespace ChatApp.Server
{
    public class ChatAppMain
    {
        public static void Main()
        {
            var kakoHook = new KakaoParser();
            Server.StartServer();
            do
            {
                while (!Console.KeyAvailable)
                {
                    Thread.Sleep(100);
                    kakoHook.Update();
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            Server.StopServer();
        }
    }
}