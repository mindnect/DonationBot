using JSAssist.Widget;
using System;
using System.Collections.Generic;
using WebSocket.Server;

namespace JSAssist
{
	internal class Server
	{
		protected WebSocketServer wssv;

		public List<JSAssistChatServer> listChatServer;

		public Server()
		{
			this.listChatServer = new List<JSAssistChatServer>();
		}

		public void RegisterChatServer(JSAssistChatServer server)
		{
			this.listChatServer.Add(server);
		}

		public void SendChat(ChatData chat)
		{
			foreach (JSAssistChatServer jSAssistChatServer in this.listChatServer)
			{
				jSAssistChatServer.SendChatMessage(chat);
			}
		}

		public void SendChatPreset(WidgetChatPreset widgetChatData)
		{
			string json = widgetChatData.ToJson();
			foreach (JSAssistChatServer jSAssistChatServer in this.listChatServer)
			{
				jSAssistChatServer.SendChatPreset(json);
			}
		}

		public void StartServer()
		{
			this.wssv = new WebSocketServer(4649);
			this.wssv.AddWebSocketService<JSAssistChatServer>("/JSAssistChatServer");
			try
			{
				this.wssv.Start();
			}
			catch
			{
			}
		}

		public void StopServer()
		{
			this.wssv.Stop();
		}

		public void UnregisterChatServer(JSAssistChatServer server)
		{
			this.listChatServer.Remove(server);
		}
	}
}