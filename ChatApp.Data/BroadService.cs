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
            Console.WriteLine("Ŭ���̾�Ʈ ����");
            Server.RegisterService(this);
            base.OnOpen();
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("Ŭ���̾�Ʈ ��������");
            Server.UnregisterService(this);
            base.OnClose(e);
        }
    }
}