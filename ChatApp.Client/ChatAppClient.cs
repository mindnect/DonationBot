using System;
using System.Threading;

namespace ChatApp.ClientLib
{
    public class ChatAppClient
    {
        public static void Main()
        {
            Client.IsConsole = true;
            Client.Start();
            Console.ReadKey();

        }
    }
}