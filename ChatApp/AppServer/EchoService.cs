using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace ChatApp.AppServer
{
    public class EchoService : WebSocketBehavior
    {
        public void SendMessage(string str)
        {
            Send(str);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            Send(e.Data);
            base.OnMessage(e);
        }

        protected override void OnOpen()
        {
            Console.WriteLine("[Echo] 클라이언트 접속");
            Server.RegisterService(this);
            base.OnOpen();
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("[Echo] 클라이언트 연결종료");
            Server.UnregisterService(this);
            base.OnClose(e);
        }
    }
}