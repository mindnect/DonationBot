using System;
using System.Threading;
using ChatApp.Service;
using Comm;

namespace ChatApp
{
    internal class ChatAppMain
    {
        private static void Main()
        {
            var chatTVPot = new TvpotService();
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