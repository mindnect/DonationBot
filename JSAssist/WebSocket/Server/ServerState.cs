using System;

namespace WebSocket.Server
{
	internal enum ServerState
	{
		Ready,
		Start,
		ShuttingDown,
		Stop
	}
}