using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using WebSocket;

namespace WebSocket.Net
{
	public sealed class HttpListener : IDisposable
	{
		private WebSocket.Net.AuthenticationSchemes _authSchemes;

		private Func<HttpListenerRequest, WebSocket.Net.AuthenticationSchemes> _authSchemeSelector;

		private string _certFolderPath;

		private Dictionary<HttpConnection, HttpConnection> _connections;

		private object _connectionsSync;

		private List<HttpListenerContext> _ctxQueue;

		private object _ctxQueueSync;

		private Dictionary<HttpListenerContext, HttpListenerContext> _ctxRegistry;

		private object _ctxRegistrySync;

		private readonly static string _defaultRealm;

		private bool _disposed;

		private bool _ignoreWriteExceptions;

		private volatile bool _listening;

		private Logger _logger;

		private HttpListenerPrefixCollection _prefixes;

		private string _realm;

		private bool _reuseAddress;

		private ServerSslConfiguration _sslConfig;

		private Func<IIdentity, NetworkCredential> _userCredFinder;

		private List<HttpListenerAsyncResult> _waitQueue;

		private object _waitQueueSync;

		public WebSocket.Net.AuthenticationSchemes AuthenticationSchemes
		{
			get
			{
				this.CheckDisposed();
				return this._authSchemes;
			}
			set
			{
				this.CheckDisposed();
				this._authSchemes = value;
			}
		}

		public Func<HttpListenerRequest, WebSocket.Net.AuthenticationSchemes> AuthenticationSchemeSelector
		{
			get
			{
				this.CheckDisposed();
				return this._authSchemeSelector;
			}
			set
			{
				this.CheckDisposed();
				this._authSchemeSelector = value;
			}
		}

		public string CertificateFolderPath
		{
			get
			{
				this.CheckDisposed();
				return this._certFolderPath;
			}
			set
			{
				this.CheckDisposed();
				this._certFolderPath = value;
			}
		}

		public bool IgnoreWriteExceptions
		{
			get
			{
				this.CheckDisposed();
				return this._ignoreWriteExceptions;
			}
			set
			{
				this.CheckDisposed();
				this._ignoreWriteExceptions = value;
			}
		}

		internal bool IsDisposed
		{
			get
			{
				return this._disposed;
			}
		}

		public bool IsListening
		{
			get
			{
				return this._listening;
			}
		}

		public static bool IsSupported
		{
			get
			{
				return true;
			}
		}

		public Logger Log
		{
			get
			{
				return this._logger;
			}
		}

		public HttpListenerPrefixCollection Prefixes
		{
			get
			{
				this.CheckDisposed();
				return this._prefixes;
			}
		}

		public string Realm
		{
			get
			{
				this.CheckDisposed();
				return this._realm;
			}
			set
			{
				this.CheckDisposed();
				this._realm = value;
			}
		}

		internal bool ReuseAddress
		{
			get
			{
				return this._reuseAddress;
			}
			set
			{
				this._reuseAddress = value;
			}
		}

		public ServerSslConfiguration SslConfiguration
		{
			get
			{
				this.CheckDisposed();
				ServerSslConfiguration serverSslConfiguration = this._sslConfig;
				if (serverSslConfiguration == null)
				{
					ServerSslConfiguration serverSslConfiguration1 = new ServerSslConfiguration(null);
					ServerSslConfiguration serverSslConfiguration2 = serverSslConfiguration1;
					this._sslConfig = serverSslConfiguration1;
					serverSslConfiguration = serverSslConfiguration2;
				}
				return serverSslConfiguration;
			}
			set
			{
				this.CheckDisposed();
				this._sslConfig = value;
			}
		}

		public bool UnsafeConnectionNtlmAuthentication
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public Func<IIdentity, NetworkCredential> UserCredentialsFinder
		{
			get
			{
				this.CheckDisposed();
				return this._userCredFinder;
			}
			set
			{
				this.CheckDisposed();
				this._userCredFinder = value;
			}
		}

		static HttpListener()
		{
			HttpListener._defaultRealm = "SECRET AREA";
		}

		public HttpListener()
		{
			this._authSchemes = WebSocket.Net.AuthenticationSchemes.Anonymous;
			this._connections = new Dictionary<HttpConnection, HttpConnection>();
			this._connectionsSync = ((ICollection)this._connections).SyncRoot;
			this._ctxQueue = new List<HttpListenerContext>();
			this._ctxQueueSync = ((ICollection)this._ctxQueue).SyncRoot;
			this._ctxRegistry = new Dictionary<HttpListenerContext, HttpListenerContext>();
			this._ctxRegistrySync = ((ICollection)this._ctxRegistry).SyncRoot;
			this._logger = new Logger();
			this._prefixes = new HttpListenerPrefixCollection(this);
			this._waitQueue = new List<HttpListenerAsyncResult>();
			this._waitQueueSync = ((ICollection)this._waitQueue).SyncRoot;
		}

		public void Abort()
		{
			if (this._disposed)
			{
				return;
			}
			this.close(true);
		}

		internal bool AddConnection(HttpConnection connection)
		{
			bool flag;
			if (!this._listening)
			{
				return false;
			}
			lock (this._connectionsSync)
			{
				if (this._listening)
				{
					this._connections[connection] = connection;
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		internal HttpListenerAsyncResult BeginGetContext(HttpListenerAsyncResult asyncResult)
		{
			HttpListenerAsyncResult httpListenerAsyncResult;
			lock (this._ctxRegistrySync)
			{
				if (!this._listening)
				{
					throw new HttpListenerException(995);
				}
				HttpListenerContext contextFromQueue = this.getContextFromQueue();
				if (contextFromQueue != null)
				{
					asyncResult.Complete(contextFromQueue, true);
				}
				else
				{
					this._waitQueue.Add(asyncResult);
				}
				httpListenerAsyncResult = asyncResult;
			}
			return httpListenerAsyncResult;
		}

		public IAsyncResult BeginGetContext(AsyncCallback callback, object state)
		{
			this.CheckDisposed();
			if (this._prefixes.Count == 0)
			{
				throw new InvalidOperationException("The listener has no URI prefix on which listens.");
			}
			if (!this._listening)
			{
				throw new InvalidOperationException("The listener hasn't been started.");
			}
			return this.BeginGetContext(new HttpListenerAsyncResult(callback, state));
		}

		internal void CheckDisposed()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException(this.GetType().ToString());
			}
		}

		private void cleanupConnections()
		{
			HttpConnection[] httpConnectionArray = null;
			lock (this._connectionsSync)
			{
				if (this._connections.Count != 0)
				{
					Dictionary<!0, !1>.KeyCollection keys = this._connections.Keys;
					httpConnectionArray = new HttpConnection[keys.Count];
					keys.CopyTo(httpConnectionArray, 0);
					this._connections.Clear();
				}
				else
				{
					return;
				}
			}
			for (int i = (int)httpConnectionArray.Length - 1; i >= 0; i--)
			{
				httpConnectionArray[i].Close(true);
			}
		}

		private void cleanupContextQueue(bool sendServiceUnavailable)
		{
			HttpListenerContext[] array = null;
			lock (this._ctxQueueSync)
			{
				if (this._ctxQueue.Count != 0)
				{
					array = this._ctxQueue.ToArray();
					this._ctxQueue.Clear();
				}
				else
				{
					return;
				}
			}
			if (!sendServiceUnavailable)
			{
				return;
			}
			HttpListenerContext[] httpListenerContextArray = array;
			for (int i = 0; i < (int)httpListenerContextArray.Length; i++)
			{
				HttpListenerResponse response = httpListenerContextArray[i].Response;
				response.StatusCode = 503;
				response.Close();
			}
		}

		private void cleanupContextRegistry()
		{
			HttpListenerContext[] httpListenerContextArray = null;
			lock (this._ctxRegistrySync)
			{
				if (this._ctxRegistry.Count != 0)
				{
					Dictionary<!0, !1>.KeyCollection keys = this._ctxRegistry.Keys;
					httpListenerContextArray = new HttpListenerContext[keys.Count];
					keys.CopyTo(httpListenerContextArray, 0);
					this._ctxRegistry.Clear();
				}
				else
				{
					return;
				}
			}
			for (int i = (int)httpListenerContextArray.Length - 1; i >= 0; i--)
			{
				httpListenerContextArray[i].Connection.Close(true);
			}
		}

		private void cleanupWaitQueue(Exception exception)
		{
			HttpListenerAsyncResult[] array = null;
			lock (this._waitQueueSync)
			{
				if (this._waitQueue.Count != 0)
				{
					array = this._waitQueue.ToArray();
					this._waitQueue.Clear();
				}
				else
				{
					return;
				}
			}
			HttpListenerAsyncResult[] httpListenerAsyncResultArray = array;
			for (int i = 0; i < (int)httpListenerAsyncResultArray.Length; i++)
			{
				httpListenerAsyncResultArray[i].Complete(exception);
			}
		}

		private void close(bool force)
		{
			if (this._listening)
			{
				this._listening = false;
				EndPointManager.RemoveListener(this);
			}
			lock (this._ctxRegistrySync)
			{
				this.cleanupContextQueue(!force);
			}
			this.cleanupContextRegistry();
			this.cleanupConnections();
			this.cleanupWaitQueue(new ObjectDisposedException(this.GetType().ToString()));
			this._disposed = true;
		}

		public void Close()
		{
			if (this._disposed)
			{
				return;
			}
			this.close(false);
		}

		public HttpListenerContext EndGetContext(IAsyncResult asyncResult)
		{
			this.CheckDisposed();
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			HttpListenerAsyncResult httpListenerAsyncResult = asyncResult as HttpListenerAsyncResult;
			if (httpListenerAsyncResult == null)
			{
				throw new ArgumentException("A wrong IAsyncResult.", "asyncResult");
			}
			if (httpListenerAsyncResult.EndCalled)
			{
				throw new InvalidOperationException("This IAsyncResult cannot be reused.");
			}
			httpListenerAsyncResult.EndCalled = true;
			if (!httpListenerAsyncResult.IsCompleted)
			{
				httpListenerAsyncResult.AsyncWaitHandle.WaitOne();
			}
			return httpListenerAsyncResult.GetContext();
		}

		private HttpListenerAsyncResult getAsyncResultFromQueue()
		{
			if (this._waitQueue.Count == 0)
			{
				return null;
			}
			HttpListenerAsyncResult item = this._waitQueue[0];
			this._waitQueue.RemoveAt(0);
			return item;
		}

		public HttpListenerContext GetContext()
		{
			this.CheckDisposed();
			if (this._prefixes.Count == 0)
			{
				throw new InvalidOperationException("The listener has no URI prefix on which listens.");
			}
			if (!this._listening)
			{
				throw new InvalidOperationException("The listener hasn't been started.");
			}
			HttpListenerAsyncResult httpListenerAsyncResult = this.BeginGetContext(new HttpListenerAsyncResult(null, null));
			httpListenerAsyncResult.InGet = true;
			return this.EndGetContext(httpListenerAsyncResult);
		}

		private HttpListenerContext getContextFromQueue()
		{
			if (this._ctxQueue.Count == 0)
			{
				return null;
			}
			HttpListenerContext item = this._ctxQueue[0];
			this._ctxQueue.RemoveAt(0);
			return item;
		}

		internal string GetRealm()
		{
			string str = this._realm;
			if (str != null && str.Length > 0)
			{
				return str;
			}
			return HttpListener._defaultRealm;
		}

		internal Func<IIdentity, NetworkCredential> GetUserCredentialsFinder()
		{
			return this._userCredFinder;
		}

		internal bool RegisterContext(HttpListenerContext context)
		{
			bool flag;
			if (!this._listening)
			{
				return false;
			}
			lock (this._ctxRegistrySync)
			{
				if (this._listening)
				{
					this._ctxRegistry[context] = context;
					HttpListenerAsyncResult asyncResultFromQueue = this.getAsyncResultFromQueue();
					if (asyncResultFromQueue != null)
					{
						asyncResultFromQueue.Complete(context);
					}
					else
					{
						this._ctxQueue.Add(context);
					}
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		internal void RemoveConnection(HttpConnection connection)
		{
			lock (this._connectionsSync)
			{
				this._connections.Remove(connection);
			}
		}

		internal WebSocket.Net.AuthenticationSchemes SelectAuthenticationScheme(HttpListenerRequest request)
		{
			WebSocket.Net.AuthenticationSchemes authenticationScheme;
			Func<HttpListenerRequest, WebSocket.Net.AuthenticationSchemes> func = this._authSchemeSelector;
			if (func == null)
			{
				return this._authSchemes;
			}
			try
			{
				authenticationScheme = func(request);
			}
			catch
			{
				authenticationScheme = WebSocket.Net.AuthenticationSchemes.None;
			}
			return authenticationScheme;
		}

		public void Start()
		{
			this.CheckDisposed();
			if (this._listening)
			{
				return;
			}
			EndPointManager.AddListener(this);
			this._listening = true;
		}

		public void Stop()
		{
			this.CheckDisposed();
			if (!this._listening)
			{
				return;
			}
			this._listening = false;
			EndPointManager.RemoveListener(this);
			lock (this._ctxRegistrySync)
			{
				this.cleanupContextQueue(true);
			}
			this.cleanupContextRegistry();
			this.cleanupConnections();
			this.cleanupWaitQueue(new HttpListenerException(995, "The listener is stopped."));
		}

		void System.IDisposable.Dispose()
		{
			if (this._disposed)
			{
				return;
			}
			this.close(true);
		}

		internal void UnregisterContext(HttpListenerContext context)
		{
			lock (this._ctxRegistrySync)
			{
				this._ctxRegistry.Remove(context);
			}
		}
	}
}