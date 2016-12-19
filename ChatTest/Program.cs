using System;
using System.IO;
using System.Threading;
using Database;

namespace ChatTest
{
    internal class Program
    {

        private static void Main()
        {
            //var watcher = new FileSystemWatcher
            //{
            //    Path = "c:\\DonationBot\\",
            //    NotifyFilter = NotifyFilters.LastWrite
            //};
            //watcher.Filter = "test.txt";
            //watcher.Changed += OnChanged;
            //watcher.EnableRaisingEvents = true;

            while (true)
            {
                Console.WriteLine(ChatDB.Chats.Count());
                Thread.Sleep(100);
            }
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            //Console.WriteLine(source.ToString());
            //Console.WriteLine(e.Name);
        }
    }
}