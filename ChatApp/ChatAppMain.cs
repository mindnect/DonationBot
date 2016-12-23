using System;
using System.Threading;
using ChatApp.Chats;
using Comm.Server;

namespace ChatApp
{
    internal class ChatAppMain
    {
        private static void Main()
        {
            var chatTVPot = new ChatTVPot();
            chatTVPot.Init();

            SocketServer.StartServer();
            do
            {
                while (!Console.KeyAvailable)
                {
                    Thread.Sleep(100);
                    chatTVPot.Update();
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            SocketServer.StopServer();
        }
    }
}