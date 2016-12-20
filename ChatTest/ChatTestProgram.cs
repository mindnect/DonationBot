using System;
using System.IO;
using System.Threading;
using Database;

namespace ChatTest
{
    internal class ChatTestProgram
    {
        private static void Main()
        {
            Thread.Sleep(1000);
            while (true)
            {
                foreach (var chat in ChatDB.Chats.FindAll())
                {
                    Console.WriteLine(chat.ToString());
                    ChatDB.Chats.Delete(chat.Id);
                }
                Thread.Sleep(1000);
            }
        }
    }
}