using System;
using System.IO;
using System.Threading;
using ChatApp;

namespace ChatApp
{
    internal class Program
    {
        public const string FilePath = "c:\\DonationBot\\Donation.txt";


        private static void Main(string[] args)
        {
            var t = new ChatTVPot();
            t.Initialize();

            while (true)
            {
                t.Update();
                var list = t.GetNewChatList();
                var writer = File.AppendText(FilePath);
                writer.AutoFlush = true;
                foreach (var chatData in list)
                {
                    //Thread.Sleep(100);
                    //if (chatData.isDonation)
                    {
                        //writer.WriteLine(chatData.user.nickName + "\t" + chatData.message + "\t" + chatData.amount);
                        Console.WriteLine(chatData.user.rank + "\t" + chatData.user.nickName + "\t" + chatData.message + "\t" + chatData.amount);
                        //writer.WriteLine
                    }
                    
                }
                list.Clear();
                writer.Close();
            }

        
        }
    }
}