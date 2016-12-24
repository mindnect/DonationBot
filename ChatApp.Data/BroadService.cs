using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Comm
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
            Console.WriteLine("[Broad] Ŭ���̾�Ʈ ����");
            Server.RegisterService(this);
            base.OnOpen();
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("[Broad] Ŭ���̾�Ʈ ��������");
            Server.UnregisterService(this);
            base.OnClose(e);
        }
    }
}