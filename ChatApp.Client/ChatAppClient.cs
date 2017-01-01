using System;
using System.Threading;

namespace ChatApp.ClientLib
{
    public class ChatAppClient
    {
        public static void Main()
        {
            Client.Client.IsConsole = true;
            Client.Client.Start();
            Console.ReadKey();

        }
    }
}