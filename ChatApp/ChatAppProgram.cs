using System.Threading;
using ChatApp.Chat;
using Database;

namespace ChatApp
{
    internal class ChatAppProgram
    {
        private static void Main()
        {
         
            ChatDB.Reset();

            var t = new ChatTVPot();
            t.Init();
            while (true)
            {
                Thread.Sleep(100);
                t.Update();
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}