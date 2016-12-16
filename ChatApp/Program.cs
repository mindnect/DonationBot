using System.Threading;
using LiteDB;

namespace ChatApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //var query = Chats.Include(x=>x.UserData).FindAll();
            //foreach (var c in query)
            //{
            //    Console.WriteLine(c.ToString());
            //}

            
            var t = new ChatTVPot();
            t.Init();
            
            while (true)
            {
                Thread.Sleep(100);
                ///LiteDB.FileDiskService k = new FileDiskService();
                
                t.Update();
                //var list = t.GetNewChatList();
                //foreach (var chatData in list)
                //{
                //    var msg = chatData.ToString();
                //    Console.WriteLine(msg);

                //    Database.Users.Insert(chatData.UserData);
                //    Database.Users.EnsureIndex(x => x.NickName);

                //    Database.Chats.Insert(chatData);
                //}
                //list.Clear();
            }
        }
    }
}