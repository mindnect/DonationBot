using System;
using System.Threading;
using ChatAppLib;
using ChatAppLib.Brokers;

namespace ChatApp
{
    internal class ChatAppMain
    {
        private static void Main()
        {
            var kakoHook = new KakaoBroker();
            kakoHook.Init();
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