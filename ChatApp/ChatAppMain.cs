using System;
using System.Threading;
using ChatApp.AppServer;
using ChatApp.Chat;

namespace ChatApp
{
    internal class ChatAppMain
    {
        private static void Main()
        {
            var chatTVPot = new ChatTVPot();
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