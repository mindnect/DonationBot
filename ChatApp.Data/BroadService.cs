using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace ChatAppLib
{
    public class BroadService : WebSocketBehavior
    {
        public const string Path = "/Broad";

        public void SendMessage(string str)
        {
            Send(str);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine(e);
            base.OnMessage(e);
        }

        protected override void OnOpen()
        {
            Console.WriteLine("클라이언트 접속");
            Server.RegisterService(this);
            base.OnOpen();
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("클라이언트 연결종료");
            Server.UnregisterService(this);
            base.OnClose(e);
        }
    }
}