using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using WebSocket;

namespace WebSocket.Net
{
	internal sealed class EndPointManager
	{
		private readonly static Dictionary<IPEndPoint, EndPointListener> _endpoints;

		static EndPointManager()
		{
			EndPointManager._endpoints = new Dictionary<IPEndPoint, EndPointListener>();
		}

		private EndPointManager()
		{
		}

		public static void AddListener(WebSocket.Net.HttpListener listener)
		{
			List<string> strs = new List<string>();
			lock (((ICollection)EndPointManager._endpoints).SyncRoot)
			{
				try
				{
					foreach (string prefix in listener.Prefixes)
					{
						EndPointManager.addPrefix(prefix, listener);
						strs.Add(prefix);
					}
				}
				catch
				{
					foreach (string str in strs)
					{
						EndPointManager.removePrefix(str, listener);
					}
					throw;
				}
			}
		}

		private static void addPrefix(string uriPrefix, WebSocket.Net.HttpListener listener)
		{
			int num;
			EndPointListener endPointListener;
			HttpListenerPrefix httpListenerPrefix = new HttpListenerPrefix(uriPrefix);
			IPAddress pAddress = EndPointManager.convertToIPAddress(httpListenerPrefix.Host);
			if (!pAddress.IsLocal())
			{
				throw new WebSocket.Net.HttpListenerException(87, "Includes an invalid host.");
			}
			if (!int.TryParse(httpListenerPrefix.Port, out num))
			{
				throw new WebSocket.Net.HttpListenerException(87, "Includes an invalid port.");
			}
			if (!num.IsPortNumber())
			{
				throw new WebSocket.Net.HttpListenerException(87, "Includes an invalid port.");
			}
			string path = httpListenerPrefix.Path;
			if (path.IndexOf('%') != -1)
			{
				throw new WebSocket.Net.HttpListenerException(87, "Includes an invalid path.");
			}
			if (path.IndexOf("//", StringComparison.Ordinal) != -1)
			{
				throw new WebSocket.Net.HttpListenerException(87, "Includes an invalid path.");
			}
			IPEndPoint pEndPoint = new IPEndPoint(pAddress, num);
			if (!EndPointManager._endpoints.TryGetValue(pEndPoint, out endPointListener))
			{
				endPointListener = new EndPointListener(pEndPoint, httpListenerPrefix.IsSecure, listener.CertificateFolderPath, listener.SslConfiguration, listener.ReuseAddress);
				EndPointManager._endpoints.Add(pEndPoint, endPointListener);
			}
			else if (endPointListener.IsSecure ^ httpListenerPrefix.IsSecure)
			{
				throw new WebSocket.Net.HttpListenerException(87, "Includes an invalid scheme.");
			}
			endPointListener.AddPrefix(httpListenerPrefix, listener);
		}

		public static void AddPrefix(string uriPrefix, WebSocket.Net.HttpListener listener)
		{
			lock (((ICollection)EndPointManager._endpoints).SyncRoot)
			{
				EndPointManager.addPrefix(uriPrefix, listener);
			}
		}

		private static IPAddress convertToIPAddress(string hostname)
		{
			if (hostname == "*" || hostname == "+")
			{
				return IPAddress.Any;
			}
			return hostname.ToIPAddress();
		}

		internal static bool RemoveEndPoint(IPEndPoint endpoint)
		{
			EndPointListener endPointListener;
			bool flag;
			lock (((ICollection)EndPointManager._endpoints).SyncRoot)
			{
				if (EndPointManager._endpoints.TryGetValue(endpoint, out endPointListener))
				{
					EndPointManager._endpoints.Remove(endpoint);
					endPointListener.Close();
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		public static void RemoveListener(WebSocket.Net.HttpListener listener)
		{
			lock (((ICollection)EndPointManager._endpoints).SyncRoot)
			{
				foreach (string prefix in listener.Prefixes)
				{
					EndPointManager.removePrefix(prefix, listener);
				}
			}
		}

		private static void removePrefix(string uriPrefix, WebSocket.Net.HttpListener listener)
		{
			int num;
			EndPointListener endPointListener;
			HttpListenerPrefix httpListenerPrefix = new HttpListenerPrefix(uriPrefix);
			IPAddress pAddress = EndPointManager.convertToIPAddress(httpListenerPrefix.Host);
			if (!pAddress.IsLocal())
			{
				return;
			}
			if (!int.TryParse(httpListenerPrefix.Port, out num))
			{
				return;
			}
			if (!num.IsPortNumber())
			{
				return;
			}
			string path = httpListenerPrefix.Path;
			if (path.IndexOf('%') != -1)
			{
				return;
			}
			if (path.IndexOf("//", StringComparison.Ordinal) != -1)
			{
				return;
			}
			IPEndPoint pEndPoint = new IPEndPoint(pAddress, num);
			if (!EndPointManager._endpoints.TryGetValue(pEndPoint, out endPointListener))
			{
				return;
			}
			if (endPointListener.IsSecure ^ httpListenerPrefix.IsSecure)
			{
				return;
			}
			endPointListener.RemovePrefix(httpListenerPrefix, listener);
		}

		public static void RemovePrefix(string uriPrefix, WebSocket.Net.HttpListener listener)
		{
			lock (((ICollection)EndPointManager._endpoints).SyncRoot)
			{
				EndPointManager.removePrefix(uriPrefix, listener);
			}
		}
	}
}