using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using WebSocket;
using WebSocket.Net;

namespace WebSocket.Net.WebSockets
{
	internal class TcpListenerWebSocketContext : WebSocketContext
	{
		private WebSocket.Net.CookieCollection _cookies;

		private Logger _logger;

		private NameValueCollection _queryString;

		private HttpRequest _request;

		private bool _secure;

		private System.IO.Stream _stream;

		private TcpClient _tcpClient;

		private Uri _uri;

		private IPrincipal _user;

		private WebSocket.WebSocket _websocket;

		public override WebSocket.Net.CookieCollection CookieCollection
		{
			get
			{
				WebSocket.Net.CookieCollection cookieCollections = this._cookies;
				if (cookieCollections == null)
				{
					WebSocket.Net.CookieCollection cookies = this._request.Cookies;
					WebSocket.Net.CookieCollection cookieCollections1 = cookies;
					this._cookies = cookies;
					cookieCollections = cookieCollections1;
				}
				return cookieCollections;
			}
		}

		public override NameValueCollection Headers
		{
			get
			{
				return this._request.Headers;
			}
		}

		public override string Host
		{
			get
			{
				return this._request.Headers["Host"];
			}
		}

		public override bool IsAuthenticated
		{
			get
			{
				return this._user != null;
			}
		}

		public override bool IsLocal
		{
			get
			{
				return this.UserEndPoint.Address.IsLocal();
			}
		}

		public override bool IsSecureConnection
		{
			get
			{
				return this._secure;
			}
		}

		public override bool IsWebSocketRequest
		{
			get
			{
				return this._request.IsWebSocketRequest;
			}
		}

		internal Logger Log
		{
			get
			{
				return this._logger;
			}
		}

		public override string Origin
		{
			get
			{
				return this._request.Headers["Origin"];
			}
		}

		public override NameValueCollection QueryString
		{
			get
			{
				string query;
				NameValueCollection nameValueCollection = this._queryString;
				if (nameValueCollection == null)
				{
					if (this._uri != null)
					{
						query = this._uri.Query;
					}
					else
					{
						query = null;
					}
					NameValueCollection nameValueCollection1 = HttpUtility.InternalParseQueryString(query, Encoding.UTF8);
					NameValueCollection nameValueCollection2 = nameValueCollection1;
					this._queryString = nameValueCollection1;
					nameValueCollection = nameValueCollection2;
				}
				return nameValueCollection;
			}
		}

		public override Uri RequestUri
		{
			get
			{
				return this._uri;
			}
		}

		public override string SecWebSocketKey
		{
			get
			{
				return this._request.Headers["Sec-WebSocket-Key"];
			}
		}

		public override IEnumerable<string> SecWebSocketProtocols
		{
			get
			{
				string item = this._request.Headers["Sec-WebSocket-Protocol"];
				if (item != null)
				{
					string[] strArrays = item.Split(new char[] { ',' });
					for (int i = 0; i < (int)strArrays.Length; i++)
					{
						yield return strArrays[i].Trim();
					}
					strArrays = null;
				}
			}
		}

		public override string SecWebSocketVersion
		{
			get
			{
				return this._request.Headers["Sec-WebSocket-Version"];
			}
		}

		public override IPEndPoint ServerEndPoint
		{
			get
			{
				return (IPEndPoint)this._tcpClient.Client.LocalEndPoint;
			}
		}

		internal System.IO.Stream Stream
		{
			get
			{
				return this._stream;
			}
		}

		public override IPrincipal User
		{
			get
			{
				return this._user;
			}
		}

		public override IPEndPoint UserEndPoint
		{
			get
			{
				return (IPEndPoint)this._tcpClient.Client.RemoteEndPoint;
			}
		}

		public override WebSocket.WebSocket WebSocket
		{
			get
			{
				return this._websocket;
			}
		}

		internal TcpListenerWebSocketContext(TcpClient tcpClient, string protocol, bool secure, ServerSslConfiguration sslConfig, Logger logger)
		{
			this._tcpClient = tcpClient;
			this._secure = secure;
			this._logger = logger;
			NetworkStream stream = tcpClient.GetStream();
			if (!secure)
			{
				this._stream = stream;
			}
			else
			{
				SslStream sslStream = new SslStream(stream, false, sslConfig.ClientCertificateValidationCallback);
				sslStream.AuthenticateAsServer(sslConfig.ServerCertificate, sslConfig.ClientCertificateRequired, sslConfig.EnabledSslProtocols, sslConfig.CheckCertificateRevocation);
				this._stream = sslStream;
			}
			this._request = HttpRequest.Read(this._stream, 90000);
			this._uri = HttpUtility.CreateRequestUrl(this._request.RequestUri, this._request.Headers["Host"], this._request.IsWebSocketRequest, secure);
			this._websocket = new WebSocket.WebSocket(this, protocol);
		}

		internal bool Authenticate(WebSocket.Net.AuthenticationSchemes scheme, string realm, Func<IIdentity, WebSocket.Net.NetworkCredential> credentialsFinder)
		{
			if (scheme == WebSocket.Net.AuthenticationSchemes.Anonymous)
			{
				return true;
			}
			if (scheme == WebSocket.Net.AuthenticationSchemes.None)
			{
				this.Close(WebSocket.Net.HttpStatusCode.Forbidden);
				return false;
			}
			string str = (new AuthenticationChallenge(scheme, realm)).ToString();
			int num = -1;
			Func<bool> func = null;
			func = () => {
				num++;
				if (num > 99)
				{
					this.Close(WebSocket.Net.HttpStatusCode.Forbidden);
					return false;
				}
				IPrincipal principal = HttpUtility.CreateUser(this._request.Headers["Authorization"], scheme, realm, this._request.HttpMethod, credentialsFinder);
				if (principal != null && principal.Identity.IsAuthenticated)
				{
					this._user = principal;
					return true;
				}
				this.SendAuthenticationChallenge(str);
				return func();
			};
			return func();
		}

		internal void Close()
		{
			this._stream.Close();
			this._tcpClient.Close();
		}

		internal void Close(WebSocket.Net.HttpStatusCode code)
		{
			this._websocket.Close(HttpResponse.CreateCloseResponse(code));
		}

		internal void SendAuthenticationChallenge(string challenge)
		{
			byte[] byteArray = HttpResponse.CreateUnauthorizedResponse(challenge).ToByteArray();
			this._stream.Write(byteArray, 0, (int)byteArray.Length);
			this._request = HttpRequest.Read(this._stream, 15000);
		}

		public override string ToString()
		{
			return this._request.ToString();
		}
	}
}