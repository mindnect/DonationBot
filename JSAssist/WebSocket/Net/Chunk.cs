using System;

namespace WebSocket.Net
{
	internal class Chunk
	{
		private byte[] _data;

		private int _offset;

		public int ReadLeft
		{
			get
			{
				return (int)this._data.Length - this._offset;
			}
		}

		public Chunk(byte[] data)
		{
			this._data = data;
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			int length = (int)this._data.Length - this._offset;
			if (length == 0)
			{
				return length;
			}
			if (count > length)
			{
				count = length;
			}
			Buffer.BlockCopy(this._data, this._offset, buffer, offset, count);
			this._offset = this._offset + count;
			return count;
		}
	}
}