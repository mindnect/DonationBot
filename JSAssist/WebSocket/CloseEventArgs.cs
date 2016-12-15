using System;

namespace WebSocket
{
	public class CloseEventArgs : EventArgs
	{
		private bool _clean;

		private WebSocket.PayloadData _payloadData;

		public ushort Code
		{
			get
			{
				return this._payloadData.Code;
			}
		}

		internal WebSocket.PayloadData PayloadData
		{
			get
			{
				return this._payloadData;
			}
		}

		public string Reason
		{
			get
			{
				return this._payloadData.Reason ?? string.Empty;
			}
		}

		public bool WasClean
		{
			get
			{
				return this._clean;
			}
			internal set
			{
				this._clean = value;
			}
		}

		internal CloseEventArgs()
		{
			this._payloadData = WebSocket.PayloadData.Empty;
		}

		internal CloseEventArgs(ushort code) : this(code, null)
		{
		}

		internal CloseEventArgs(CloseStatusCode code) : this((ushort)code, null)
		{
		}

		internal CloseEventArgs(WebSocket.PayloadData payloadData)
		{
			this._payloadData = payloadData;
		}

		internal CloseEventArgs(ushort code, string reason)
		{
			this._payloadData = new WebSocket.PayloadData(code, reason);
		}

		internal CloseEventArgs(CloseStatusCode code, string reason) : this((ushort)code, reason)
		{
		}
	}
}