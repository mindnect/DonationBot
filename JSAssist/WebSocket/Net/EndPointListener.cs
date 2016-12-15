using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace WebSocket.Net
{
	internal sealed class EndPointListener
	{
		private List<HttpListenerPrefix> _all;

		private readonly static string _defaultCertFolderPath;

		private IPEndPoint _endpoint;

		private Dictionary<HttpListenerPrefix, WebSocket.Net.HttpListener> _prefixes;

		private bool _secure;

		private Socket _socket;

		private ServerSslConfiguration _sslConfig;

		private List<HttpListenerPrefix> _unhandled;

		private Dictionary<HttpConnection, HttpConnection> _unregistered;

		private object _unregisteredSync;

		public IPAddress Address
		{
			get
			{
				return this._endpoint.Address;
			}
		}

		public bool IsSecure
		{
			get
			{
				return this._secure;
			}
		}

		public int Port
		{
			get
			{
				return this._endpoint.Port;
			}
		}

		public ServerSslConfiguration SslConfiguration
		{
			get
			{
				return this._sslConfig;
			}
		}

		static EndPointListener()
		{
			EndPointListener._defaultCertFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		}

		internal EndPointListener(IPEndPoint endpoint, bool secure, string certificateFolderPath, ServerSslConfiguration sslConfig, bool reuseAddress)
		{
			if (secure)
			{
				X509Certificate2 certificate = EndPointListener.getCertificate(endpoint.Port, certificateFolderPath, sslConfig.ServerCertificate);
				if (certificate == null)
				{
					throw new ArgumentException("No server certificate could be found.");
				}
				this._secure = true;
				this._sslConfig = new ServerSslConfiguration(certificate, sslConfig.ClientCertificateRequired, sslConfig.EnabledSslProtocols, sslConfig.CheckCertificateRevocation)
				{
					ClientCertificateValidationCallback = sslConfig.ClientCertificateValidationCallback
				};
			}
			this._endpoint = endpoint;
			this._prefixes = new Dictionary<HttpListenerPrefix, WebSocket.Net.HttpListener>();
			this._unregistered = new Dictionary<HttpConnection, HttpConnection>();
			this._unregisteredSync = ((ICollection)this._unregistered).SyncRoot;
			this._socket = new Socket(endpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			if (reuseAddress)
			{
				this._socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			}
			this._socket.Bind(endpoint);
			this._socket.Listen(500);
			this._socket.BeginAccept(new AsyncCallback(EndPointListener.onAccept), this);
		}

		public void AddPrefix(HttpListenerPrefix prefix, WebSocket.Net.HttpListener listener)
		{
			List<HttpListenerPrefix> httpListenerPrefixes;
			List<HttpListenerPrefix> httpListenerPrefixes1;
			Dictionary<HttpListenerPrefix, WebSocket.Net.HttpListener> httpListenerPrefixes2;
			Dictionary<HttpListenerPrefix, WebSocket.Net.HttpListener> httpListenerPrefixes3;
			if (prefix.Host == "*")
			{
				do
				{
					httpListenerPrefixes = this._unhandled;
					httpListenerPrefixes1 = (httpListenerPrefixes != null ? new List<HttpListenerPrefix>(httpListenerPrefixes) : new List<HttpListenerPrefix>());
					prefix.Listener = listener;
					EndPointListener.addSpecial(httpListenerPrefixes1, prefix);
				}
				while (Interlocked.CompareExchange<List<HttpListenerPrefix>>(ref this._unhandled, httpListenerPrefixes1, httpListenerPrefixes) != httpListenerPrefixes);
				return;
			}
			if (prefix.Host == "+")
			{
				do
				{
					httpListenerPrefixes = this._all;
					httpListenerPrefixes1 = (httpListenerPrefixes != null ? new List<HttpListenerPrefix>(httpListenerPrefixes) : new List<HttpListenerPrefix>());
					prefix.Listener = listener;
					EndPointListener.addSpecial(httpListenerPrefixes1, prefix);
				}
				while (Interlocked.CompareExchange<List<HttpListenerPrefix>>(ref this._all, httpListenerPrefixes1, httpListenerPrefixes) != httpListenerPrefixes);
				return;
			}
			do
			{
				httpListenerPrefixes2 = this._prefixes;
				if (httpListenerPrefixes2.ContainsKey(prefix))
				{
					if (httpListenerPrefixes2[prefix] != listener)
					{
						throw new WebSocket.Net.HttpListenerException(87, string.Format("There's another listener for {0}.", prefix));
					}
					return;
				}
				httpListenerPrefixes3 = new Dictionary<HttpListenerPrefix, WebSocket.Net.HttpListener>(httpListenerPrefixes2);
				httpListenerPrefixes3[prefix] = listener;
			}
			while (Interlocked.CompareExchange<Dictionary<HttpListenerPrefix, WebSocket.Net.HttpListener>>(ref this._prefixes, httpListenerPrefixes3, httpListenerPrefixes2) != httpListenerPrefixes2);
		}

		private static void addSpecial(List<HttpListenerPrefix> prefixes, HttpListenerPrefix prefix)
		{
			string path = prefix.Path;
			foreach (HttpListenerPrefix httpListenerPrefix in prefixes)
			{
				if (httpListenerPrefix.Path != path)
				{
					continue;
				}
				throw new WebSocket.Net.HttpListenerException(87, "The prefix is already in use.");
			}
			prefixes.Add(prefix);
		}

		internal static bool CertificateExists(int port, string folderPath)
		{
			if (folderPath == null || folderPath.Length == 0)
			{
				folderPath = EndPointListener._defaultCertFolderPath;
			}
			string str = Path.Combine(folderPath, string.Format("{0}.cer", port));
			string str1 = Path.Combine(folderPath, string.Format("{0}.key", port));
			if (!File.Exists(str))
			{
				return false;
			}
			return File.Exists(str1);
		}

		public void Close()
		{
			this._socket.Close();
			HttpConnection[] httpConnectionArray = null;
			lock (this._unregisteredSync)
			{
				if (this._unregistered.Count != 0)
				{
					Dictionary<!0, !1>.KeyCollection keys = this._unregistered.Keys;
					httpConnectionArray = new HttpConnection[keys.Count];
					keys.CopyTo(httpConnectionArray, 0);
					this._unregistered.Clear();
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

		private static RSACryptoServiceProvider createRSAFromFile(string filename)
		{
			byte[] numArray = null;
			using (FileStream fileStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				numArray = new byte[checked((IntPtr)fileStream.Length)];
				fileStream.Read(numArray, 0, (int)numArray.Length);
			}
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
			rSACryptoServiceProvider.ImportCspBlob(numArray);
			return rSACryptoServiceProvider;
		}

		private static X509Certificate2 getCertificate(int port, string folderPath, X509Certificate2 defaultCertificate)
		{
			if (folderPath == null || folderPath.Length == 0)
			{
				folderPath = EndPointListener._defaultCertFolderPath;
			}
			try
			{
				string str = Path.Combine(folderPath, string.Format("{0}.cer", port));
				string str1 = Path.Combine(folderPath, string.Format("{0}.key", port));
				if (File.Exists(str) && File.Exists(str1))
				{
					return new X509Certificate2(str)
					{
						PrivateKey = EndPointListener.createRSAFromFile(str1)
					};
				}
			}
			catch
			{
			}
			return defaultCertificate;
		}

		private void leaveIfNoPrefix()
		{
			if (this._prefixes.Count > 0)
			{
				return;
			}
			List<HttpListenerPrefix> httpListenerPrefixes = this._unhandled;
			if (httpListenerPrefixes != null && httpListenerPrefixes.Count > 0)
			{
				return;
			}
			httpListenerPrefixes = this._all;
			if (httpListenerPrefixes != null && httpListenerPrefixes.Count > 0)
			{
				return;
			}
			EndPointManager.RemoveEndPoint(this._endpoint);
		}

		private static void onAccept(IAsyncResult asyncResult)
		{
			EndPointListener asyncState = (EndPointListener)asyncResult.AsyncState;
			Socket socket = null;
			try
			{
				socket = asyncState._socket.EndAccept(asyncResult);
			}
			catch (SocketException socketException)
			{
			}
			catch (ObjectDisposedException objectDisposedException)
			{
				return;
			}
			try
			{
				asyncState._socket.BeginAccept(new AsyncCallback(EndPointListener.onAccept), asyncState);
			}
			catch
			{
				if (socket != null)
				{
					socket.Close();
				}
				return;
			}
			if (socket == null)
			{
				return;
			}
			EndPointListener.processAccepted(socket, asyncState);
		}

		private static void processAccepted(Socket socket, EndPointListener listener)
		{
			HttpConnection httpConnection = null;
			try
			{
				httpConnection = new HttpConnection(socket, listener);
				lock (listener._unregisteredSync)
				{
					listener._unregistered[httpConnection] = httpConnection;
				}
				httpConnection.BeginReadRequest();
			}
			catch
			{
				if (httpConnection == null)
				{
					socket.Close();
				}
				else
				{
					httpConnection.Close(true);
				}
			}
		}

		internal void RemoveConnection(HttpConnection connection)
		{
			lock (this._unregisteredSync)
			{
				this._unregistered.Remove(connection);
			}
		}

		public void RemovePrefix(HttpListenerPrefix prefix, WebSocket.Net.HttpListener listener)
		{
			List<HttpListenerPrefix> httpListenerPrefixes;
			List<HttpListenerPrefix> httpListenerPrefixes1;
			Dictionary<HttpListenerPrefix, WebSocket.Net.HttpListener> httpListenerPrefixes2;
			Dictionary<HttpListenerPrefix, WebSocket.Net.HttpListener> httpListenerPrefixes3;
			if (prefix.Host == "*")
			{
				do
				{
					httpListenerPrefixes = this._unhandled;
					if (httpListenerPrefixes == null)
					{
						break;
					}
					httpListenerPrefixes1 = new List<HttpListenerPrefix>(httpListenerPrefixes);
				}
				while (EndPointListener.removeSpecial(httpListenerPrefixes1, prefix) && Interlocked.CompareExchange<List<HttpListenerPrefix>>(ref this._unhandled, httpListenerPrefixes1, httpListenerPrefixes) != httpListenerPrefixes);
				this.leaveIfNoPrefix();
				return;
			}
			if (prefix.Host == "+")
			{
				do
				{
					httpListenerPrefixes = this._all;
					if (httpListenerPrefixes == null)
					{
						break;
					}
					httpListenerPrefixes1 = new List<HttpListenerPrefix>(httpListenerPrefixes);
				}
				while (EndPointListener.removeSpecial(httpListenerPrefixes1, prefix) && Interlocked.CompareExchange<List<HttpListenerPrefix>>(ref this._all, httpListenerPrefixes1, httpListenerPrefixes) != httpListenerPrefixes);
				this.leaveIfNoPrefix();
				return;
			}
			do
			{
				httpListenerPrefixes2 = this._prefixes;
				if (!httpListenerPrefixes2.ContainsKey(prefix))
				{
					break;
				}
				httpListenerPrefixes3 = new Dictionary<HttpListenerPrefix, WebSocket.Net.HttpListener>(httpListenerPrefixes2);
				httpListenerPrefixes3.Remove(prefix);
			}
			while (Interlocked.CompareExchange<Dictionary<HttpListenerPrefix, WebSocket.Net.HttpListener>>(ref this._prefixes, httpListenerPrefixes3, httpListenerPrefixes2) != httpListenerPrefixes2);
			this.leaveIfNoPrefix();
		}

		private static bool removeSpecial(List<HttpListenerPrefix> prefixes, HttpListenerPrefix prefix)
		{
			string path = prefix.Path;
			int count = prefixes.Count;
			for (int i = 0; i < count; i++)
			{
				if (prefixes[i].Path == path)
				{
					prefixes.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		private static WebSocket.Net.HttpListener searchHttpListenerFromSpecial(string path, List<HttpListenerPrefix> prefixes)
		{
			if (prefixes == null)
			{
				return null;
			}
			WebSocket.Net.HttpListener listener = null;
			int num = -1;
			foreach (HttpListenerPrefix prefix in prefixes)
			{
				string str = prefix.Path;
				int length = str.Length;
				if (length < num || !path.StartsWith(str))
				{
					continue;
				}
				num = length;
				listener = prefix.Listener;
			}
			return listener;
		}

		internal bool TrySearchHttpListener(Uri uri, out WebSocket.Net.HttpListener listener)
		{
			listener = null;
			if (uri == null)
			{
				return false;
			}
			string host = uri.Host;
			bool flag = Uri.CheckHostName(host) == UriHostNameType.Dns;
			string str = uri.Port.ToString();
			string str1 = HttpUtility.UrlDecode(uri.AbsolutePath);
			string str2 = (str1[str1.Length - 1] != '/' ? string.Concat(str1, "/") : str1);
			if (host != null && host.Length > 0)
			{
				int num = -1;
				foreach (HttpListenerPrefix key in this._prefixes.Keys)
				{
					if (flag)
					{
						string host1 = key.Host;
						if (Uri.CheckHostName(host1) == UriHostNameType.Dns && host1 != host)
						{
							continue;
						}
					}
					if (key.Port != str)
					{
						continue;
					}
					string path = key.Path;
					int length = path.Length;
					if (length < num || !str1.StartsWith(path) && !str2.StartsWith(path))
					{
						continue;
					}
					num = length;
					listener = this._prefixes[key];
				}
				if (num != -1)
				{
					return true;
				}
			}
			List<HttpListenerPrefix> httpListenerPrefixes = this._unhandled;
			listener = EndPointListener.searchHttpListenerFromSpecial(str1, httpListenerPrefixes);
			if (listener == null && str2 != str1)
			{
				listener = EndPointListener.searchHttpListenerFromSpecial(str2, httpListenerPrefixes);
			}
			if (listener != null)
			{
				return true;
			}
			httpListenerPrefixes = this._all;
			listener = EndPointListener.searchHttpListenerFromSpecial(str1, httpListenerPrefixes);
			if (listener == null && str2 != str1)
			{
				listener = EndPointListener.searchHttpListenerFromSpecial(str2, httpListenerPrefixes);
			}
			return listener != null;
		}
	}
}