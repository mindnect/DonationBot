using System;
using System.Diagnostics;
using System.IO;

namespace WebSocket
{
	public class Logger
	{
		private volatile string _file;

		private volatile WebSocket.LogLevel _level;

		private Action<LogData, string> _output;

		private object _sync;

		public string File
		{
			get
			{
				return this._file;
			}
			set
			{
				lock (this._sync)
				{
					this._file = value;
					this.Warn(string.Format("The current path to the log file has been changed to {0}.", this._file));
				}
			}
		}

		public WebSocket.LogLevel Level
		{
			get
			{
				return (WebSocket.LogLevel)this._level;
			}
			set
			{
				lock (this._sync)
				{
					this._level = value;
					this.Warn(string.Format("The current logging level has been changed to {0}.", (WebSocket.LogLevel)this._level));
				}
			}
		}

		public Action<LogData, string> Output
		{
			get
			{
				return this._output;
			}
			set
			{
				lock (this._sync)
				{
					this._output = value ?? new Action<LogData, string>(Logger.defaultOutput);
					this.Warn("The current output action has been changed.");
				}
			}
		}

		public Logger() : this(WebSocket.LogLevel.Error, null, null)
		{
		}

		public Logger(WebSocket.LogLevel level) : this(level, null, null)
		{
		}

		public Logger(WebSocket.LogLevel level, string file, Action<LogData, string> output)
		{
			this._level = level;
			this._file = file;
			this._output = output ?? new Action<LogData, string>(Logger.defaultOutput);
			this._sync = new object();
		}

		public void Debug(string message)
		{
			if (this._level > WebSocket.LogLevel.Debug)
			{
				return;
			}
			this.output(message, WebSocket.LogLevel.Debug);
		}

		private static void defaultOutput(LogData data, string path)
		{
			string str = data.ToString();
			Console.WriteLine(str);
			if (path != null && path.Length > 0)
			{
				Logger.writeToFile(str, path);
			}
		}

		public void Error(string message)
		{
			if (this._level > WebSocket.LogLevel.Error)
			{
				return;
			}
			this.output(message, WebSocket.LogLevel.Error);
		}

		public void Fatal(string message)
		{
			this.output(message, WebSocket.LogLevel.Fatal);
		}

		public void Info(string message)
		{
			if (this._level > WebSocket.LogLevel.Info)
			{
				return;
			}
			this.output(message, WebSocket.LogLevel.Info);
		}

		private void output(string message, WebSocket.LogLevel level)
		{
			lock (this._sync)
			{
				if (this._level <= level)
				{
					LogData logDatum = null;
					try
					{
						logDatum = new LogData(level, new StackFrame(2, true), message);
						this._output(logDatum, this._file);
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						logDatum = new LogData(WebSocket.LogLevel.Fatal, new StackFrame(0, true), exception.Message);
						Console.WriteLine(logDatum.ToString());
					}
				}
			}
		}

		public void Trace(string message)
		{
			if (this._level > WebSocket.LogLevel.Trace)
			{
				return;
			}
			this.output(message, WebSocket.LogLevel.Trace);
		}

		public void Warn(string message)
		{
			if (this._level > WebSocket.LogLevel.Warn)
			{
				return;
			}
			this.output(message, WebSocket.LogLevel.Warn);
		}

		private static void writeToFile(string value, string path)
		{
			using (StreamWriter streamWriter = new StreamWriter(path, true))
			{
				using (TextWriter textWriter = TextWriter.Synchronized(streamWriter))
				{
					textWriter.WriteLine(value);
				}
			}
		}
	}
}