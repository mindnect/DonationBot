using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using WebSocket;
using WebSocket.Net;

namespace WebSocket.Server
{
	public class WebSocketServiceManager
	{
		private volatile bool _clean;

		private Dictionary<string, WebSocketServiceHost> _hosts;

		private Logger _logger;

		private volatile ServerState _state;

		private object _sync;

		private TimeSpan _waitTime;

		public int Count
		{
			get
			{
				int count;
				lock (this._sync)
				{
					count = this._hosts.Count;
				}
				return count;
			}
		}

		public IEnumerable<WebSocketServiceHost> Hosts
		{
			get
			{
				IEnumerable<WebSocketServiceHost> list;
				lock (this._sync)
				{
					list = this._hosts.Values.ToList<WebSocketServiceHost>();
				}
				return list;
			}
		}

		public WebSocketServiceHost this[string path]
		{
			get
			{
				WebSocketServiceHost webSocketServiceHost;
				this.TryGetServiceHost(path, out webSocketServiceHost);
				return webSocketServiceHost;
			}
		}

		public bool KeepClean
		{
			get
			{
				return this._clean;
			}
			internal set
			{
				lock (this._sync)
				{
					if (value ^ this._clean)
					{
						this._clean = value;
						foreach (WebSocketServiceHost webSocketServiceHost in this._hosts.Values)
						{
							webSocketServiceHost.KeepClean = value;
						}
					}
				}
			}
		}

		public IEnumerable<string> Paths
		{
			get
			{
				IEnumerable<string> list;
				lock (this._sync)
				{
					list = this._hosts.Keys.ToList<string>();
				}
				return list;
			}
		}

		public int SessionCount
		{
			get
			{
				int count = 0;
				foreach (WebSocketServiceHost host in this.Hosts)
				{
					if (this._state == ServerState.Start)
					{
						count = count + host.Sessions.Count;
					}
					else
					{
						return count;
					}
				}
				return count;
			}
		}

		public TimeSpan WaitTime
		{
			get
			{
				return this._waitTime;
			}
			internal set
			{
				lock (this._sync)
				{
					if (value != this._waitTime)
					{
						this._waitTime = value;
						foreach (WebSocketServiceHost webSocketServiceHost in this._hosts.Values)
						{
							webSocketServiceHost.WaitTime = value;
						}
					}
				}
			}
		}

		internal WebSocketServiceManager() : this(new Logger())
		{
		}

		internal WebSocketServiceManager(Logger logger)
		{
			this._logger = logger;
			this._clean = true;
			this._hosts = new Dictionary<string, WebSocketServiceHost>();
			this._state = ServerState.Ready;
			this._sync = ((ICollection)this._hosts).SyncRoot;
			this._waitTime = TimeSpan.FromSeconds(1);
		}

		internal void Add<TBehavior>(string path, Func<TBehavior> initializer)
		where TBehavior : WebSocketBehavior
		{
			WebSocketServiceHost webSocketServiceHost;
			lock (this._sync)
			{
				path = HttpUtility.UrlDecode(path).TrimEndSlash();
				if (!this._hosts.TryGetValue(path, out webSocketServiceHost))
				{
					webSocketServiceHost = new WebSocketServiceHost<TBehavior>(path, initializer, this._logger);
					if (!this._clean)
					{
						webSocketServiceHost.KeepClean = false;
					}
					if (this._waitTime != webSocketServiceHost.WaitTime)
					{
						webSocketServiceHost.WaitTime = this._waitTime;
					}
					if (this._state == ServerState.Start)
					{
						webSocketServiceHost.Start();
					}
					this._hosts.Add(path, webSocketServiceHost);
				}
				else
				{
					this._logger.Error(string.Concat("A WebSocket service with the specified path already exists:\n  path: ", path));
				}
			}
		}

		private void broadcast(Opcode opcode, byte[] data, Action completed)
		{
			Dictionary<CompressionMethod, byte[]> compressionMethods = new Dictionary<CompressionMethod, byte[]>();
			try
			{
				try
				{
					foreach (WebSocketServiceHost host in this.Hosts)
					{
						if (this._state != ServerState.Start)
						{
							break;
						}
						host.Sessions.Broadcast(opcode, data, compressionMethods);
					}
					if (completed != null)
					{
						completed();
					}
				}
				catch (Exception exception)
				{
					this._logger.Fatal(exception.ToString());
				}
			}
			finally
			{
				compressionMethods.Clear();
			}
		}

		private void broadcast(Opcode opcode, Stream stream, Action completed)
		{
			Dictionary<CompressionMethod, Stream> compressionMethods = new Dictionary<CompressionMethod, Stream>();
			try
			{
				try
				{
					foreach (WebSocketServiceHost host in this.Hosts)
					{
						if (this._state != ServerState.Start)
						{
							break;
						}
						host.Sessions.Broadcast(opcode, stream, compressionMethods);
					}
					if (completed != null)
					{
						completed();
					}
				}
				catch (Exception exception)
				{
					this._logger.Fatal(exception.ToString());
				}
			}
			finally
			{
				foreach (Stream value in compressionMethods.Values)
				{
					value.Dispose();
				}
				compressionMethods.Clear();
			}
		}

		public void Broadcast(byte[] data)
		{
			string str = ((ServerState)this._state).CheckIfAvailable(false, true, false) ?? WebSocket.CheckSendParameter(data);
			if (str != null)
			{
				this._logger.Error(str);
				return;
			}
			if ((long)data.Length <= (long)WebSocket.FragmentLength)
			{
				this.broadcast(Opcode.Binary, data, null);
				return;
			}
			this.broadcast(Opcode.Binary, new MemoryStream(data), null);
		}

		public void Broadcast(string data)
		{
			string str = ((ServerState)this._state).CheckIfAvailable(false, true, false) ?? WebSocket.CheckSendParameter(data);
			if (str != null)
			{
				this._logger.Error(str);
				return;
			}
			byte[] numArray = data.UTF8Encode();
			if ((long)numArray.Length <= (long)WebSocket.FragmentLength)
			{
				this.broadcast(Opcode.Text, numArray, null);
				return;
			}
			this.broadcast(Opcode.Text, new MemoryStream(numArray), null);
		}

		private void broadcastAsync(Opcode opcode, byte[] data, Action completed)
		{
			ThreadPool.QueueUserWorkItem((object state) => this.broadcast(opcode, data, completed));
		}

		private void broadcastAsync(Opcode opcode, Stream stream, Action completed)
		{
			ThreadPool.QueueUserWorkItem((object state) => this.broadcast(opcode, stream, completed));
		}

		public void BroadcastAsync(byte[] data, Action completed)
		{
			string str = ((ServerState)this._state).CheckIfAvailable(false, true, false) ?? WebSocket.CheckSendParameter(data);
			if (str != null)
			{
				this._logger.Error(str);
				return;
			}
			if ((long)data.Length <= (long)WebSocket.FragmentLength)
			{
				this.broadcastAsync(Opcode.Binary, data, completed);
				return;
			}
			this.broadcastAsync(Opcode.Binary, new MemoryStream(data), completed);
		}

		public void BroadcastAsync(string data, Action completed)
		{
			string str = ((ServerState)this._state).CheckIfAvailable(false, true, false) ?? WebSocket.CheckSendParameter(data);
			if (str != null)
			{
				this._logger.Error(str);
				return;
			}
			byte[] numArray = data.UTF8Encode();
			if ((long)numArray.Length <= (long)WebSocket.FragmentLength)
			{
				this.broadcastAsync(Opcode.Text, numArray, completed);
				return;
			}
			this.broadcastAsync(Opcode.Text, new MemoryStream(numArray), completed);
		}

		public void BroadcastAsync(Stream stream, int length, Action completed)
		{
			string str = ((ServerState)this._state).CheckIfAvailable(false, true, false) ?? WebSocket.CheckSendParameters(stream, length);
			if (str != null)
			{
				this._logger.Error(str);
				return;
			}
			stream.ReadBytesAsync(length, (byte[] data) => {
				int num = (int)data.Length;
				if (num == 0)
				{
					this._logger.Error("The data cannot be read from 'stream'.");
					return;
				}
				if (num < length)
				{
					this._logger.Warn(string.Format("The data with 'length' cannot be read from 'stream':\n  expected: {0}\n  actual: {1}", length, num));
				}
				if (num <= WebSocket.FragmentLength)
				{
					this.broadcast(Opcode.Binary, data, completed);
					return;
				}
				this.broadcast(Opcode.Binary, new MemoryStream(data), completed);
			}, (Exception ex) => this._logger.Fatal(ex.ToString()));
		}

		private Dictionary<string, Dictionary<string, bool>> broadping(byte[] frameAsBytes, TimeSpan timeout)
		{
			Dictionary<string, Dictionary<string, bool>> strs = new Dictionary<string, Dictionary<string, bool>>();
			foreach (WebSocketServiceHost host in this.Hosts)
			{
				if (this._state == ServerState.Start)
				{
					strs.Add(host.Path, host.Sessions.Broadping(frameAsBytes, timeout));
				}
				else
				{
					return strs;
				}
			}
			return strs;
		}

		public Dictionary<string, Dictionary<string, bool>> Broadping()
		{
			string str = ((ServerState)this._state).CheckIfAvailable(false, true, false);
			if (str != null)
			{
				this._logger.Error(str);
				return null;
			}
			return this.broadping(WebSocketFrame.EmptyPingBytes, this._waitTime);
		}

		public Dictionary<string, Dictionary<string, bool>> Broadping(string message)
		{
			if (message == null || message.Length == 0)
			{
				return this.Broadping();
			}
			byte[] numArray = null;
			string str = ((ServerState)this._state).CheckIfAvailable(false, true, false) ?? WebSocket.CheckPingParameter(message, out numArray);
			if (str != null)
			{
				this._logger.Error(str);
				return null;
			}
			return this.broadping(WebSocketFrame.CreatePingFrame(numArray, false).ToArray(), this._waitTime);
		}

		internal bool InternalTryGetServiceHost(string path, out WebSocketServiceHost host)
		{
			bool flag;
			lock (this._sync)
			{
				path = HttpUtility.UrlDecode(path).TrimEndSlash();
				flag = this._hosts.TryGetValue(path, out host);
			}
			if (!flag)
			{
				this._logger.Error(string.Concat("A WebSocket service with the specified path isn't found:\n  path: ", path));
			}
			return flag;
		}

		internal bool Remove(string path)
		{
			WebSocketServiceHost webSocketServiceHost;
			bool flag;
			lock (this._sync)
			{
				path = HttpUtility.UrlDecode(path).TrimEndSlash();
				if (this._hosts.TryGetValue(path, out webSocketServiceHost))
				{
					this._hosts.Remove(path);
					if (webSocketServiceHost.State == ServerState.Start)
					{
						webSocketServiceHost.Stop(1001, null);
					}
					return true;
				}
				else
				{
					this._logger.Error(string.Concat("A WebSocket service with the specified path isn't found:\n  path: ", path));
					flag = false;
				}
			}
			return flag;
		}

		internal void Start()
		{
			lock (this._sync)
			{
				foreach (WebSocketServiceHost value in this._hosts.Values)
				{
					value.Start();
				}
				this._state = ServerState.Start;
			}
		}

		internal void Stop(CloseEventArgs e, bool send, bool receive)
		{
			byte[] array;
			lock (this._sync)
			{
				this._state = ServerState.ShuttingDown;
				if (send)
				{
					array = WebSocketFrame.CreateCloseFrame(e.PayloadData, false).ToArray();
				}
				else
				{
					array = null;
				}
				byte[] numArray = array;
				foreach (WebSocketServiceHost value in this._hosts.Values)
				{
					value.Sessions.Stop(e, numArray, receive);
				}
				this._hosts.Clear();
				this._state = ServerState.Stop;
			}
		}

		public bool TryGetServiceHost(string path, out WebSocketServiceHost host)
		{
			string str = ((ServerState)this._state).CheckIfAvailable(false, true, false) ?? path.CheckIfValidServicePath();
			if (str == null)
			{
				return this.InternalTryGetServiceHost(path, out host);
			}
			this._logger.Error(str);
			host = null;
			return false;
		}
	}
}