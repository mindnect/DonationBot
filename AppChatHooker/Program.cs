using System;
using System.IO;
using System.Threading;
using AlcoholV;

namespace ChatHooker
{
    internal class Program
    {
        public const string FilePath = "c:\\DonationBot\\Donation.txt";


        private static void Main(string[] args)
        {
            var t = new ChatTVPot();
            t.Initialize();
            t.FindPotplayer();
          
            
            while (true)
            {
                t.Update();

                Thread.Sleep(100);
                var list = t.GetNewChatList();


                var writer = File.AppendText(FilePath);
                writer.AutoFlush = true;
                foreach (var chatData in list)
                {

                    //if (chatData.isSpon)
                    {
                        //Console.WriteLine(chatData.user.platform);
                        //Console.WriteLine(chatData.user.username +"/t"+ chatData.amount+"/t"+chatData.message);
                        writer.WriteLine(chatData.user.username + "\t" + chatData.message + "\t" + chatData.amount);
                        //writer.WriteLine
                    }
                    
                }
                list.Clear();
                writer.Close();
            }

        
        }
    }
}