using System;
using System.Threading;
using Comm.Extensions;
using Comm.Packets;
using WebSocketSharp;

namespace Comm.Client
{
    public static class SocketClient
    {
        private const string Address = "ws://localhost:14416/Broad";

        private const string MsgEventOpen = "[Open]";
        private const string MsgEventClose = "[Close]";
        private const string MsgEventError = "[Error]";
        private const string MsgEventRetry = "[Retry]";

        private static bool _isRetry;

        static SocketClient()
        {
            WebSocket = new WebSocket(Address);
            WebSocket.OnOpen += OnOpenHandler;
            WebSocket.OnClose += OnCloseHandler;
            WebSocket.OnError += OnErrorHandler;
            WebSocket.OnMessage += OnMessageHandler;
            WebSocket.Log.Level = LogLevel.Fatal;
        }

        public static bool IsConsole { get; set; }
        private static WebSocket WebSocket { get; }

        public static event Action<string> OnOpen , OnClose , OnError , OnRetry;
        public static event Action<Packet> OnMessage;

        public static void Connect()
        {
            try
            {
                WebSocket.Connect();
            }
            catch
            {
                //Console.WriteLine(e);
            }
        }

        public static void Close()
        {
            try
            {
                WebSocket.Close();
            }
            catch
            {
                //Console.WriteLine(e);
            }
        }

        private static void Retry()
        {
            _isRetry = true;
            while (!WebSocket.IsAlive)
            {
                OnRetry?.Invoke(MsgEventRetry);
                Console.WriteLine(MsgEventRetry);
                Connect();
                Thread.Sleep(100);
            }
            _isRetry = false;
        }

        private static void OnMessageHandler(object sender, MessageEventArgs e)
        {
            var xmlData = e.Data;
            var packet = Xml.Deserialize<Packet>(xmlData);
            OnMessage?.Invoke(packet);
        }

        private static void OnErrorHandler(object sender, ErrorEventArgs e)
        {
            if (IsConsole) Console.WriteLine(MsgEventError + e.Message);
            OnError?.Invoke(MsgEventError + e.Message);
        }

        private static void OnCloseHandler(object sender, CloseEventArgs e)
        {
            if (_isRetry) return;
            new Thread(Retry).Start();

            if (IsConsole) Console.WriteLine(MsgEventClose + e.Reason);
            OnClose?.Invoke(MsgEventOpen + e.Reason);
        }

        private static void OnOpenHandler(object sender, EventArgs e)
        {
            if (IsConsole) Console.WriteLine(MsgEventOpen);
            OnOpen?.Invoke(MsgEventOpen);
        }
    }
}