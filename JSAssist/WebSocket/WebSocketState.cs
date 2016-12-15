using System;

namespace WebSocket
{
	public enum WebSocketState : ushort
	{
		Connecting,
		Open,
		Closing,
		Closed
	}
}