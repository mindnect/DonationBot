using System;
using System.Threading;
using ChatApp.Parser;
using ChatAppLib;

namespace ChatApp
{
    internal class ChatAppMain
    {
        private static void Main()
        {
            var chatTVPot = new TvpotParser();
            chatTVPot.Init();

            Server.StartServer();
            do
            {
                while (!Console.KeyAvailable)
                {
                    Thread.Sleep(100);
                    chatTVPot.Update();
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            Server.StopServer();
        }
    }
}