using System;
using System.Threading;
using ChatApp.Client.Extensions;
using ChatApp.Client.Models;
using WebSocketSharp;

namespace ChatApp.Client
{
    public static class Client
    {
        private const string Address = "ws://localhost:14416/Broad";

        private const string MsgEventOpen = "[Open]";
        private const string MsgEventClose = "[Close]";
        private const string MsgEventError = "[Error]";
        private const string MsgEventRetry = "[Retry]";

        private static bool _isRetry;

        static Client()
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

        public static event Action<string> OnLogging;
        public static event Action<Packet> OnNotice, OnEnter, OnRename, OnExit, OnChat, OnWhisper, OnSpon;

        public static void Start()
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

        public static void Stop()
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
                OnLogging?.Invoke(MsgEventRetry);
                Console.WriteLine(MsgEventRetry);
                Start();
                Thread.Sleep(100);
            }
            _isRetry = false;
        }

        private static void OnMessageHandler(object sender, MessageEventArgs e)
        {
            var packet = JSon.DeSerialize(e.Data);
            Console.WriteLine(packet);
            switch (packet.type)
            {
                case PacketType.log:
                    OnLogging?.Invoke(packet.message.msg);
                    break;
                case PacketType.notice:
                    OnNotice?.Invoke(packet);
                    break;
                case PacketType.enter:
                    OnEnter?.Invoke(packet);
                    break;
                case PacketType.rename:
                    OnRename?.Invoke(packet);
                    break;
                case PacketType.exit:
                    OnExit?.Invoke(packet);
                    break;
                case PacketType.chat:
                    OnChat?.Invoke(packet);
                    break;
                case PacketType.whisper:
                    OnWhisper?.Invoke(packet);
                    break;
                case PacketType.spon:
                    OnSpon?.Invoke(packet);
                    break;
            }
        }

        private static void OnErrorHandler(object sender, ErrorEventArgs e)
        {
            if (IsConsole) Console.WriteLine(MsgEventError + e.Message);
            OnLogging?.Invoke(MsgEventError + e.Message);
        }

        private static void OnCloseHandler(object sender, CloseEventArgs e)
        {
            if (_isRetry) return;
            new Thread(Retry).Start();

            if (IsConsole) Console.WriteLine(MsgEventClose + e.Reason);
            OnLogging?.Invoke(MsgEventOpen + e.Reason);
        }

        private static void OnOpenHandler(object sender, EventArgs e)
        {
            if (IsConsole) Console.WriteLine(MsgEventOpen);
            OnLogging?.Invoke(MsgEventOpen);
        }
    }
}