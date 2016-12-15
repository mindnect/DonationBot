using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WebSocket.Net
{
	internal sealed class HttpConnection
	{
		private byte[] _buffer;

		private const int _bufferLength = 8192;

		private WebSocket.Net.HttpListenerContext _context;

		private bool _contextRegistered;

		private StringBuilder _currentLine;

		private InputState _inputState;

		private RequestStream _inputStream;

		private WebSocket.Net.HttpListener _lastListener;

		private LineState _lineState;

		private EndPointListener _listener;

		private ResponseStream _outputStream;

		private int _position;

		private MemoryStream _requestBuffer;

		private int _reuses;

		private bool _secure;

		private Socket _socket;

		private System.IO.Stream _stream;

		private object _sync;

		private int _timeout;

		private Dictionary<int, bool> _timeoutCanceled;

		private Timer _timer;

		public bool IsClosed
		{
			get
			{
				return this._socket == null;
			}
		}

		public bool IsSecure
		{
			get
			{
				return this._secure;
			}
		}

		public IPEndPoint LocalEndPoint
		{
			get
			{
				return (IPEndPoint)this._socket.LocalEndPoint;
			}
		}

		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return (IPEndPoint)this._socket.RemoteEndPoint;
			}
		}

		public int Reuses
		{
			get
			{
				return this._reuses;
			}
		}

		public System.IO.Stream Stream
		{
			get
			{
				return this._stream;
			}
		}

		internal HttpConnection(Socket socket, EndPointListener listener)
		{
			this._socket = socket;
			this._listener = listener;
			this._secure = listener.IsSecure;
			NetworkStream networkStream = new NetworkStream(socket, false);
			if (!this._secure)
			{
				this._stream = networkStream;
			}
			else
			{
				ServerSslConfiguration sslConfiguration = listener.SslConfiguration;
				SslStream sslStream = new SslStream(networkStream, false, sslConfiguration.ClientCertificateValidationCallback);
				sslStream.AuthenticateAsServer(sslConfiguration.ServerCertificate, sslConfiguration.ClientCertificateRequired, sslConfiguration.EnabledSslProtocols, sslConfiguration.CheckCertificateRevocation);
				this._stream = sslStream;
			}
			this._sync = new object();
			this._timeout = 90000;
			this._timeoutCanceled = new Dictionary<int, bool>();
			this._timer = new Timer(new TimerCallback(HttpConnection.onTimeout), this, -1, -1);
			this.init();
		}

		public void BeginReadRequest()
		{
			if (this._buffer == null)
			{
				this._buffer = new byte[8192];
			}
			if (this._reuses == 1)
			{
				this._timeout = 15000;
			}
			try
			{
				this._timeoutCanceled.Add(this._reuses, false);
				this._timer.Change(this._timeout, -1);
				this._stream.BeginRead(this._buffer, 0, 8192, new AsyncCallback(HttpConnection.onRead), this);
			}
			catch
			{
				this.close();
			}
		}

		private void close()
		{
			lock (this._sync)
			{
				if (this._socket != null)
				{
					this.disposeTimer();
					this.disposeRequestBuffer();
					this.disposeStream();
					this.closeSocket();
				}
				else
				{
					return;
				}
			}
			this.unregisterContext();
			this.removeConnection();
		}

		internal void Close(bool force)
		{
			if (this._socket == null)
			{
				return;
			}
			lock (this._sync)
			{
				if (this._socket != null)
				{
					if (!force)
					{
						this.GetResponseStream().Close(false);
						if (!this._context.Response.CloseConnection && this._context.Request.FlushInput())
						{
							this._reuses = this._reuses + 1;
							this.disposeRequestBuffer();
							this.unregisterContext();
							this.init();
							this.BeginReadRequest();
							return;
						}
					}
					else if (this._outputStream != null)
					{
						this._outputStream.Close(true);
					}
					this.close();
				}
			}
		}

		public void Close()
		{
			this.Close(false);
		}

		private void closeSocket()
		{
			try
			{
				this._socket.Shutdown(SocketShutdown.Both);
			}
			catch
			{
			}
			this._socket.Close();
			this._socket = null;
		}

		private void disposeRequestBuffer()
		{
			if (this._requestBuffer == null)
			{
				return;
			}
			this._requestBuffer.Dispose();
			this._requestBuffer = null;
		}

		private void disposeStream()
		{
			if (this._stream == null)
			{
				return;
			}
			this._inputStream = null;
			this._outputStream = null;
			this._stream.Dispose();
			this._stream = null;
		}

		private void disposeTimer()
		{
			if (this._timer == null)
			{
				return;
			}
			try
			{
				this._timer.Change(-1, -1);
			}
			catch
			{
			}
			this._timer.Dispose();
			this._timer = null;
		}

		public RequestStream GetRequestStream(long contentLength, bool chunked)
		{
			RequestStream requestStream;
			if (this._inputStream != null || this._socket == null)
			{
				return this._inputStream;
			}
			lock (this._sync)
			{
				if (this._socket != null)
				{
					byte[] buffer = this._requestBuffer.GetBuffer();
					int length = (int)this._requestBuffer.Length;
					this.disposeRequestBuffer();
					if (!chunked)
					{
						this._inputStream = new RequestStream(this._stream, buffer, this._position, length - this._position, contentLength);
					}
					else
					{
						this._context.Response.SendChunked = true;
						this._inputStream = new ChunkedRequestStream(this._stream, buffer, this._position, length - this._position, this._context);
					}
					requestStream = this._inputStream;
				}
				else
				{
					requestStream = this._inputStream;
				}
			}
			return requestStream;
		}

		public ResponseStream GetResponseStream()
		{
			ResponseStream responseStream;
			if (this._outputStream != null || this._socket == null)
			{
				return this._outputStream;
			}
			lock (this._sync)
			{
				if (this._socket != null)
				{
					WebSocket.Net.HttpListener listener = this._context.Listener;
					bool flag = (listener != null ? listener.IgnoreWriteExceptions : true);
					this._outputStream = new ResponseStream(this._stream, this._context.Response, flag);
					responseStream = this._outputStream;
				}
				else
				{
					responseStream = this._outputStream;
				}
			}
			return responseStream;
		}

		private void init()
		{
			this._context = new WebSocket.Net.HttpListenerContext(this);
			this._inputState = InputState.RequestLine;
			this._inputStream = null;
			this._lineState = LineState.None;
			this._outputStream = null;
			this._position = 0;
			this._requestBuffer = new MemoryStream();
		}

		private static void onRead(IAsyncResult asyncResult)
		{
			WebSocket.Net.HttpListener httpListener;
			HttpConnection asyncState = (HttpConnection)asyncResult.AsyncState;
			if (asyncState._socket == null)
			{
				return;
			}
			lock (asyncState._sync)
			{
				if (asyncState._socket != null)
				{
					int num = -1;
					int length = 0;
					try
					{
						int num1 = asyncState._reuses;
						if (!asyncState._timeoutCanceled[num1])
						{
							asyncState._timer.Change(-1, -1);
							asyncState._timeoutCanceled[num1] = true;
						}
						num = asyncState._stream.EndRead(asyncResult);
						asyncState._requestBuffer.Write(asyncState._buffer, 0, num);
						length = (int)asyncState._requestBuffer.Length;
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						if (asyncState._requestBuffer == null || asyncState._requestBuffer.Length <= (long)0)
						{
							asyncState.close();
							return;
						}
						else
						{
							asyncState.SendError(exception.Message, 400);
							return;
						}
					}
					if (num <= 0)
					{
						asyncState.close();
					}
					else if (!asyncState.processInput(asyncState._requestBuffer.GetBuffer(), length))
					{
						asyncState._stream.BeginRead(asyncState._buffer, 0, 8192, new AsyncCallback(HttpConnection.onRead), asyncState);
					}
					else
					{
						if (!asyncState._context.HasError)
						{
							asyncState._context.Request.FinishInitialization();
						}
						if (asyncState._context.HasError)
						{
							asyncState.SendError();
						}
						else if (asyncState._listener.TrySearchHttpListener(asyncState._context.Request.Url, out httpListener))
						{
							if (asyncState._lastListener != httpListener)
							{
								asyncState.removeConnection();
								if (httpListener.AddConnection(asyncState))
								{
									asyncState._lastListener = httpListener;
								}
								else
								{
									asyncState.close();
									return;
								}
							}
							asyncState._context.Listener = httpListener;
							if (asyncState._context.Authenticate())
							{
								if (asyncState._context.Register())
								{
									asyncState._contextRegistered = true;
								}
							}
						}
						else
						{
							asyncState.SendError(null, 404);
						}
					}
				}
			}
		}

		private static void onTimeout(object state)
		{
			HttpConnection httpConnection = (HttpConnection)state;
			int num = httpConnection._reuses;
			if (httpConnection._socket == null)
			{
				return;
			}
			lock (httpConnection._sync)
			{
				if (httpConnection._socket != null)
				{
					if (!httpConnection._timeoutCanceled[num])
					{
						httpConnection.SendError(null, 408);
					}
				}
			}
		}

		private bool processInput(byte[] data, int length)
		{
			bool flag;
			if (this._currentLine == null)
			{
				this._currentLine = new StringBuilder(64);
			}
			int num = 0;
			try
			{
				do
				{
				Label2:
					string str = this.readLineFrom(data, this._position, length, out num);
					string str1 = str;
					if (str != null)
					{
						this._position = this._position + num;
						if (str1.Length == 0)
						{
							if (this._inputState != InputState.RequestLine)
							{
								if (this._position > 32768)
								{
									this._context.ErrorMessage = "Headers too long";
								}
								this._currentLine = null;
								flag = true;
								return flag;
							}
							else
							{
								goto Label2;
							}
						}
						else if (this._inputState != InputState.RequestLine)
						{
							this._context.Request.AddHeader(str1);
						}
						else
						{
							this._context.Request.SetRequestLine(str1);
							this._inputState = InputState.Headers;
						}
					}
					else
					{
						this._position = this._position + num;
						if (this._position < 32768)
						{
							return false;
						}
						this._context.ErrorMessage = "Headers too long";
						return true;
					}
				}
				while (!this._context.HasError);
				flag = true;
			}
			catch (Exception exception)
			{
				this._context.ErrorMessage = exception.Message;
				flag = true;
			}
			return flag;
		}

		private string readLineFrom(byte[] buffer, int offset, int length, out int read)
		{
			read = 0;
			for (int i = offset; i < length && this._lineState != LineState.Lf; i++)
			{
				read = read + 1;
				byte num = buffer[i];
				if (num == 13)
				{
					this._lineState = LineState.Cr;
				}
				else if (num != 10)
				{
					this._currentLine.Append((char)num);
				}
				else
				{
					this._lineState = LineState.Lf;
				}
			}
			if (this._lineState != LineState.Lf)
			{
				return null;
			}
			string str = this._currentLine.ToString();
			this._currentLine.Length = 0;
			this._lineState = LineState.None;
			return str;
		}

		private void removeConnection()
		{
			if (this._lastListener != null)
			{
				this._lastListener.RemoveConnection(this);
				return;
			}
			this._listener.RemoveConnection(this);
		}

		public void SendError()
		{
			this.SendError(this._context.ErrorMessage, this._context.ErrorStatus);
		}

		public void SendError(string message, int status)
		{
			if (this._socket == null)
			{
				return;
			}
			lock (this._sync)
			{
				if (this._socket != null)
				{
					try
					{
						WebSocket.Net.HttpListenerResponse response = this._context.Response;
						response.StatusCode = status;
						response.ContentType = "text/html";
						StringBuilder stringBuilder = new StringBuilder(64);
						stringBuilder.AppendFormat("<html><body><h1>{0} {1}", status, response.StatusDescription);
						if (message == null || message.Length <= 0)
						{
							stringBuilder.Append("</h1></body></html>");
						}
						else
						{
							stringBuilder.AppendFormat(" ({0})</h1></body></html>", message);
						}
						Encoding uTF8 = Encoding.UTF8;
						byte[] bytes = uTF8.GetBytes(stringBuilder.ToString());
						response.ContentEncoding = uTF8;
						response.ContentLength64 = (long)bytes.Length;
						response.Close(bytes, true);
					}
					catch
					{
						this.Close(true);
					}
				}
			}
		}

		private void unregisterContext()
		{
			if (!this._contextRegistered)
			{
				return;
			}
			this._context.Unregister();
			this._contextRegistered = false;
		}
	}
}