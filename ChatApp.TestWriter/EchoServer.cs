using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace TestServer
{
    public class EchoServer : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            base.Send(e.Data);
            base.OnMessage(e);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            //base.OnError(e);
            //Console.WriteLine(e.Message);
            var t = e.Message;
        }

        protected override void OnOpen()
        {
            Console.WriteLine("Client Connected");
            base.OnOpen();
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("Client Disconnected");
            base.OnClose(e);
        }

    }
}