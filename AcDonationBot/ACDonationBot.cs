using System.Linq;
using System.Reflection;
using Comm.Client;
using Comm.Extensions;
using Comm.Packets;
using Verse;

namespace AlcoholV
{
    [StaticConstructorOnStartup]
    public class AcDonationBot
    {
        static AcDonationBot()
        {
            Log.Message(AssemblyName + " 초기화");
            SocketClient.OnClose += OnClose;
            SocketClient.OnError += OnError;
            SocketClient.OnOpen += OnOpen;
            SocketClient.OnRetry += OnRetry;
            SocketClient.Connect();
        }


        private static Assembly Assembly => Assembly.GetAssembly(typeof(AcDonationBot));

        public static string AssemblyName => Assembly.FullName.Split(',').First();

        private static void OnRetry(string s)
        {
            Log.Message(s);
        }

        private static void OnOpen(string s)
        {
            Log.Message(s);
        }

        private static void OnError(string s)
        {
            Log.Message(s);
        }

        private static void OnClose(string s)
        {
            Log.Message(s);
        }
    }
}