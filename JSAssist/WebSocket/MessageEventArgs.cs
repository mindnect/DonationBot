using System;

namespace WebSocket
{
	public class MessageEventArgs : EventArgs
	{
		private string _data;

		private bool _dataSet;

		private WebSocket.Opcode _opcode;

		private byte[] _rawData;

		public string Data
		{
			get
			{
				this.setData();
				return this._data;
			}
		}

		public bool IsBinary
		{
			get
			{
				return this._opcode == WebSocket.Opcode.Binary;
			}
		}

		public bool IsPing
		{
			get
			{
				return this._opcode == WebSocket.Opcode.Ping;
			}
		}

		public bool IsText
		{
			get
			{
				return this._opcode == WebSocket.Opcode.Text;
			}
		}

		internal WebSocket.Opcode Opcode
		{
			get
			{
				return this._opcode;
			}
		}

		public byte[] RawData
		{
			get
			{
				this.setData();
				return this._rawData;
			}
		}

		internal MessageEventArgs(WebSocketFrame frame)
		{
			this._opcode = frame.Opcode;
			this._rawData = frame.PayloadData.ApplicationData;
		}

		internal MessageEventArgs(WebSocket.Opcode opcode, byte[] rawData)
		{
			if ((long)rawData.Length > PayloadData.MaxLength)
			{
				throw new WebSocketException(CloseStatusCode.TooBig);
			}
			this._opcode = opcode;
			this._rawData = rawData;
		}

		private void setData()
		{
			if (this._dataSet)
			{
				return;
			}
			if (this._opcode == WebSocket.Opcode.Binary)
			{
				this._dataSet = true;
				return;
			}
			this._data = this._rawData.UTF8Decode();
			this._dataSet = true;
		}
	}
}