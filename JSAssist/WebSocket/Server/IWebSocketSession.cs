using System;
using WebSocket;
using WebSocket.Net.WebSockets;

namespace WebSocket.Server
{
	public interface IWebSocketSession
	{
		WebSocketContext Context
		{
			get;
		}

		string ID
		{
			get;
		}

		string Protocol
		{
			get;
		}

		DateTime StartTime
		{
			get;
		}

		WebSocketState State
		{
			get;
		}
	}
}