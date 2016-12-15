using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;
using WebSocket;
using WebSocket.Net.WebSockets;

namespace WebSocket.Server
{
	public class WebSocketSessionManager
	{
		private volatile bool _clean;

		private object _forSweep;

		private Logger _logger;

		private Dictionary<string, IWebSocketSession> _sessions;

		private volatile ServerState _state;

		private volatile bool _sweeping;

		private System.Timers.Timer _sweepTimer;

		private object _sync;

		private TimeSpan _waitTime;

		public IEnumerable<string> ActiveIDs
		{
			get
			{
				foreach (KeyValuePair<string, bool> keyValuePair in this.Broadping(WebSocketFrame.EmptyPingBytes, this._waitTime))
				{
					if (!keyValuePair.Value)
					{
						continue;
					}
					yield return keyValuePair.Key;
				}
			}
		}

		public int Count
		{
			get
			{
				int count;
				lock (this._sync)
				{
					count = this._sessions.Count;
				}
				return count;
			}
		}

		public IEnumerable<string> IDs
		{
			get
			{
				IEnumerable<string> list;
				if (this._state == ServerState.ShuttingDown)
				{
					return new string[0];
				}
				lock (this._sync)
				{
					list = this._sessions.Keys.ToList<string>();
				}
				return list;
			}
		}

		public IEnumerable<string> InactiveIDs
		{
			get
			{
				foreach (KeyValuePair<string, bool> keyValuePair in this.Broadping(WebSocketFrame.EmptyPingBytes, this._waitTime))
				{
					if (keyValuePair.Value)
					{
						continue;
					}
					yield return keyValuePair.Key;
				}
			}
		}

		public IWebSocketSession this[string id]
		{
			get
			{
				IWebSocketSession webSocketSession;
				this.TryGetSession(id, out webSocketSession);
				return webSocketSession;
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
				if (value ^ this._clean)
				{
					return;
				}
				this._clean = value;
				if (this._state == ServerState.Start)
				{
					this._sweepTimer.Enabled = value;
				}
			}
		}

		public IEnumerable<IWebSocketSession> Sessions
		{
			get
			{
				IEnumerable<IWebSocketSession> list;
				if (this._state == ServerState.ShuttingDown)
				{
					return new IWebSocketSession[0];
				}
				lock (this._sync)
				{
					list = this._sessions.Values.ToList<IWebSocketSession>();
				}
				return list;
			}
		}

		internal ServerState State
		{
			get
			{
				return (ServerState)this._state;
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
				if (value == this._waitTime)
				{
					return;
				}
				this._waitTime = value;
				foreach (IWebSocketSession session in this.Sessions)
				{
					session.Context.WebSocket.WaitTime = value;
				}
			}
		}

		internal WebSocketSessionManager() : this(new Logger())
		{
		}

		internal WebSocketSessionManager(Logger logger)
		{
			this._logger = logger;
			this._clean = true;
			this._forSweep = new object();
			this._sessions = new Dictionary<string, IWebSocketSession>();
			this._state = ServerState.Ready;
			this._sync = ((ICollection)this._sessions).SyncRoot;
			this._waitTime = TimeSpan.FromSeconds(1);
			this.setSweepTimer(60000);
		}

		internal string Add(IWebSocketSession session)
		{
			string str;
			lock (this._sync)
			{
				if (this._state == ServerState.Start)
				{
					string str1 = WebSocketSessionManager.createID();
					this._sessions.Add(str1, session);
					str = str1;
				}
				else
				{
					str = null;
				}
			}
			return str;
		}

		private void broadcast(Opcode opcode, byte[] data, Action completed)
		{
			Dictionary<CompressionMethod, byte[]> compressionMethods = new Dictionary<CompressionMethod, byte[]>();
			try
			{
				try
				{
					this.Broadcast(opcode, data, compressionMethods);
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
					this.Broadcast(opcode, stream, compressionMethods);
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

		internal void Broadcast(Opcode opcode, byte[] data, Dictionary<CompressionMethod, byte[]> cache)
		{
			foreach (IWebSocketSession session in this.Sessions)
			{
				if (this._state == ServerState.Start)
				{
					session.Context.WebSocket.Send(opcode, data, cache);
				}
				else
				{
					return;
				}
			}
		}

		internal void Broadcast(Opcode opcode, Stream stream, Dictionary<CompressionMethod, Stream> cache)
		{
			foreach (IWebSocketSession session in this.Sessions)
			{
				if (this._state == ServerState.Start)
				{
					session.Context.WebSocket.Send(opcode, stream, cache);
				}
				else
				{
					return;
				}
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

		internal Dictionary<string, bool> Broadping(byte[] frameAsBytes, TimeSpan timeout)
		{
			Dictionary<string, bool> strs = new Dictionary<string, bool>();
			foreach (IWebSocketSession session in this.Sessions)
			{
				if (this._state == ServerState.Start)
				{
					strs.Add(session.ID, session.Context.WebSocket.Ping(frameAsBytes, timeout));
				}
				else
				{
					return strs;
				}
			}
			return strs;
		}

		public Dictionary<string, bool> Broadping()
		{
			string str = ((ServerState)this._state).CheckIfAvailable(false, true, false);
			if (str != null)
			{
				this._logger.Error(str);
				return null;
			}
			return this.Broadping(WebSocketFrame.EmptyPingBytes, this._waitTime);
		}

		public Dictionary<string, bool> Broadping(string message)
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
			return this.Broadping(WebSocketFrame.CreatePingFrame(numArray, false).ToArray(), this._waitTime);
		}

		public void CloseSession(string id)
		{
			IWebSocketSession webSocketSession;
			if (this.TryGetSession(id, out webSocketSession))
			{
				webSocketSession.Context.WebSocket.Close();
			}
		}

		public void CloseSession(string id, ushort code, string reason)
		{
			IWebSocketSession webSocketSession;
			if (this.TryGetSession(id, out webSocketSession))
			{
				webSocketSession.Context.WebSocket.Close(code, reason);
			}
		}

		public void CloseSession(string id, CloseStatusCode code, string reason)
		{
			IWebSocketSession webSocketSession;
			if (this.TryGetSession(id, out webSocketSession))
			{
				webSocketSession.Context.WebSocket.Close(code, reason);
			}
		}

		private static string createID()
		{
			return Guid.NewGuid().ToString("N");
		}

		public bool PingTo(string id)
		{
			IWebSocketSession webSocketSession;
			if (!this.TryGetSession(id, out webSocketSession))
			{
				return false;
			}
			return webSocketSession.Context.WebSocket.Ping();
		}

		public bool PingTo(string message, string id)
		{
			IWebSocketSession webSocketSession;
			if (!this.TryGetSession(id, out webSocketSession))
			{
				return false;
			}
			return webSocketSession.Context.WebSocket.Ping(message);
		}

		internal bool Remove(string id)
		{
			bool flag;
			lock (this._sync)
			{
				flag = this._sessions.Remove(id);
			}
			return flag;
		}

		public void SendTo(byte[] data, string id)
		{
			IWebSocketSession webSocketSession;
			if (this.TryGetSession(id, out webSocketSession))
			{
				webSocketSession.Context.WebSocket.Send(data);
			}
		}

		public void SendTo(string data, string id)
		{
			IWebSocketSession webSocketSession;
			if (this.TryGetSession(id, out webSocketSession))
			{
				webSocketSession.Context.WebSocket.Send(data);
			}
		}

		public void SendToAsync(byte[] data, string id, Action<bool> completed)
		{
			IWebSocketSession webSocketSession;
			if (this.TryGetSession(id, out webSocketSession))
			{
				webSocketSession.Context.WebSocket.SendAsync(data, completed);
			}
		}

		public void SendToAsync(string data, string id, Action<bool> completed)
		{
			IWebSocketSession webSocketSession;
			if (this.TryGetSession(id, out webSocketSession))
			{
				webSocketSession.Context.WebSocket.SendAsync(data, completed);
			}
		}

		public void SendToAsync(Stream stream, int length, string id, Action<bool> completed)
		{
			IWebSocketSession webSocketSession;
			if (this.TryGetSession(id, out webSocketSession))
			{
				webSocketSession.Context.WebSocket.SendAsync(stream, length, completed);
			}
		}

		private void setSweepTimer(double interval)
		{
			this._sweepTimer = new System.Timers.Timer(interval);
			this._sweepTimer.Elapsed += new ElapsedEventHandler((object sender, ElapsedEventArgs e) => this.Sweep());
		}

		internal void Start()
		{
			lock (this._sync)
			{
				this._sweepTimer.Enabled = this._clean;
				this._state = ServerState.Start;
			}
		}

		internal void Stop(CloseEventArgs e, byte[] frameAsBytes, bool receive)
		{
			lock (this._sync)
			{
				this._state = ServerState.ShuttingDown;
				this._sweepTimer.Enabled = false;
				foreach (IWebSocketSession list in this._sessions.Values.ToList<IWebSocketSession>())
				{
					list.Context.WebSocket.Close(e, frameAsBytes, receive);
				}
				this._state = ServerState.Stop;
			}
		}

		public void Sweep()
		{
			IWebSocketSession webSocketSession;
			if (this._state != ServerState.Start || this._sweeping || this.Count == 0)
			{
				return;
			}
			lock (this._forSweep)
			{
				this._sweeping = true;
				foreach (string inactiveID in this.InactiveIDs)
				{
					if (this._state == ServerState.Start)
					{
						lock (this._sync)
						{
							if (this._sessions.TryGetValue(inactiveID, out webSocketSession))
							{
								WebSocketState state = webSocketSession.State;
								if (state == WebSocketState.Open)
								{
									webSocketSession.Context.WebSocket.Close(CloseStatusCode.ProtocolError);
									continue;
								}
								else if (state != WebSocketState.Closing)
								{
									this._sessions.Remove(inactiveID);
								}
								else
								{
									continue;
								}
							}
						}
					}
					else
					{
						goto Label0;
					}
				}
			Label0:
				this._sweeping = false;
			}
		}

		private bool tryGetSession(string id, out IWebSocketSession session)
		{
			bool flag;
			lock (this._sync)
			{
				flag = this._sessions.TryGetValue(id, out session);
			}
			if (!flag)
			{
				this._logger.Error(string.Concat("A session with the specified ID isn't found:\n  ID: ", id));
			}
			return flag;
		}

		public bool TryGetSession(string id, out IWebSocketSession session)
		{
			string str = ((ServerState)this._state).CheckIfAvailable(false, true, false) ?? id.CheckIfValidSessionID();
			if (str == null)
			{
				return this.tryGetSession(id, out session);
			}
			this._logger.Error(str);
			session = null;
			return false;
		}
	}
}