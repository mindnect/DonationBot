using JSAssist.Widget;
using System;
using System.Collections.Generic;
using WebSocket;
using WebSocket.Server;

namespace JSAssist
{
	internal class JSAssistChatServer : WebSocketBehavior
	{
		public JSAssistChatServer()
		{
		}

		protected override void OnClose(CloseEventArgs e)
		{
			base.OnClose(e);
			Program.server.UnregisterChatServer(this);
		}

		protected override void OnMessage(MessageEventArgs e)
		{
			base.OnMessage(e);
		}

		protected override void OnOpen()
		{
			base.OnOpen();
			this.SendChatPreset();
			Program.server.RegisterChatServer(this);
		}

		public void SendChatMessage(ChatData chat)
		{
			string str = string.Format("\"platform\" : \"{0}\", \"username\" : \"{1}\", \"message\" : \"{2}\", \"type\" : \"chat_message\"", chat.user.platform, chat.user.username, chat.message);
			base.Send(string.Concat("{", str, "}"));
		}

		public void SendChatPreset(string jsonData)
		{
			base.Send(jsonData);
		}

		public void SendChatPreset()
		{
			foreach (WidgetChatPreset widgetChatPreset in Program.config.listChatPreset)
			{
				base.Send(widgetChatPreset.ToJson());
			}
		}
	}
}