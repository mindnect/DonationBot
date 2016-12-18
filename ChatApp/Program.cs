using System.Threading;
using ChatApp.Chat;

namespace ChatApp
{
    internal class Program
    {
        private static void Main()
        {
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