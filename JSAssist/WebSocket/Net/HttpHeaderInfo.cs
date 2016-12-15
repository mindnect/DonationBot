using System;

namespace WebSocket.Net
{
	internal class HttpHeaderInfo
	{
		private string _name;

		private HttpHeaderType _type;

		internal bool IsMultiValueInRequest
		{
			get
			{
				return (this._type & HttpHeaderType.MultiValueInRequest) == HttpHeaderType.MultiValueInRequest;
			}
		}

		internal bool IsMultiValueInResponse
		{
			get
			{
				return (this._type & HttpHeaderType.MultiValueInResponse) == HttpHeaderType.MultiValueInResponse;
			}
		}

		public bool IsRequest
		{
			get
			{
				return (this._type & HttpHeaderType.Request) == HttpHeaderType.Request;
			}
		}

		public bool IsResponse
		{
			get
			{
				return (this._type & HttpHeaderType.Response) == HttpHeaderType.Response;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		public HttpHeaderType Type
		{
			get
			{
				return this._type;
			}
		}

		internal HttpHeaderInfo(string name, HttpHeaderType type)
		{
			this._name = name;
			this._type = type;
		}

		public bool IsMultiValue(bool response)
		{
			if ((this._type & HttpHeaderType.MultiValue) != HttpHeaderType.MultiValue)
			{
				if (!response)
				{
					return this.IsMultiValueInRequest;
				}
				return this.IsMultiValueInResponse;
			}
			if (!response)
			{
				return this.IsRequest;
			}
			return this.IsResponse;
		}

		public bool IsRestricted(bool response)
		{
			if ((this._type & HttpHeaderType.Restricted) != HttpHeaderType.Restricted)
			{
				return false;
			}
			if (!response)
			{
				return this.IsRequest;
			}
			return this.IsResponse;
		}
	}
}