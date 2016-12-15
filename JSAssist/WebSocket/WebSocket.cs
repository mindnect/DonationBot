using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using WebSocket.Net;
using WebSocket.Net.WebSockets;

namespace WebSocket
{
	public class WebSocket : IDisposable
	{
		private AuthenticationChallenge _authChallenge;

		private string _base64Key;

		private bool _client;

		private Action _closeContext;

		private CompressionMethod _compression;

		private WebSocketContext _context;

		private WebSocket.Net.CookieCollection _cookies;

		private NetworkCredential _credentials;

		private bool _emitOnPing;

		private bool _enableRedirection;

		private AutoResetEvent _exitReceiving;

		private string _extensions;

		private bool _extensionsRequested;

		private object _forMessageEventQueue;

		private object _forSend;

		private object _forState;

		private MemoryStream _fragmentsBuffer;

		private bool _fragmentsCompressed;

		private Opcode _fragmentsOpcode;

		private const string _guid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

		private Func<WebSocketContext, string> _handshakeRequestChecker;

		private bool _ignoreExtensions;

		private bool _inContinuation;

		private volatile bool _inMessage;

		private volatile Logger _logger;

		private Action<MessageEventArgs> _message;

		private Queue<MessageEventArgs> _messageEventQueue;

		private uint _nonceCount;

		private string _origin;

		private bool _preAuth;

		private string _protocol;

		private string[] _protocols;

		private bool _protocolsRequested;

		private NetworkCredential _proxyCredentials;

		private Uri _proxyUri;

		private volatile WebSocketState _readyState;

		private AutoResetEvent _receivePong;

		private bool _secure;

		private ClientSslConfiguration _sslConfig;

		private Stream _stream;

		private TcpClient _tcpClient;

		private Uri _uri;

		private const string _version = "13";

		private TimeSpan _waitTime;

		internal readonly static byte[] EmptyBytes;

		internal readonly static int FragmentLength;

		internal readonly static RandomNumberGenerator RandomNumber;

		public CompressionMethod Compression
		{
			get
			{
				return this._compression;
			}
			set
			{
				string str;
				lock (this._forState)
				{
					if (this.checkIfAvailable(true, false, true, false, false, true, out str))
					{
						this._compression = value;
					}
					else
					{
						this._logger.Error(str);
						this.error("An error has occurred in setting the compression.", null);
					}
				}
			}
		}

		internal WebSocket.Net.CookieCollection CookieCollection
		{
			get
			{
				return this._cookies;
			}
		}

		public IEnumerable<Cookie> Cookies
		{
			get
			{
				lock (this._cookies.SyncRoot)
				{
					foreach (Cookie _cooky in this._cookies)
					{
						yield return _cooky;
					}
				}
			}
		}

		public NetworkCredential Credentials
		{
			get
			{
				return this._credentials;
			}
		}

		internal Func<WebSocketContext, string> CustomHandshakeRequestChecker
		{
			get
			{
				return this._handshakeRequestChecker;
			}
			set
			{
				this._handshakeRequestChecker = value;
			}
		}

		public bool EmitOnPing
		{
			get
			{
				return this._emitOnPing;
			}
			set
			{
				this._emitOnPing = value;
			}
		}

		public bool EnableRedirection
		{
			get
			{
				return this._enableRedirection;
			}
			set
			{
				string str;
				lock (this._forState)
				{
					if (this.checkIfAvailable(true, false, true, false, false, true, out str))
					{
						this._enableRedirection = value;
					}
					else
					{
						this._logger.Error(str);
						this.error("An error has occurred in setting the enable redirection.", null);
					}
				}
			}
		}

		public string Extensions
		{
			get
			{
				return this._extensions ?? string.Empty;
			}
		}

		internal bool HasMessage
		{
			get
			{
				bool count;
				lock (this._forMessageEventQueue)
				{
					count = this._messageEventQueue.Count > 0;
				}
				return count;
			}
		}

		internal bool IgnoreExtensions
		{
			get
			{
				return this._ignoreExtensions;
			}
			set
			{
				this._ignoreExtensions = value;
			}
		}

		public bool IsAlive
		{
			get
			{
				return this.Ping();
			}
		}

		internal bool IsConnected
		{
			get
			{
				if (this._readyState == WebSocketState.Open)
				{
					return true;
				}
				return this._readyState == WebSocketState.Closing;
			}
		}

		public bool IsSecure
		{
			get
			{
				return this._secure;
			}
		}

		public Logger Log
		{
			get
			{
				return this._logger;
			}
			internal set
			{
				this._logger = value;
			}
		}

		public string Origin
		{
			get
			{
				return this._origin;
			}
			set
			{
				string str;
				Uri uri;
				lock (this._forState)
				{
					if (!this.checkIfAvailable(true, false, true, false, false, true, out str))
					{
						this._logger.Error(str);
						this.error("An error has occurred in setting the origin.", null);
					}
					else if (value.IsNullOrEmpty())
					{
						this._origin = value;
					}
					else if (!Uri.TryCreate(value, UriKind.Absolute, out uri) || (int)uri.Segments.Length > 1)
					{
						this._logger.Error("The syntax of an origin must be '<scheme>://<host>[:<port>]'.");
						this.error("An error has occurred in setting the origin.", null);
					}
					else
					{
						this._origin = value.TrimEnd(new char[] { '/' });
					}
				}
			}
		}

		public string Protocol
		{
			get
			{
				return this._protocol ?? string.Empty;
			}
			internal set
			{
				this._protocol = value;
			}
		}

		public WebSocketState ReadyState
		{
			get
			{
				return (WebSocketState)this._readyState;
			}
		}

		public ClientSslConfiguration SslConfiguration
		{
			get
			{
				if (!this._client)
				{
					return null;
				}
				ClientSslConfiguration clientSslConfiguration = this._sslConfig;
				if (clientSslConfiguration == null)
				{
					ClientSslConfiguration clientSslConfiguration1 = new ClientSslConfiguration(this._uri.DnsSafeHost);
					ClientSslConfiguration clientSslConfiguration2 = clientSslConfiguration1;
					this._sslConfig = clientSslConfiguration1;
					clientSslConfiguration = clientSslConfiguration2;
				}
				return clientSslConfiguration;
			}
			set
			{
				string str;
				lock (this._forState)
				{
					if (this.checkIfAvailable(true, false, true, false, false, true, out str))
					{
						this._sslConfig = value;
					}
					else
					{
						this._logger.Error(str);
						this.error("An error has occurred in setting the ssl configuration.", null);
					}
				}
			}
		}

		public Uri Url
		{
			get
			{
				if (this._client)
				{
					return this._uri;
				}
				return this._context.RequestUri;
			}
		}

		public TimeSpan WaitTime
		{
			get
			{
				return this._waitTime;
			}
			set
			{
				string str;
				lock (this._forState)
				{
					if (!this.checkIfAvailable(true, true, true, false, false, true, out str) || !value.CheckWaitTime(out str))
					{
						this._logger.Error(str);
						this.error("An error has occurred in setting the wait time.", null);
					}
					else
					{
						this._waitTime = value;
					}
				}
			}
		}

		static WebSocket()
		{
			WebSocket.EmptyBytes = new byte[0];
			WebSocket.FragmentLength = 1016;
			WebSocket.RandomNumber = new RNGCryptoServiceProvider();
		}

		internal WebSocket(HttpListenerWebSocketContext context, string protocol)
		{
			this._context = context;
			this._protocol = protocol;
			this._closeContext = new Action(context.Close);
			this._logger = context.Log;
			this._message = new Action<MessageEventArgs>(this.messages);
			this._secure = context.IsSecureConnection;
			this._stream = context.Stream;
			this._waitTime = TimeSpan.FromSeconds(1);
			this.init();
		}

		internal WebSocket(TcpListenerWebSocketContext context, string protocol)
		{
			this._context = context;
			this._protocol = protocol;
			this._closeContext = new Action(context.Close);
			this._logger = context.Log;
			this._message = new Action<MessageEventArgs>(this.messages);
			this._secure = context.IsSecureConnection;
			this._stream = context.Stream;
			this._waitTime = TimeSpan.FromSeconds(1);
			this.init();
		}

		public WebSocket(string url, params string[] protocols)
		{
			string str;
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			if (url.Length == 0)
			{
				throw new ArgumentException("An empty string.", "url");
			}
			if (!url.TryCreateWebSocketUri(out this._uri, out str))
			{
				throw new ArgumentException(str, "url");
			}
			if (protocols != null && protocols.Length != 0)
			{
				str = protocols.CheckIfValidProtocols();
				if (str != null)
				{
					throw new ArgumentException(str, "protocols");
				}
				this._protocols = protocols;
			}
			this._base64Key = WebSocket.CreateBase64Key();
			this._client = true;
			this._logger = new Logger();
			this._message = new Action<MessageEventArgs>(this.messagec);
			this._secure = this._uri.Scheme == "wss";
			this._waitTime = TimeSpan.FromSeconds(5);
			this.init();
		}

		private bool accept()
		{
			string str;
			bool flag;
			lock (this._forState)
			{
				if (this.checkIfAvailable(true, false, false, false, out str))
				{
					try
					{
						if (this.acceptHandshake())
						{
							this._readyState = WebSocketState.Open;
						}
						else
						{
							flag = false;
							return flag;
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						this._logger.Fatal(exception.ToString());
						this.fatal("An exception has occurred while accepting.", exception);
						flag = false;
						return flag;
					}
					flag = true;
				}
				else
				{
					this._logger.Error(str);
					this.error("An error has occurred in accepting.", null);
					flag = false;
				}
			}
			return flag;
		}

		public void Accept()
		{
			string str;
			if (this.checkIfAvailable(false, true, true, false, false, false, out str))
			{
				if (this.accept())
				{
					this.open();
				}
				return;
			}
			this._logger.Error(str);
			this.error("An error has occurred in accepting.", null);
		}

		public void AcceptAsync()
		{
			string str;
			if (!this.checkIfAvailable(false, true, true, false, false, false, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in accepting.", null);
				return;
			}
			Func<bool> func = new Func<bool>(this.accept);
			func.BeginInvoke((IAsyncResult ar) => {
				if (func.EndInvoke(ar))
				{
					this.open();
				}
			}, null);
		}

		private bool acceptHandshake()
		{
			string str;
			this._logger.Debug(string.Format("A request from {0}:\n{1}", this._context.UserEndPoint, this._context));
			if (!this.checkHandshakeRequest(this._context, out str))
			{
				this.sendHttpResponse(this.createHandshakeFailureResponse(HttpStatusCode.BadRequest));
				this._logger.Fatal(str);
				this.fatal("An error has occurred while accepting.", CloseStatusCode.ProtocolError);
				return false;
			}
			if (!this.customCheckHandshakeRequest(this._context, out str))
			{
				this.sendHttpResponse(this.createHandshakeFailureResponse(HttpStatusCode.BadRequest));
				this._logger.Fatal(str);
				this.fatal("An error has occurred while accepting.", CloseStatusCode.PolicyViolation);
				return false;
			}
			this._base64Key = this._context.Headers["Sec-WebSocket-Key"];
			if (this._protocol != null)
			{
				this.processSecWebSocketProtocolHeader(this._context.SecWebSocketProtocols);
			}
			if (!this._ignoreExtensions)
			{
				this.processSecWebSocketExtensionsClientHeader(this._context.Headers["Sec-WebSocket-Extensions"]);
			}
			return this.sendHttpResponse(this.createHandshakeResponse());
		}

		private bool checkHandshakeRequest(WebSocketContext context, out string message)
		{
			message = null;
			if (context.RequestUri == null)
			{
				message = "Specifies an invalid Request-URI.";
				return false;
			}
			if (!context.IsWebSocketRequest)
			{
				message = "Not a WebSocket handshake request.";
				return false;
			}
			NameValueCollection headers = context.Headers;
			if (!this.validateSecWebSocketKeyHeader(headers["Sec-WebSocket-Key"]))
			{
				message = "Includes no Sec-WebSocket-Key header, or it has an invalid value.";
				return false;
			}
			if (!this.validateSecWebSocketVersionClientHeader(headers["Sec-WebSocket-Version"]))
			{
				message = "Includes no Sec-WebSocket-Version header, or it has an invalid value.";
				return false;
			}
			if (!this.validateSecWebSocketProtocolClientHeader(headers["Sec-WebSocket-Protocol"]))
			{
				message = "Includes an invalid Sec-WebSocket-Protocol header.";
				return false;
			}
			if (this._ignoreExtensions || this.validateSecWebSocketExtensionsClientHeader(headers["Sec-WebSocket-Extensions"]))
			{
				return true;
			}
			message = "Includes an invalid Sec-WebSocket-Extensions header.";
			return false;
		}

		private bool checkHandshakeResponse(HttpResponse response, out string message)
		{
			message = null;
			if (response.IsRedirect)
			{
				message = "Indicates the redirection.";
				return false;
			}
			if (response.IsUnauthorized)
			{
				message = "Requires the authentication.";
				return false;
			}
			if (!response.IsWebSocketResponse)
			{
				message = "Not a WebSocket handshake response.";
				return false;
			}
			NameValueCollection headers = response.Headers;
			if (!this.validateSecWebSocketAcceptHeader(headers["Sec-WebSocket-Accept"]))
			{
				message = "Includes no Sec-WebSocket-Accept header, or it has an invalid value.";
				return false;
			}
			if (!this.validateSecWebSocketProtocolServerHeader(headers["Sec-WebSocket-Protocol"]))
			{
				message = "Includes no Sec-WebSocket-Protocol header, or it has an invalid value.";
				return false;
			}
			if (!this.validateSecWebSocketExtensionsServerHeader(headers["Sec-WebSocket-Extensions"]))
			{
				message = "Includes an invalid Sec-WebSocket-Extensions header.";
				return false;
			}
			if (this.validateSecWebSocketVersionServerHeader(headers["Sec-WebSocket-Version"]))
			{
				return true;
			}
			message = "Includes an invalid Sec-WebSocket-Version header.";
			return false;
		}

		private bool checkIfAvailable(bool connecting, bool open, bool closing, bool closed, out string message)
		{
			message = null;
			if (!connecting && this._readyState == WebSocketState.Connecting)
			{
				message = "This operation is not available in: connecting";
				return false;
			}
			if (!open && this._readyState == WebSocketState.Open)
			{
				message = "This operation is not available in: open";
				return false;
			}
			if (!closing && this._readyState == WebSocketState.Closing)
			{
				message = "This operation is not available in: closing";
				return false;
			}
			if (closed || this._readyState != WebSocketState.Closed)
			{
				return true;
			}
			message = "This operation is not available in: closed";
			return false;
		}

		private bool checkIfAvailable(bool client, bool server, bool connecting, bool open, bool closing, bool closed, out string message)
		{
			message = null;
			if (!client && this._client)
			{
				message = "This operation is not available in: client";
				return false;
			}
			if (!server && !this._client)
			{
				message = "This operation is not available in: server";
				return false;
			}
			return this.checkIfAvailable(connecting, open, closing, closed, out message);
		}

		internal static bool CheckParametersForClose(ushort code, string reason, bool client, out string message)
		{
			message = null;
			if (!code.IsCloseStatusCode())
			{
				message = "'code' is an invalid status code.";
				return false;
			}
			if (code == 1005 && !reason.IsNullOrEmpty())
			{
				message = "'code' cannot have a reason.";
				return false;
			}
			if (code == 1010 && !client)
			{
				message = "'code' cannot be used by a server.";
				return false;
			}
			if (code == 1011 & client)
			{
				message = "'code' cannot be used by a client.";
				return false;
			}
			if (reason.IsNullOrEmpty() || (int)reason.UTF8Encode().Length <= 123)
			{
				return true;
			}
			message = "The size of 'reason' is greater than the allowable max size.";
			return false;
		}

		internal static bool CheckParametersForClose(CloseStatusCode code, string reason, bool client, out string message)
		{
			message = null;
			if (code == CloseStatusCode.NoStatus && !reason.IsNullOrEmpty())
			{
				message = "'code' cannot have a reason.";
				return false;
			}
			if (code == CloseStatusCode.MandatoryExtension && !client)
			{
				message = "'code' cannot be used by a server.";
				return false;
			}
			if (code == CloseStatusCode.ServerError & client)
			{
				message = "'code' cannot be used by a client.";
				return false;
			}
			if (reason.IsNullOrEmpty() || (int)reason.UTF8Encode().Length <= 123)
			{
				return true;
			}
			message = "The size of 'reason' is greater than the allowable max size.";
			return false;
		}

		private static bool checkParametersForSetCredentials(string username, string password, out string message)
		{
			message = null;
			if (username.IsNullOrEmpty())
			{
				return true;
			}
			if (username.Contains(new char[] { ':' }) || !username.IsText())
			{
				message = "'username' contains an invalid character.";
				return false;
			}
			if (password.IsNullOrEmpty())
			{
				return true;
			}
			if (password.IsText())
			{
				return true;
			}
			message = "'password' contains an invalid character.";
			return false;
		}

		private static bool checkParametersForSetProxy(string url, string username, string password, out string message)
		{
			Uri uri;
			message = null;
			if (url.IsNullOrEmpty())
			{
				return true;
			}
			if (!Uri.TryCreate(url, UriKind.Absolute, out uri) || uri.Scheme != "http" || (int)uri.Segments.Length > 1)
			{
				message = "'url' is an invalid URL.";
				return false;
			}
			if (username.IsNullOrEmpty())
			{
				return true;
			}
			if (username.Contains(new char[] { ':' }) || !username.IsText())
			{
				message = "'username' contains an invalid character.";
				return false;
			}
			if (password.IsNullOrEmpty())
			{
				return true;
			}
			if (password.IsText())
			{
				return true;
			}
			message = "'password' contains an invalid character.";
			return false;
		}

		internal static string CheckPingParameter(string message, out byte[] bytes)
		{
			bytes = message.UTF8Encode();
			if ((int)bytes.Length <= 125)
			{
				return null;
			}
			return "A message has greater than the allowable max size.";
		}

		private bool checkReceivedFrame(WebSocketFrame frame, out string message)
		{
			message = null;
			bool isMasked = frame.IsMasked;
			if (this._client & isMasked)
			{
				message = "A frame from the server is masked.";
				return false;
			}
			if (!this._client && !isMasked)
			{
				message = "A frame from a client is not masked.";
				return false;
			}
			if (this._inContinuation && frame.IsData)
			{
				message = "A data frame has been received while receiving continuation frames.";
				return false;
			}
			if (frame.IsCompressed && this._compression == CompressionMethod.None)
			{
				message = "A compressed frame has been received without any agreement for it.";
				return false;
			}
			if (frame.Rsv2 == Rsv.On)
			{
				message = "The RSV2 of a frame is non-zero without any negotiation for it.";
				return false;
			}
			if (frame.Rsv3 != Rsv.On)
			{
				return true;
			}
			message = "The RSV3 of a frame is non-zero without any negotiation for it.";
			return false;
		}

		internal static string CheckSendParameter(byte[] data)
		{
			if (data != null)
			{
				return null;
			}
			return "'data' is null.";
		}

		internal static string CheckSendParameter(FileInfo file)
		{
			if (file != null)
			{
				return null;
			}
			return "'file' is null.";
		}

		internal static string CheckSendParameter(string data)
		{
			if (data != null)
			{
				return null;
			}
			return "'data' is null.";
		}

		internal static string CheckSendParameters(Stream stream, int length)
		{
			if (stream == null)
			{
				return "'stream' is null.";
			}
			if (!stream.CanRead)
			{
				return "'stream' cannot be read.";
			}
			if (length >= 1)
			{
				return null;
			}
			return "'length' is less than 1.";
		}

		private void close(ushort code, string reason)
		{
			if (code == 1005)
			{
				this.close(new CloseEventArgs(), true, true, false);
				return;
			}
			bool flag = !code.IsReserved();
			this.close(new CloseEventArgs(code, reason), flag, flag, false);
		}

		private void close(CloseEventArgs e, bool send, bool receive, bool received)
		{
			byte[] array;
			lock (this._forState)
			{
				if (this._readyState == WebSocketState.Closing)
				{
					this._logger.Info("The closing is already in progress.");
					return;
				}
				else if (this._readyState != WebSocketState.Closed)
				{
					send = (!send ? false : this._readyState == WebSocketState.Open);
					receive = receive & send;
					this._readyState = WebSocketState.Closing;
				}
				else
				{
					this._logger.Info("The connection has been closed.");
					return;
				}
			}
			this._logger.Trace("Begin closing the connection.");
			if (send)
			{
				array = WebSocketFrame.CreateCloseFrame(e.PayloadData, this._client).ToArray();
			}
			else
			{
				array = null;
			}
			e.WasClean = this.closeHandshake(array, receive, received);
			this.releaseResources();
			this._logger.Trace("End closing the connection.");
			this._readyState = WebSocketState.Closed;
			try
			{
				this.OnClose.Emit<CloseEventArgs>(this, e);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this._logger.Error(exception.ToString());
				this.error("An exception has occurred during the OnClose event.", exception);
			}
		}

		internal void Close(HttpResponse response)
		{
			this._readyState = WebSocketState.Closing;
			this.sendHttpResponse(response);
			this.releaseServerResources();
			this._readyState = WebSocketState.Closed;
		}

		internal void Close(HttpStatusCode code)
		{
			this.Close(this.createHandshakeFailureResponse(code));
		}

		internal void Close(CloseEventArgs e, byte[] frameAsBytes, bool receive)
		{
			lock (this._forState)
			{
				if (this._readyState == WebSocketState.Closing)
				{
					this._logger.Info("The closing is already in progress.");
					return;
				}
				else if (this._readyState != WebSocketState.Closed)
				{
					this._readyState = WebSocketState.Closing;
				}
				else
				{
					this._logger.Info("The connection has been closed.");
					return;
				}
			}
			e.WasClean = this.closeHandshake(frameAsBytes, receive, false);
			this.releaseServerResources();
			this.releaseCommonResources();
			this._readyState = WebSocketState.Closed;
			try
			{
				this.OnClose.Emit<CloseEventArgs>(this, e);
			}
			catch (Exception exception)
			{
				this._logger.Error(exception.ToString());
			}
		}

		public void Close()
		{
			string str;
			if (this.checkIfAvailable(true, true, false, false, out str))
			{
				this.close(new CloseEventArgs(), true, true, false);
				return;
			}
			this._logger.Error(str);
			this.error("An error has occurred in closing the connection.", null);
		}

		public void Close(ushort code)
		{
			string str;
			if (!this.checkIfAvailable(true, true, false, false, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
				return;
			}
			if (WebSocket.CheckParametersForClose(code, null, this._client, out str))
			{
				this.close(code, null);
				return;
			}
			this._logger.Error(str);
			this.error("An error has occurred in closing the connection.", null);
		}

		public void Close(CloseStatusCode code)
		{
			string str;
			if (!this.checkIfAvailable(true, true, false, false, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
				return;
			}
			if (WebSocket.CheckParametersForClose(code, null, this._client, out str))
			{
				this.close((ushort)code, null);
				return;
			}
			this._logger.Error(str);
			this.error("An error has occurred in closing the connection.", null);
		}

		public void Close(ushort code, string reason)
		{
			string str;
			if (!this.checkIfAvailable(true, true, false, false, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
				return;
			}
			if (WebSocket.CheckParametersForClose(code, reason, this._client, out str))
			{
				this.close(code, reason);
				return;
			}
			this._logger.Error(str);
			this.error("An error has occurred in closing the connection.", null);
		}

		public void Close(CloseStatusCode code, string reason)
		{
			string str;
			if (!this.checkIfAvailable(true, true, false, false, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
				return;
			}
			if (WebSocket.CheckParametersForClose(code, reason, this._client, out str))
			{
				this.close((ushort)code, reason);
				return;
			}
			this._logger.Error(str);
			this.error("An error has occurred in closing the connection.", null);
		}

		private void closeAsync(ushort code, string reason)
		{
			if (code == 1005)
			{
				this.closeAsync(new CloseEventArgs(), true, true, false);
				return;
			}
			bool flag = !code.IsReserved();
			this.closeAsync(new CloseEventArgs(code, reason), flag, flag, false);
		}

		private void closeAsync(CloseEventArgs e, bool send, bool receive, bool received)
		{
			Action<CloseEventArgs, bool, bool, bool> action = new Action<CloseEventArgs, bool, bool, bool>(this.close);
			action.BeginInvoke(e, send, receive, received, (IAsyncResult ar) => action.EndInvoke(ar), null);
		}

		public void CloseAsync()
		{
			string str;
			if (this.checkIfAvailable(true, true, false, false, out str))
			{
				this.closeAsync(new CloseEventArgs(), true, true, false);
				return;
			}
			this._logger.Error(str);
			this.error("An error has occurred in closing the connection.", null);
		}

		public void CloseAsync(ushort code)
		{
			string str;
			if (!this.checkIfAvailable(true, true, false, false, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
				return;
			}
			if (WebSocket.CheckParametersForClose(code, null, this._client, out str))
			{
				this.closeAsync(code, null);
				return;
			}
			this._logger.Error(str);
			this.error("An error has occurred in closing the connection.", null);
		}

		public void CloseAsync(CloseStatusCode code)
		{
			string str;
			if (!this.checkIfAvailable(true, true, false, false, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
				return;
			}
			if (WebSocket.CheckParametersForClose(code, null, this._client, out str))
			{
				this.closeAsync((ushort)code, null);
				return;
			}
			this._logger.Error(str);
			this.error("An error has occurred in closing the connection.", null);
		}

		public void CloseAsync(ushort code, string reason)
		{
			string str;
			if (!this.checkIfAvailable(true, true, false, false, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
				return;
			}
			if (WebSocket.CheckParametersForClose(code, reason, this._client, out str))
			{
				this.closeAsync(code, reason);
				return;
			}
			this._logger.Error(str);
			this.error("An error has occurred in closing the connection.", null);
		}

		public void CloseAsync(CloseStatusCode code, string reason)
		{
			string str;
			if (!this.checkIfAvailable(true, true, false, false, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in closing the connection.", null);
				return;
			}
			if (WebSocket.CheckParametersForClose(code, reason, this._client, out str))
			{
				this.closeAsync((ushort)code, reason);
				return;
			}
			this._logger.Error(str);
			this.error("An error has occurred in closing the connection.", null);
		}

		private bool closeHandshake(byte[] frameAsBytes, bool receive, bool received)
		{
			bool flag;
			bool flag1 = (frameAsBytes == null ? false : this.sendBytes(frameAsBytes));
			if (received)
			{
				flag = true;
			}
			else
			{
				flag = (!(receive & flag1) || this._exitReceiving == null ? false : this._exitReceiving.WaitOne(this._waitTime));
			}
			received = flag;
			bool flag2 = flag1 & received;
			this._logger.Debug(string.Format("Was clean?: {0}\n  sent: {1}\n  received: {2}", flag2, flag1, received));
			return flag2;
		}

		private bool connect()
		{
			string str;
			bool flag;
			lock (this._forState)
			{
				if (this.checkIfAvailable(true, false, false, true, out str))
				{
					try
					{
						this._readyState = WebSocketState.Connecting;
						if (this.doHandshake())
						{
							this._readyState = WebSocketState.Open;
						}
						else
						{
							flag = false;
							return flag;
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						this._logger.Fatal(exception.ToString());
						this.fatal("An exception has occurred while connecting.", exception);
						flag = false;
						return flag;
					}
					flag = true;
				}
				else
				{
					this._logger.Error(str);
					this.error("An error has occurred in connecting.", null);
					flag = false;
				}
			}
			return flag;
		}

		public void Connect()
		{
			string str;
			if (this.checkIfAvailable(true, false, true, false, false, true, out str))
			{
				if (this.connect())
				{
					this.open();
				}
				return;
			}
			this._logger.Error(str);
			this.error("An error has occurred in connecting.", null);
		}

		public void ConnectAsync()
		{
			string str;
			if (!this.checkIfAvailable(true, false, true, false, false, true, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in connecting.", null);
				return;
			}
			Func<bool> func = new Func<bool>(this.connect);
			func.BeginInvoke((IAsyncResult ar) => {
				if (func.EndInvoke(ar))
				{
					this.open();
				}
			}, null);
		}

		internal static string CreateBase64Key()
		{
			byte[] numArray = new byte[16];
			WebSocket.RandomNumber.GetBytes(numArray);
			return Convert.ToBase64String(numArray);
		}

		private string createExtensions()
		{
			StringBuilder stringBuilder = new StringBuilder(80);
			if (this._compression != CompressionMethod.None)
			{
				string extensionString = this._compression.ToExtensionString(new string[] { "server_no_context_takeover", "client_no_context_takeover" });
				stringBuilder.AppendFormat("{0}, ", extensionString);
			}
			int length = stringBuilder.Length;
			if (length <= 2)
			{
				return null;
			}
			stringBuilder.Length = length - 2;
			return stringBuilder.ToString();
		}

		private HttpResponse createHandshakeFailureResponse(HttpStatusCode code)
		{
			HttpResponse httpResponse = HttpResponse.CreateCloseResponse(code);
			httpResponse.Headers["Sec-WebSocket-Version"] = "13";
			return httpResponse;
		}

		private HttpRequest createHandshakeRequest()
		{
			HttpRequest httpRequest = HttpRequest.CreateWebSocketRequest(this._uri);
			NameValueCollection headers = httpRequest.Headers;
			if (!this._origin.IsNullOrEmpty())
			{
				headers["Origin"] = this._origin;
			}
			headers["Sec-WebSocket-Key"] = this._base64Key;
			this._protocolsRequested = this._protocols != null;
			if (this._protocolsRequested)
			{
				headers["Sec-WebSocket-Protocol"] = this._protocols.ToString<string>(", ");
			}
			this._extensionsRequested = this._compression != CompressionMethod.None;
			if (this._extensionsRequested)
			{
				headers["Sec-WebSocket-Extensions"] = this.createExtensions();
			}
			headers["Sec-WebSocket-Version"] = "13";
			AuthenticationResponse authenticationResponse = null;
			if (this._authChallenge != null && this._credentials != null)
			{
				authenticationResponse = new AuthenticationResponse(this._authChallenge, this._credentials, this._nonceCount);
				this._nonceCount = authenticationResponse.NonceCount;
			}
			else if (this._preAuth)
			{
				authenticationResponse = new AuthenticationResponse(this._credentials);
			}
			if (authenticationResponse != null)
			{
				headers["Authorization"] = authenticationResponse.ToString();
			}
			if (this._cookies.Count > 0)
			{
				httpRequest.SetCookies(this._cookies);
			}
			return httpRequest;
		}

		private HttpResponse createHandshakeResponse()
		{
			HttpResponse httpResponse = HttpResponse.CreateWebSocketResponse();
			NameValueCollection headers = httpResponse.Headers;
			headers["Sec-WebSocket-Accept"] = WebSocket.CreateResponseKey(this._base64Key);
			if (this._protocol != null)
			{
				headers["Sec-WebSocket-Protocol"] = this._protocol;
			}
			if (this._extensions != null)
			{
				headers["Sec-WebSocket-Extensions"] = this._extensions;
			}
			if (this._cookies.Count > 0)
			{
				httpResponse.SetCookies(this._cookies);
			}
			return httpResponse;
		}

		internal static string CreateResponseKey(string base64Key)
		{
			StringBuilder stringBuilder = new StringBuilder(base64Key, 64);
			stringBuilder.Append("258EAFA5-E914-47DA-95CA-C5AB0DC85B11");
			return Convert.ToBase64String((new SHA1CryptoServiceProvider()).ComputeHash(stringBuilder.ToString().UTF8Encode()));
		}

		private bool customCheckHandshakeRequest(WebSocketContext context, out string message)
		{
			message = null;
			if (this._handshakeRequestChecker == null)
			{
				return true;
			}
			string str = this._handshakeRequestChecker(context);
			string str1 = str;
			message = str;
			return str1 == null;
		}

		private MessageEventArgs dequeueFromMessageEventQueue()
		{
			MessageEventArgs messageEventArg;
			MessageEventArgs messageEventArg1;
			lock (this._forMessageEventQueue)
			{
				if (this._messageEventQueue.Count > 0)
				{
					messageEventArg1 = this._messageEventQueue.Dequeue();
				}
				else
				{
					messageEventArg1 = null;
				}
				messageEventArg = messageEventArg1;
			}
			return messageEventArg;
		}

		private bool doHandshake()
		{
			string str;
			this.setClientStream();
			HttpResponse httpResponse = this.sendHandshakeRequest();
			if (!this.checkHandshakeResponse(httpResponse, out str))
			{
				this._logger.Fatal(str);
				this.fatal("An error has occurred while connecting.", CloseStatusCode.ProtocolError);
				return false;
			}
			if (this._protocolsRequested)
			{
				this._protocol = httpResponse.Headers["Sec-WebSocket-Protocol"];
			}
			if (this._extensionsRequested)
			{
				this.processSecWebSocketExtensionsServerHeader(httpResponse.Headers["Sec-WebSocket-Extensions"]);
			}
			this.processCookies(httpResponse.Cookies);
			return true;
		}

		private void enqueueToMessageEventQueue(MessageEventArgs e)
		{
			lock (this._forMessageEventQueue)
			{
				this._messageEventQueue.Enqueue(e);
			}
		}

		private void error(string message, Exception exception)
		{
			try
			{
				this.OnError.Emit<WebSocket.ErrorEventArgs>(this, new WebSocket.ErrorEventArgs(message, exception));
			}
			catch (Exception exception1)
			{
				this._logger.Error(exception1.ToString());
			}
		}

		private void fatal(string message, Exception exception)
		{
			this.fatal(message, (exception is WebSocketException ? ((WebSocketException)exception).Code : CloseStatusCode.Abnormal));
		}

		private void fatal(string message, CloseStatusCode code)
		{
			this.close(new CloseEventArgs(code, message), !code.IsReserved(), false, false);
		}

		private void init()
		{
			this._compression = CompressionMethod.None;
			this._cookies = new WebSocket.Net.CookieCollection();
			this._forSend = new object();
			this._forState = new object();
			this._messageEventQueue = new Queue<MessageEventArgs>();
			this._forMessageEventQueue = ((ICollection)this._messageEventQueue).SyncRoot;
			this._readyState = WebSocketState.Connecting;
		}

		internal void InternalAccept()
		{
			try
			{
				if (this.acceptHandshake())
				{
					this._readyState = WebSocketState.Open;
				}
				else
				{
					return;
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this._logger.Fatal(exception.ToString());
				this.fatal("An exception has occurred while accepting.", exception);
				return;
			}
			this.open();
		}

		private void message()
		{
			MessageEventArgs messageEventArg = null;
			lock (this._forMessageEventQueue)
			{
				if (this._inMessage || this._messageEventQueue.Count == 0 || this._readyState != WebSocketState.Open)
				{
					return;
				}
				else
				{
					this._inMessage = true;
					messageEventArg = this._messageEventQueue.Dequeue();
				}
			}
			this._message(messageEventArg);
		}

		private void messagec(MessageEventArgs e)
		{
			while (true)
			{
				try
				{
					this.OnMessage.Emit<MessageEventArgs>(this, e);
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					this._logger.Error(exception.ToString());
					this.error("An exception has occurred during an OnMessage event.", exception);
				}
				lock (this._forMessageEventQueue)
				{
					if (this._messageEventQueue.Count == 0 || this._readyState != WebSocketState.Open)
					{
						this._inMessage = false;
						break;
					}
					else
					{
						e = this._messageEventQueue.Dequeue();
					}
				}
			}
		}

		private void messages(MessageEventArgs e)
		{
			MessageEventArgs messageEventArg = e;
			try
			{
				this.OnMessage.Emit<MessageEventArgs>(this, messageEventArg);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this._logger.Error(exception.ToString());
				this.error("An exception has occurred during an OnMessage event.", exception);
			}
			lock (this._forMessageEventQueue)
			{
				if (this._messageEventQueue.Count == 0 || this._readyState != WebSocketState.Open)
				{
					this._inMessage = false;
					return;
				}
				else
				{
					messageEventArg = this._messageEventQueue.Dequeue();
				}
			}
			ThreadPool.QueueUserWorkItem((object state) => this.messages(messageEventArg));
		}

		private void open()
		{
			this._inMessage = true;
			this.startReceiving();
			try
			{
				this.OnOpen.Emit(this, EventArgs.Empty);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this._logger.Error(exception.ToString());
				this.error("An exception has occurred during the OnOpen event.", exception);
			}
			MessageEventArgs messageEventArg = null;
			lock (this._forMessageEventQueue)
			{
				if (this._messageEventQueue.Count == 0 || this._readyState != WebSocketState.Open)
				{
					this._inMessage = false;
					return;
				}
				else
				{
					messageEventArg = this._messageEventQueue.Dequeue();
				}
			}
			this._message.BeginInvoke(messageEventArg, (IAsyncResult ar) => this._message.EndInvoke(ar), null);
		}

		internal bool Ping(byte[] frameAsBytes, TimeSpan timeout)
		{
			if (this._readyState != WebSocketState.Open)
			{
				return false;
			}
			if (!this.send(frameAsBytes))
			{
				return false;
			}
			AutoResetEvent autoResetEvent = this._receivePong;
			if (autoResetEvent == null)
			{
				return false;
			}
			return autoResetEvent.WaitOne(timeout);
		}

		public bool Ping()
		{
			return this.Ping((this._client ? WebSocketFrame.CreatePingFrame(true).ToArray() : WebSocketFrame.EmptyPingBytes), this._waitTime);
		}

		public bool Ping(string message)
		{
			byte[] numArray;
			if (message == null || message.Length == 0)
			{
				return this.Ping();
			}
			string str = WebSocket.CheckPingParameter(message, out numArray);
			if (str == null)
			{
				return this.Ping(WebSocketFrame.CreatePingFrame(numArray, this._client).ToArray(), this._waitTime);
			}
			this._logger.Error(str);
			this.error("An error has occurred in sending a ping.", null);
			return false;
		}

		private bool processCloseFrame(WebSocketFrame frame)
		{
			PayloadData payloadData = frame.PayloadData;
			this.close(new CloseEventArgs(payloadData), !payloadData.HasReservedCode, false, true);
			return false;
		}

		private void processCookies(WebSocket.Net.CookieCollection cookies)
		{
			if (cookies.Count == 0)
			{
				return;
			}
			this._cookies.SetOrRemove(cookies);
		}

		private bool processDataFrame(WebSocketFrame frame)
		{
			this.enqueueToMessageEventQueue((frame.IsCompressed ? new MessageEventArgs(frame.Opcode, frame.PayloadData.ApplicationData.Decompress(this._compression)) : new MessageEventArgs(frame)));
			return true;
		}

		private bool processFragmentFrame(WebSocketFrame frame)
		{
			byte[] numArray;
			if (!this._inContinuation)
			{
				if (frame.IsContinuation)
				{
					return true;
				}
				this._fragmentsOpcode = frame.Opcode;
				this._fragmentsCompressed = frame.IsCompressed;
				this._fragmentsBuffer = new MemoryStream();
				this._inContinuation = true;
			}
			this._fragmentsBuffer.WriteBytes(frame.PayloadData.ApplicationData, 1024);
			if (frame.IsFinal)
			{
				using (this._fragmentsBuffer)
				{
					numArray = (this._fragmentsCompressed ? this._fragmentsBuffer.DecompressToArray(this._compression) : this._fragmentsBuffer.ToArray());
					this.enqueueToMessageEventQueue(new MessageEventArgs(this._fragmentsOpcode, numArray));
				}
				this._fragmentsBuffer = null;
				this._inContinuation = false;
			}
			return true;
		}

		private bool processPingFrame(WebSocketFrame frame)
		{
			if (this.send((new WebSocketFrame(Opcode.Pong, frame.PayloadData, this._client)).ToArray()))
			{
				this._logger.Trace("Returned a pong.");
			}
			if (this._emitOnPing)
			{
				this.enqueueToMessageEventQueue(new MessageEventArgs(frame));
			}
			return true;
		}

		private bool processPongFrame(WebSocketFrame frame)
		{
			this._receivePong.Set();
			this._logger.Trace("Received a pong.");
			return true;
		}

		private bool processReceivedFrame(WebSocketFrame frame)
		{
			string str;
			if (!this.checkReceivedFrame(frame, out str))
			{
				throw new WebSocketException(CloseStatusCode.ProtocolError, str);
			}
			frame.Unmask();
			if (frame.IsFragment)
			{
				return this.processFragmentFrame(frame);
			}
			if (frame.IsData)
			{
				return this.processDataFrame(frame);
			}
			if (frame.IsPing)
			{
				return this.processPingFrame(frame);
			}
			if (frame.IsPong)
			{
				return this.processPongFrame(frame);
			}
			if (!frame.IsClose)
			{
				return this.processUnsupportedFrame(frame);
			}
			return this.processCloseFrame(frame);
		}

		private void processSecWebSocketExtensionsClientHeader(string value)
		{
			if (value == null)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(80);
			bool flag = false;
			foreach (string str in Ext.SplitHeaderValue(value, new char[] { ',' }))
			{
				string str1 = str.Trim();
				if (flag || !str1.IsCompressionExtension(CompressionMethod.Deflate))
				{
					continue;
				}
				this._compression = CompressionMethod.Deflate;
				stringBuilder.AppendFormat("{0}, ", this._compression.ToExtensionString(new string[] { "client_no_context_takeover", "server_no_context_takeover" }));
				flag = true;
			}
			int length = stringBuilder.Length;
			if (length > 2)
			{
				stringBuilder.Length = length - 2;
				this._extensions = stringBuilder.ToString();
			}
		}

		private void processSecWebSocketExtensionsServerHeader(string value)
		{
			if (value == null)
			{
				this._compression = CompressionMethod.None;
				return;
			}
			this._extensions = value;
		}

		private void processSecWebSocketProtocolHeader(IEnumerable<string> values)
		{
			if (values.Contains<string>((string p) => p == this._protocol))
			{
				return;
			}
			this._protocol = null;
		}

		private bool processUnsupportedFrame(WebSocketFrame frame)
		{
			this._logger.Fatal(string.Concat("An unsupported frame:", frame.PrintToString(false)));
			this.fatal("There is no way to handle it.", CloseStatusCode.PolicyViolation);
			return false;
		}

		private void releaseClientResources()
		{
			if (this._stream != null)
			{
				this._stream.Dispose();
				this._stream = null;
			}
			if (this._tcpClient != null)
			{
				this._tcpClient.Close();
				this._tcpClient = null;
			}
		}

		private void releaseCommonResources()
		{
			if (this._fragmentsBuffer != null)
			{
				this._fragmentsBuffer.Dispose();
				this._fragmentsBuffer = null;
				this._inContinuation = false;
			}
			if (this._receivePong != null)
			{
				this._receivePong.Close();
				this._receivePong = null;
			}
			if (this._exitReceiving != null)
			{
				this._exitReceiving.Close();
				this._exitReceiving = null;
			}
		}

		private void releaseResources()
		{
			if (!this._client)
			{
				this.releaseServerResources();
			}
			else
			{
				this.releaseClientResources();
			}
			this.releaseCommonResources();
		}

		private void releaseServerResources()
		{
			if (this._closeContext == null)
			{
				return;
			}
			this._closeContext();
			this._closeContext = null;
			this._stream = null;
			this._context = null;
		}

		private bool send(byte[] frameAsBytes)
		{
			bool flag;
			lock (this._forState)
			{
				if (this._readyState == WebSocketState.Open)
				{
					flag = this.sendBytes(frameAsBytes);
				}
				else
				{
					this._logger.Error("The sending has been interrupted.");
					flag = false;
				}
			}
			return flag;
		}

		private bool send(Opcode opcode, Stream stream)
		{
			bool flag;
			lock (this._forSend)
			{
				Stream stream1 = stream;
				bool flag1 = false;
				bool flag2 = false;
				try
				{
					try
					{
						if (this._compression != CompressionMethod.None)
						{
							stream = stream.Compress(this._compression);
							flag1 = true;
						}
						flag2 = this.send(opcode, stream, flag1);
						if (!flag2)
						{
							this.error("The sending has been interrupted.", null);
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						this._logger.Error(exception.ToString());
						this.error("An exception has occurred while sending data.", exception);
					}
				}
				finally
				{
					if (flag1)
					{
						stream.Dispose();
					}
					stream1.Dispose();
				}
				flag = flag2;
			}
			return flag;
		}

		private bool send(Opcode opcode, Stream stream, bool compressed)
		{
			long length = stream.Length;
			if (length == 0)
			{
				return this.send(Fin.Final, opcode, WebSocket.EmptyBytes, compressed);
			}
			long fragmentLength = length / (long)WebSocket.FragmentLength;
			int num = (int)(length % (long)WebSocket.FragmentLength);
			byte[] numArray = null;
			if (fragmentLength == 0)
			{
				numArray = new byte[num];
				if (stream.Read(numArray, 0, num) != num)
				{
					return false;
				}
				return this.send(Fin.Final, opcode, numArray, compressed);
			}
			numArray = new byte[WebSocket.FragmentLength];
			if (fragmentLength == (long)1 && num == 0)
			{
				if (stream.Read(numArray, 0, WebSocket.FragmentLength) != WebSocket.FragmentLength)
				{
					return false;
				}
				return this.send(Fin.Final, opcode, numArray, compressed);
			}
			if (stream.Read(numArray, 0, WebSocket.FragmentLength) != WebSocket.FragmentLength || !this.send(Fin.More, opcode, numArray, compressed))
			{
				return false;
			}
			long num1 = (num == 0 ? fragmentLength - (long)2 : fragmentLength - (long)1);
			for (long i = (long)0; i < num1; i = i + (long)1)
			{
				if (stream.Read(numArray, 0, WebSocket.FragmentLength) != WebSocket.FragmentLength || !this.send(Fin.More, Opcode.Cont, numArray, compressed))
				{
					return false;
				}
			}
			if (num != 0)
			{
				numArray = new byte[num];
			}
			else
			{
				num = WebSocket.FragmentLength;
			}
			if (stream.Read(numArray, 0, num) != num)
			{
				return false;
			}
			return this.send(Fin.Final, Opcode.Cont, numArray, compressed);
		}

		private bool send(Fin fin, Opcode opcode, byte[] data, bool compressed)
		{
			bool flag;
			lock (this._forState)
			{
				if (this._readyState == WebSocketState.Open)
				{
					flag = this.sendBytes((new WebSocketFrame(fin, opcode, data, compressed, this._client)).ToArray());
				}
				else
				{
					this._logger.Error("The sending has been interrupted.");
					flag = false;
				}
			}
			return flag;
		}

		internal void Send(Opcode opcode, byte[] data, Dictionary<CompressionMethod, byte[]> cache)
		{
			byte[] array;
			lock (this._forSend)
			{
				lock (this._forState)
				{
					if (this._readyState == WebSocketState.Open)
					{
						try
						{
							if (!cache.TryGetValue(this._compression, out array))
							{
								array = (new WebSocketFrame(Fin.Final, opcode, data.Compress(this._compression), this._compression != CompressionMethod.None, false)).ToArray();
								cache.Add(this._compression, array);
							}
							this.sendBytes(array);
						}
						catch (Exception exception)
						{
							this._logger.Error(exception.ToString());
						}
					}
					else
					{
						this._logger.Error("The sending has been interrupted.");
					}
				}
			}
		}

		internal void Send(Opcode opcode, Stream stream, Dictionary<CompressionMethod, Stream> cache)
		{
			Stream stream1;
			lock (this._forSend)
			{
				try
				{
					if (cache.TryGetValue(this._compression, out stream1))
					{
						stream1.Position = (long)0;
					}
					else
					{
						stream1 = stream.Compress(this._compression);
						cache.Add(this._compression, stream1);
					}
					this.send(opcode, stream1, this._compression != CompressionMethod.None);
				}
				catch (Exception exception)
				{
					this._logger.Error(exception.ToString());
				}
			}
		}

		public void Send(byte[] data)
		{
			string str = ((WebSocketState)this._readyState).CheckIfAvailable(false, true, false, false) ?? WebSocket.CheckSendParameter(data);
			if (str == null)
			{
				this.send(Opcode.Binary, new MemoryStream(data));
				return;
			}
			this._logger.Error(str);
			this.error("An error has occurred in sending data.", null);
		}

		public void Send(FileInfo file)
		{
			string str = ((WebSocketState)this._readyState).CheckIfAvailable(false, true, false, false) ?? WebSocket.CheckSendParameter(file);
			if (str == null)
			{
				this.send(Opcode.Binary, file.OpenRead());
				return;
			}
			this._logger.Error(str);
			this.error("An error has occurred in sending data.", null);
		}

		public void Send(string data)
		{
			string str = ((WebSocketState)this._readyState).CheckIfAvailable(false, true, false, false) ?? WebSocket.CheckSendParameter(data);
			if (str == null)
			{
				this.send(Opcode.Text, new MemoryStream(data.UTF8Encode()));
				return;
			}
			this._logger.Error(str);
			this.error("An error has occurred in sending data.", null);
		}

		private void sendAsync(Opcode opcode, Stream stream, Action<bool> completed)
		{
			Func<Opcode, Stream, bool> func = new Func<Opcode, Stream, bool>(this.send);
			func.BeginInvoke(opcode, stream, (IAsyncResult ar) => {
				try
				{
					bool flag = func.EndInvoke(ar);
					if (completed != null)
					{
						completed(flag);
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					this._logger.Error(exception.ToString());
					this.error("An exception has occurred during a send callback.", exception);
				}
			}, null);
		}

		public void SendAsync(byte[] data, Action<bool> completed)
		{
			string str = ((WebSocketState)this._readyState).CheckIfAvailable(false, true, false, false) ?? WebSocket.CheckSendParameter(data);
			if (str == null)
			{
				this.sendAsync(Opcode.Binary, new MemoryStream(data), completed);
				return;
			}
			this._logger.Error(str);
			this.error("An error has occurred in sending data.", null);
		}

		public void SendAsync(FileInfo file, Action<bool> completed)
		{
			string str = ((WebSocketState)this._readyState).CheckIfAvailable(false, true, false, false) ?? WebSocket.CheckSendParameter(file);
			if (str == null)
			{
				this.sendAsync(Opcode.Binary, file.OpenRead(), completed);
				return;
			}
			this._logger.Error(str);
			this.error("An error has occurred in sending data.", null);
		}

		public void SendAsync(string data, Action<bool> completed)
		{
			string str = ((WebSocketState)this._readyState).CheckIfAvailable(false, true, false, false) ?? WebSocket.CheckSendParameter(data);
			if (str == null)
			{
				this.sendAsync(Opcode.Text, new MemoryStream(data.UTF8Encode()), completed);
				return;
			}
			this._logger.Error(str);
			this.error("An error has occurred in sending data.", null);
		}

		public void SendAsync(Stream stream, int length, Action<bool> completed)
		{
			string str = ((WebSocketState)this._readyState).CheckIfAvailable(false, true, false, false) ?? WebSocket.CheckSendParameters(stream, length);
			if (str != null)
			{
				this._logger.Error(str);
				this.error("An error has occurred in sending data.", null);
				return;
			}
			stream.ReadBytesAsync(length, (byte[] data) => {
				int num = (int)data.Length;
				if (num == 0)
				{
					this._logger.Error("The data cannot be read from 'stream'.");
					this.error("An error has occurred in sending data.", null);
					return;
				}
				if (num < length)
				{
					this._logger.Warn(string.Format("The length of the data is less than 'length':\n  expected: {0}\n  actual: {1}", length, num));
				}
				bool flag = this.send(Opcode.Binary, new MemoryStream(data));
				if (completed != null)
				{
					completed(flag);
				}
			}, (Exception ex) => {
				this._logger.Error(ex.ToString());
				this.error("An exception has occurred while sending data.", ex);
			});
		}

		private bool sendBytes(byte[] bytes)
		{
			bool flag;
			try
			{
				this._stream.Write(bytes, 0, (int)bytes.Length);
				flag = true;
			}
			catch (Exception exception)
			{
				this._logger.Error(exception.ToString());
				flag = false;
			}
			return flag;
		}

		private HttpResponse sendHandshakeRequest()
		{
			Uri uri;
			string str;
			HttpRequest httpRequest = this.createHandshakeRequest();
			HttpResponse httpResponse = this.sendHttpRequest(httpRequest, 90000);
			if (httpResponse.IsUnauthorized)
			{
				string item = httpResponse.Headers["WWW-Authenticate"];
				this._logger.Warn(string.Format("Received an authentication requirement for '{0}'.", item));
				if (item.IsNullOrEmpty())
				{
					this._logger.Error("No authentication challenge is specified.");
					return httpResponse;
				}
				this._authChallenge = AuthenticationChallenge.Parse(item);
				if (this._authChallenge == null)
				{
					this._logger.Error("An invalid authentication challenge is specified.");
					return httpResponse;
				}
				if (this._credentials != null && (!this._preAuth || this._authChallenge.Scheme == AuthenticationSchemes.Digest))
				{
					if (httpResponse.HasConnectionClose)
					{
						this.releaseClientResources();
						this.setClientStream();
					}
					AuthenticationResponse authenticationResponse = new AuthenticationResponse(this._authChallenge, this._credentials, this._nonceCount);
					this._nonceCount = authenticationResponse.NonceCount;
					httpRequest.Headers["Authorization"] = authenticationResponse.ToString();
					httpResponse = this.sendHttpRequest(httpRequest, 15000);
				}
			}
			if (httpResponse.IsRedirect)
			{
				string item1 = httpResponse.Headers["Location"];
				this._logger.Warn(string.Format("Received a redirection to '{0}'.", item1));
				if (this._enableRedirection)
				{
					if (item1.IsNullOrEmpty())
					{
						this._logger.Error("No url to redirect is located.");
						return httpResponse;
					}
					if (!item1.TryCreateWebSocketUri(out uri, out str))
					{
						this._logger.Error(string.Concat("An invalid url to redirect is located: ", str));
						return httpResponse;
					}
					this.releaseClientResources();
					this._uri = uri;
					this._secure = uri.Scheme == "wss";
					this.setClientStream();
					return this.sendHandshakeRequest();
				}
			}
			return httpResponse;
		}

		private HttpResponse sendHttpRequest(HttpRequest request, int millisecondsTimeout)
		{
			this._logger.Debug(string.Concat("A request to the server:\n", request.ToString()));
			HttpResponse response = request.GetResponse(this._stream, millisecondsTimeout);
			this._logger.Debug(string.Concat("A response to this request:\n", response.ToString()));
			return response;
		}

		private bool sendHttpResponse(HttpResponse response)
		{
			this._logger.Debug(string.Concat("A response to this request:\n", response.ToString()));
			return this.sendBytes(response.ToByteArray());
		}

		private void sendProxyConnectRequest()
		{
			HttpRequest str = HttpRequest.CreateConnectRequest(this._uri);
			HttpResponse httpResponse = this.sendHttpRequest(str, 90000);
			if (httpResponse.IsProxyAuthenticationRequired)
			{
				string item = httpResponse.Headers["Proxy-Authenticate"];
				this._logger.Warn(string.Format("Received a proxy authentication requirement for '{0}'.", item));
				if (item.IsNullOrEmpty())
				{
					throw new WebSocketException("No proxy authentication challenge is specified.");
				}
				AuthenticationChallenge authenticationChallenge = AuthenticationChallenge.Parse(item);
				if (authenticationChallenge == null)
				{
					throw new WebSocketException("An invalid proxy authentication challenge is specified.");
				}
				if (this._proxyCredentials != null)
				{
					if (httpResponse.HasConnectionClose)
					{
						this.releaseClientResources();
						this._tcpClient = new TcpClient(this._proxyUri.DnsSafeHost, this._proxyUri.Port);
						this._stream = this._tcpClient.GetStream();
					}
					AuthenticationResponse authenticationResponse = new AuthenticationResponse(authenticationChallenge, this._proxyCredentials, 0);
					str.Headers["Proxy-Authorization"] = authenticationResponse.ToString();
					httpResponse = this.sendHttpRequest(str, 15000);
				}
				if (httpResponse.IsProxyAuthenticationRequired)
				{
					throw new WebSocketException("A proxy authentication is required.");
				}
			}
			if (httpResponse.StatusCode[0] != '2')
			{
				throw new WebSocketException("The proxy has failed a connection to the requested host and port.");
			}
		}

		private void setClientStream()
		{
			if (this._proxyUri == null)
			{
				this._tcpClient = new TcpClient(this._uri.DnsSafeHost, this._uri.Port);
				this._stream = this._tcpClient.GetStream();
			}
			else
			{
				this._tcpClient = new TcpClient(this._proxyUri.DnsSafeHost, this._proxyUri.Port);
				this._stream = this._tcpClient.GetStream();
				this.sendProxyConnectRequest();
			}
			if (this._secure)
			{
				ClientSslConfiguration sslConfiguration = this.SslConfiguration;
				string targetHost = sslConfiguration.TargetHost;
				if (targetHost != this._uri.DnsSafeHost)
				{
					throw new WebSocketException(CloseStatusCode.TlsHandshakeFailure, "An invalid host name is specified.");
				}
				try
				{
					SslStream sslStream = new SslStream(this._stream, false, sslConfiguration.ServerCertificateValidationCallback, sslConfiguration.ClientCertificateSelectionCallback);
					sslStream.AuthenticateAsClient(targetHost, sslConfiguration.ClientCertificates, sslConfiguration.EnabledSslProtocols, sslConfiguration.CheckCertificateRevocation);
					this._stream = sslStream;
				}
				catch (Exception exception)
				{
					throw new WebSocketException(CloseStatusCode.TlsHandshakeFailure, exception);
				}
			}
		}

		public void SetCookie(Cookie cookie)
		{
			string str;
			if (!this.checkIfAvailable(true, false, true, false, false, true, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in setting a cookie.", null);
				return;
			}
			if (cookie == null)
			{
				this._logger.Error("'cookie' is null.");
				this.error("An error has occurred in setting a cookie.", null);
				return;
			}
			lock (this._forState)
			{
				if (this.checkIfAvailable(true, false, false, true, out str))
				{
					lock (this._cookies.SyncRoot)
					{
						this._cookies.SetOrRemove(cookie);
					}
				}
				else
				{
					this._logger.Error(str);
					this.error("An error has occurred in setting a cookie.", null);
				}
			}
		}

		public void SetCredentials(string username, string password, bool preAuth)
		{
			string str;
			if (!this.checkIfAvailable(true, false, true, false, false, true, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in setting the credentials.", null);
				return;
			}
			if (!WebSocket.checkParametersForSetCredentials(username, password, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in setting the credentials.", null);
				return;
			}
			lock (this._forState)
			{
				if (!this.checkIfAvailable(true, false, false, true, out str))
				{
					this._logger.Error(str);
					this.error("An error has occurred in setting the credentials.", null);
				}
				else if (!username.IsNullOrEmpty())
				{
					this._credentials = new NetworkCredential(username, password, this._uri.PathAndQuery, new string[0]);
					this._preAuth = preAuth;
				}
				else
				{
					this._logger.Warn("The credentials are initialized.");
					this._credentials = null;
					this._preAuth = false;
				}
			}
		}

		public void SetProxy(string url, string username, string password)
		{
			string str;
			if (!this.checkIfAvailable(true, false, true, false, false, true, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in setting the proxy.", null);
				return;
			}
			if (!WebSocket.checkParametersForSetProxy(url, username, password, out str))
			{
				this._logger.Error(str);
				this.error("An error has occurred in setting the proxy.", null);
				return;
			}
			lock (this._forState)
			{
				if (!this.checkIfAvailable(true, false, false, true, out str))
				{
					this._logger.Error(str);
					this.error("An error has occurred in setting the proxy.", null);
				}
				else if (!url.IsNullOrEmpty())
				{
					this._proxyUri = new Uri(url);
					if (!username.IsNullOrEmpty())
					{
						this._proxyCredentials = new NetworkCredential(username, password, string.Format("{0}:{1}", this._uri.DnsSafeHost, this._uri.Port), new string[0]);
					}
					else
					{
						this._logger.Warn("The credentials for the proxy are initialized.");
						this._proxyCredentials = null;
					}
				}
				else
				{
					this._logger.Warn("The url and credentials for the proxy are initialized.");
					this._proxyUri = null;
					this._proxyCredentials = null;
				}
			}
		}

		private void startReceiving()
		{
			Action<WebSocketFrame> action2 = null;
			if (this._messageEventQueue.Count > 0)
			{
				this._messageEventQueue.Clear();
			}
			this._exitReceiving = new AutoResetEvent(false);
			this._receivePong = new AutoResetEvent(false);
			Action action3 = null;
			action3 = () => {
				Stream stream = this._stream;
				Action<WebSocketFrame> u003cu003e9_1 = action2;
				if (u003cu003e9_1 == null)
				{
					Action<WebSocketFrame> action = (WebSocketFrame frame) => {
						if (!this.processReceivedFrame(frame) || this._readyState == WebSocketState.Closed)
						{
							AutoResetEvent u003cu003e4_this = this._exitReceiving;
							if (u003cu003e4_this != null)
							{
								u003cu003e4_this.Set();
							}
							return;
						}
						action3();
						if (this._inMessage || !this.HasMessage || this._readyState != WebSocketState.Open)
						{
							return;
						}
						this.message();
					};
					Action<WebSocketFrame> action1 = action;
					action2 = action;
					u003cu003e9_1 = action1;
				}
				WebSocketFrame.ReadFrameAsync(stream, false, u003cu003e9_1, (Exception ex) => {
					this._logger.Fatal(ex.ToString());
					this.fatal("An exception has occurred while receiving.", ex);
				});
			};
			action3();
		}

		void System.IDisposable.Dispose()
		{
			this.close(new CloseEventArgs(1001), true, true, false);
		}

		private bool validateSecWebSocketAcceptHeader(string value)
		{
			if (value == null)
			{
				return false;
			}
			return value == WebSocket.CreateResponseKey(this._base64Key);
		}

		private bool validateSecWebSocketExtensionsClientHeader(string value)
		{
			if (value == null)
			{
				return true;
			}
			return value.Length > 0;
		}

		private bool validateSecWebSocketExtensionsServerHeader(string value)
		{
			bool flag;
			if (value == null)
			{
				return true;
			}
			if (value.Length == 0)
			{
				return false;
			}
			if (!this._extensionsRequested)
			{
				return false;
			}
			bool flag1 = this._compression != CompressionMethod.None;
			using (IEnumerator<string> enumerator = Ext.SplitHeaderValue(value, new char[] { ',' }).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string str = enumerator.Current.Trim();
					if (!flag1 || !str.IsCompressionExtension(this._compression))
					{
						flag = false;
						return flag;
					}
					else if (str.Contains("server_no_context_takeover"))
					{
						if (!str.Contains("client_no_context_takeover"))
						{
							this._logger.Warn("The server hasn't sent back 'client_no_context_takeover'.");
						}
						string extensionString = this._compression.ToExtensionString(new string[0]);
						if (!Ext.SplitHeaderValue(str, new char[] { ';' }).Contains<string>((string t) => {
							t = t.Trim();
							if (!(t != extensionString) || !(t != "server_no_context_takeover"))
							{
								return false;
							}
							return t != "client_no_context_takeover";
						}))
						{
							continue;
						}
						flag = false;
						return flag;
					}
					else
					{
						this._logger.Error("The server hasn't sent back 'server_no_context_takeover'.");
						flag = false;
						return flag;
					}
				}
				return true;
			}
			return flag;
		}

		private bool validateSecWebSocketKeyHeader(string value)
		{
			if (value == null)
			{
				return false;
			}
			return value.Length > 0;
		}

		private bool validateSecWebSocketProtocolClientHeader(string value)
		{
			if (value == null)
			{
				return true;
			}
			return value.Length > 0;
		}

		private bool validateSecWebSocketProtocolServerHeader(string value)
		{
			if (value == null)
			{
				return !this._protocolsRequested;
			}
			if (value.Length == 0)
			{
				return false;
			}
			if (!this._protocolsRequested)
			{
				return false;
			}
			return this._protocols.Contains<string>((string p) => p == value);
		}

		private bool validateSecWebSocketVersionClientHeader(string value)
		{
			if (value == null)
			{
				return false;
			}
			return value == "13";
		}

		private bool validateSecWebSocketVersionServerHeader(string value)
		{
			if (value == null)
			{
				return true;
			}
			return value == "13";
		}

		public event EventHandler<CloseEventArgs> OnClose;

		public event EventHandler<WebSocket.ErrorEventArgs> OnError;

		public event EventHandler<MessageEventArgs> OnMessage;

		public event EventHandler OnOpen;
	}
}