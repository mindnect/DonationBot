using System;

namespace WebSocket.Net
{
	internal enum InputChunkState
	{
		None,
		Data,
		DataEnded,
		Trailer,
		End
	}
}