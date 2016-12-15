using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading;
using WebSocket;
using WebSocket.Net;
using WebSocket.Net.WebSockets;

namespace WebSocket.Server
{
	public class HttpServer
	{
		private IPAddress _address;

		private string _hostname;

		private WebSocket.Net.HttpListener _listener;

		private Logger _logger;

		private int _port;

		private Thread _receiveThread;

		private string _rootPath;

		private bool _secure;

		private WebSocketServiceManager _services;

		private volatile ServerState _state;

		private object _sync;

		private bool _windows;

		public IPAddress Address
		{
			get
			{
				return this._address;
			}
		}

		public WebSocket.Net.AuthenticationSchemes AuthenticationSchemes
		{
			get
			{
				return this._listener.AuthenticationSchemes;
			}
			set
			{
				string str = ((ServerState)this._state).CheckIfAvailable(true, false, false);
				if (str != null)
				{
					this._logger.Error(str);
					return;
				}
				this._listener.AuthenticationSchemes = value;
			}
		}

		public bool IsListening
		{
			get
			{
				return this._state == ServerState.Start;
			}
		}

		public bool IsSecure
		{
			get
			{
				return this._secure;
			}
		}

		public bool KeepClean
		{
			get
			{
				return this._services.KeepClean;
			}
			set
			{
				string str = ((ServerState)this._state).CheckIfAvailable(true, false, false);
				if (str != null)
				{
					this._logger.Error(str);
					return;
				}
				this._services.KeepClean = value;
			}
		}

		public Logger Log
		{
			get
			{
				return this._logger;
			}
		}

		public int Port
		{
			get
			{
				return this._port;
			}
		}

		public string Realm
		{
			get
			{
				return this._listener.Realm;
			}
			set
			{
				string str = ((ServerState)this._state).CheckIfAvailable(true, false, false);
				if (str != null)
				{
					this._logger.Error(str);
					return;
				}
				this._listener.Realm = value;
			}
		}

		public bool ReuseAddress
		{
			get
			{
				return this._listener.ReuseAddress;
			}
			set
			{
				string str = ((ServerState)this._state).CheckIfAvailable(true, false, false);
				if (str != null)
				{
					this._logger.Error(str);
					return;
				}
				this._listener.ReuseAddress = value;
			}
		}

		public string RootPath
		{
			get
			{
				if (this._rootPath != null && this._rootPath.Length > 0)
				{
					return this._rootPath;
				}
				string str = "./Public";
				string str1 = str;
				this._rootPath = str;
				return str1;
			}
			set
			{
				string str = ((ServerState)this._state).CheckIfAvailable(true, false, false);
				if (str == null)
				{
					this._rootPath = value;
					return;
				}
				this._logger.Error(str);
			}
		}

		public ServerSslConfiguration SslConfiguration
		{
			get
			{
				return this._listener.SslConfiguration;
			}
			set
			{
				string str = ((ServerState)this._state).CheckIfAvailable(true, false, false);
				if (str != null)
				{
					this._logger.Error(str);
					return;
				}
				this._listener.SslConfiguration = value;
			}
		}

		public Func<IIdentity, WebSocket.Net.NetworkCredential> UserCredentialsFinder
		{
			get
			{
				return this._listener.UserCredentialsFinder;
			}
			set
			{
				string str = ((ServerState)this._state).CheckIfAvailable(true, false, false);
				if (str != null)
				{
					this._logger.Error(str);
					return;
				}
				this._listener.UserCredentialsFinder = value;
			}
		}

		public TimeSpan WaitTime
		{
			get
			{
				return this._services.WaitTime;
			}
			set
			{
				string str = ((ServerState)this._state).CheckIfAvailable(true, false, false) ?? value.CheckIfValidWaitTime();
				if (str != null)
				{
					this._logger.Error(str);
					return;
				}
				this._services.WaitTime = value;
			}
		}

		public WebSocketServiceManager WebSocketServices
		{
			get
			{
				return this._services;
			}
		}

		public HttpServer()
		{
			this.init("*", IPAddress.Any, 80, false);
		}

		public HttpServer(int port) : this(port, port == 443)
		{
		}

		public HttpServer(string url)
		{
			Uri uri;
			string str;
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			if (url.Length == 0)
			{
				throw new ArgumentException("An empty string.", "url");
			}
			if (!HttpServer.tryCreateUri(url, out uri, out str))
			{
				throw new ArgumentException(str, "url");
			}
			string host = HttpServer.getHost(uri);
			IPAddress pAddress = host.ToIPAddress();
			if (!pAddress.IsLocal())
			{
				throw new ArgumentException(string.Concat("The host part isn't a local host name: ", url), "url");
			}
			this.init(host, pAddress, uri.Port, uri.Scheme == "https");
		}

		public HttpServer(int port, bool secure)
		{
			if (!port.IsPortNumber())
			{
				throw new ArgumentOutOfRangeException("port", string.Concat("Not between 1 and 65535 inclusive: ", port));
			}
			this.init("*", IPAddress.Any, port, secure);
		}

		public HttpServer(IPAddress address, int port) : this(address, port, port == 443)
		{
		}

		public HttpServer(IPAddress address, int port, bool secure)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (!address.IsLocal())
			{
				throw new ArgumentException(string.Concat("Not a local IP address: ", address), "address");
			}
			if (!port.IsPortNumber())
			{
				throw new ArgumentOutOfRangeException("port", string.Concat("Not between 1 and 65535 inclusive: ", port));
			}
			this.init(null, address, port, secure);
		}

		private void abort()
		{
			lock (this._sync)
			{
				if (this.IsListening)
				{
					this._state = ServerState.ShuttingDown;
				}
				else
				{
					return;
				}
			}
			this._services.Stop(new CloseEventArgs(CloseStatusCode.ServerError), true, false);
			this._listener.Abort();
			this._state = ServerState.Stop;
		}

		public void AddWebSocketService<TBehavior>(string path, Func<TBehavior> initializer)
		where TBehavior : WebSocketBehavior
		{
			object obj = path.CheckIfValidServicePath();
			if (obj == null)
			{
				if (initializer == null)
				{
					obj = "'initializer' is null.";
				}
				else
				{
					obj = null;
				}
			}
			string str = (string)obj;
			if (str != null)
			{
				this._logger.Error(str);
				return;
			}
			this._services.Add<TBehavior>(path, initializer);
		}

		public void AddWebSocketService<TBehaviorWithNew>(string path)
		where TBehaviorWithNew : WebSocketBehavior, new()
		{
			this.AddWebSocketService<TBehaviorWithNew>(path, () => Activator.CreateInstance<TBehaviorWithNew>());
		}

		private bool checkIfAvailable(bool ready, bool start, bool shutting, bool stop, out string message)
		{
			message = null;
			if (!ready && this._state == ServerState.Ready)
			{
				message = "This operation is not available in: ready";
				return false;
			}
			if (!start && this._state == ServerState.Start)
			{
				message = "This operation is not available in: start";
				return false;
			}
			if (!shutting && this._state == ServerState.ShuttingDown)
			{
				message = "This operation is not available in: shutting down";
				return false;
			}
			if (stop || this._state != ServerState.Stop)
			{
				return true;
			}
			message = "This operation is not available in: stop";
			return false;
		}

		private string checkIfCertificateExists()
		{
			if (!this._secure)
			{
				return null;
			}
			bool serverCertificate = this._listener.SslConfiguration.ServerCertificate != null;
			bool flag = EndPointListener.CertificateExists(this._port, this._listener.CertificateFolderPath);
			if (serverCertificate & flag)
			{
				this._logger.Warn("The server certificate associated with the port number already exists.");
				return null;
			}
			if (serverCertificate | flag)
			{
				return null;
			}
			return "The secure connection requires a server certificate.";
		}

		private static string convertToString(IPAddress address)
		{
			if (address.AddressFamily != AddressFamily.InterNetworkV6)
			{
				return address.ToString();
			}
			return string.Format("[{0}]", address.ToString());
		}

		public byte[] GetFile(string path)
		{
			path = string.Concat(this.RootPath, path);
			if (this._windows)
			{
				path = path.Replace("/", "\\");
			}
			if (!File.Exists(path))
			{
				return null;
			}
			return File.ReadAllBytes(path);
		}

		private static string getHost(Uri uri)
		{
			if (uri.HostNameType != UriHostNameType.IPv6)
			{
				return uri.DnsSafeHost;
			}
			return uri.Host;
		}

		private void init(string hostname, IPAddress address, int port, bool secure)
		{
			this._hostname = hostname ?? HttpServer.convertToString(address);
			this._address = address;
			this._port = port;
			this._secure = secure;
			this._listener = new WebSocket.Net.HttpListener();
			this._listener.Prefixes.Add(string.Format("http{0}://{1}:{2}/", (secure ? "s" : ""), this._hostname, port));
			this._logger = this._listener.Log;
			this._services = new WebSocketServiceManager(this._logger);
			this._sync = new object();
			OperatingSystem oSVersion = Environment.OSVersion;
			this._windows = (oSVersion.Platform == PlatformID.Unix ? false : oSVersion.Platform != PlatformID.MacOSX);
		}

		private void processRequest(WebSocket.Net.HttpListenerContext context)
		{
			EventHandler<HttpRequestEventArgs> eventHandler;
			string httpMethod = context.Request.HttpMethod;
			if (httpMethod == "GET")
			{
				eventHandler = this.OnGet;
			}
			else if (httpMethod == "HEAD")
			{
				eventHandler = this.OnHead;
			}
			else if (httpMethod == "POST")
			{
				eventHandler = this.OnPost;
			}
			else if (httpMethod == "PUT")
			{
				eventHandler = this.OnPut;
			}
			else if (httpMethod == "DELETE")
			{
				eventHandler = this.OnDelete;
			}
			else if (httpMethod == "OPTIONS")
			{
				eventHandler = this.OnOptions;
			}
			else if (httpMethod == "TRACE")
			{
				eventHandler = this.OnTrace;
			}
			else if (httpMethod == "CONNECT")
			{
				eventHandler = this.OnConnect;
			}
			else if (httpMethod == "PATCH")
			{
				eventHandler = this.OnPatch;
			}
			else
			{
				eventHandler = null;
			}
			EventHandler<HttpRequestEventArgs> eventHandler1 = eventHandler;
			if (eventHandler1 == null)
			{
				context.Response.StatusCode = 501;
			}
			else
			{
				eventHandler1(this, new HttpRequestEventArgs(context));
			}
			context.Response.Close();
		}

		private void processRequest(HttpListenerWebSocketContext context)
		{
			WebSocketServiceHost webSocketServiceHost;
			if (!this._services.InternalTryGetServiceHost(context.RequestUri.AbsolutePath, out webSocketServiceHost))
			{
				context.Close(WebSocket.Net.HttpStatusCode.NotImplemented);
				return;
			}
			webSocketServiceHost.StartSession(context);
		}

		private void receiveRequest()
		{
			while (true)
			{
				try
				{
					WebSocket.Net.HttpListenerContext context = this._listener.GetContext();
					ThreadPool.QueueUserWorkItem((object state) => {
						try
						{
							if (!context.Request.IsUpgradeTo("websocket"))
							{
								this.processRequest(context);
							}
							else
							{
								this.processRequest(context.AcceptWebSocket(null));
							}
						}
						catch (Exception exception)
						{
							this._logger.Fatal(exception.ToString());
							context.Connection.Close(true);
						}
					});
				}
				catch (WebSocket.Net.HttpListenerException httpListenerException1)
				{
					WebSocket.Net.HttpListenerException httpListenerException = httpListenerException1;
					this._logger.Warn(string.Concat("Receiving has been stopped.\n  reason: ", httpListenerException.Message));
					break;
				}
				catch (Exception exception1)
				{
					this._logger.Fatal(exception1.ToString());
					break;
				}
			}
			if (this.IsListening)
			{
				this.abort();
			}
		}

		public bool RemoveWebSocketService(string path)
		{
			string str = path.CheckIfValidServicePath();
			if (str == null)
			{
				return this._services.Remove(path);
			}
			this._logger.Error(str);
			return false;
		}

		public void Start()
		{
			lock (this._sync)
			{
				string str = ((ServerState)this._state).CheckIfAvailable(true, false, false) ?? this.checkIfCertificateExists();
				if (str == null)
				{
					this._services.Start();
					this.startReceiving();
					this._state = ServerState.Start;
				}
				else
				{
					this._logger.Error(str);
				}
			}
		}

		private void startReceiving()
		{
			this._listener.Start();
			this._receiveThread = new Thread(new ThreadStart(this.receiveRequest))
			{
				IsBackground = true
			};
			this._receiveThread.Start();
		}

		public void Stop()
		{
			string str;
			if (!this.checkIfAvailable(false, true, false, false, out str))
			{
				this._logger.Error(str);
				return;
			}
			lock (this._sync)
			{
				if (this.checkIfAvailable(false, true, false, false, out str))
				{
					this._state = ServerState.ShuttingDown;
				}
				else
				{
					this._logger.Error(str);
					return;
				}
			}
			this._services.Stop(new CloseEventArgs(), true, true);
			this.stopReceiving(5000);
			this._state = ServerState.Stop;
		}

		public void Stop(ushort code, string reason)
		{
			string str;
			if (!this.checkIfAvailable(false, true, false, false, out str))
			{
				this._logger.Error(str);
				return;
			}
			if (!WebSocket.CheckParametersForClose(code, reason, false, out str))
			{
				this._logger.Error(str);
				return;
			}
			lock (this._sync)
			{
				if (this.checkIfAvailable(false, true, false, false, out str))
				{
					this._state = ServerState.ShuttingDown;
				}
				else
				{
					this._logger.Error(str);
					return;
				}
			}
			if (code != 1005)
			{
				bool flag = !code.IsReserved();
				this._services.Stop(new CloseEventArgs(code, reason), flag, flag);
			}
			else
			{
				this._services.Stop(new CloseEventArgs(), true, true);
			}
			this.stopReceiving(5000);
			this._state = ServerState.Stop;
		}

		public void Stop(CloseStatusCode code, string reason)
		{
			string str;
			if (!this.checkIfAvailable(false, true, false, false, out str))
			{
				this._logger.Error(str);
				return;
			}
			if (!WebSocket.CheckParametersForClose(code, reason, false, out str))
			{
				this._logger.Error(str);
				return;
			}
			lock (this._sync)
			{
				if (this.checkIfAvailable(false, true, false, false, out str))
				{
					this._state = ServerState.ShuttingDown;
				}
				else
				{
					this._logger.Error(str);
					return;
				}
			}
			if (code != CloseStatusCode.NoStatus)
			{
				bool flag = !code.IsReserved();
				this._services.Stop(new CloseEventArgs(code, reason), flag, flag);
			}
			else
			{
				this._services.Stop(new CloseEventArgs(), true, true);
			}
			this.stopReceiving(5000);
			this._state = ServerState.Stop;
		}

		private void stopReceiving(int millisecondsTimeout)
		{
			this._listener.Close();
			this._receiveThread.Join(millisecondsTimeout);
		}

		private static bool tryCreateUri(string uriString, out Uri result, out string message)
		{
			result = null;
			Uri uri = uriString.ToUri();
			if (uri == null)
			{
				message = string.Concat("An invalid URI string: ", uriString);
				return false;
			}
			if (!uri.IsAbsoluteUri)
			{
				message = string.Concat("Not an absolute URI: ", uriString);
				return false;
			}
			string scheme = uri.Scheme;
			if (!(scheme == "http") && !(scheme == "https"))
			{
				message = string.Concat("The scheme part isn't 'http' or 'https': ", uriString);
				return false;
			}
			if (uri.PathAndQuery != "/")
			{
				message = string.Concat("Includes the path or query component: ", uriString);
				return false;
			}
			if (uri.Fragment.Length > 0)
			{
				message = string.Concat("Includes the fragment component: ", uriString);
				return false;
			}
			if (uri.Port == 0)
			{
				message = string.Concat("The port part is zero: ", uriString);
				return false;
			}
			result = uri;
			message = string.Empty;
			return true;
		}

		public event EventHandler<HttpRequestEventArgs> OnConnect;

		public event EventHandler<HttpRequestEventArgs> OnDelete;

		public event EventHandler<HttpRequestEventArgs> OnGet;

		public event EventHandler<HttpRequestEventArgs> OnHead;

		public event EventHandler<HttpRequestEventArgs> OnOptions;

		public event EventHandler<HttpRequestEventArgs> OnPatch;

		public event EventHandler<HttpRequestEventArgs> OnPost;

		public event EventHandler<HttpRequestEventArgs> OnPut;

		public event EventHandler<HttpRequestEventArgs> OnTrace;
	}
}