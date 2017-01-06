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
        private static bool _isRetry;

        static Client()
        {
            WebSocket = new WebSocket(Address);

            WebSocket.OnOpen += (sender, args) => { OnOpen?.Invoke(); };
            WebSocket.OnClose += (sender, args) =>
            {
                if (_isRetry) return;
                new Thread(Retry).Start();
                OnClose?.Invoke(args.Reason);
            };

            WebSocket.OnError += (sender, args) => { OnError?.Invoke(args.Message); };
            WebSocket.OnMessage += OnMessage;
            WebSocket.Log.Level = LogLevel.Fatal;
        }

        //public static bool IsConsole { get; set; }
        private static WebSocket WebSocket { get; }

        public static event Action OnRetry, OnOpen;
        public static event Action<string> OnError, OnClose;
        public static event Action<Packet> OnPacket, OnNotice, OnEnter, OnRename, OnExit, OnChat, OnWhisper, OnSpon;

        public static void Start()
        {
            try
            {
                WebSocket.Connect();
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
            }
        }

        public static void Stop()
        {
            try
            {
                WebSocket.Close();
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
            }
        }

        private static void Retry()
        {
            _isRetry = true;
            while (!WebSocket.IsAlive)
            {
                OnRetry?.Invoke();
                Start();
                Thread.Sleep(100);
            }
            _isRetry = false;
        }

        private static void OnMessage(object sender, MessageEventArgs eventArgs)
        {
            try
            {
                var packet = Xml.Deserialize<Packet>(eventArgs.Data);
                if (Environment.UserInteractive) Console.WriteLine(packet.ToString());
                OnPacket?.Invoke(packet);

                switch (packet.type)
                {
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
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
            }
        }
    }
}