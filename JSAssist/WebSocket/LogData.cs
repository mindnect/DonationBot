using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace WebSocket
{
	public class LogData
	{
		private StackFrame _caller;

		private DateTime _date;

		private WebSocket.LogLevel _level;

		private string _message;

		public StackFrame Caller
		{
			get
			{
				return this._caller;
			}
		}

		public DateTime Date
		{
			get
			{
				return this._date;
			}
		}

		public WebSocket.LogLevel Level
		{
			get
			{
				return this._level;
			}
		}

		public string Message
		{
			get
			{
				return this._message;
			}
		}

		internal LogData(WebSocket.LogLevel level, StackFrame caller, string message)
		{
			this._level = level;
			this._caller = caller;
			this._message = message ?? string.Empty;
			this._date = DateTime.Now;
		}

		public override string ToString()
		{
			string str = string.Format("{0}|{1,-5}|", this._date, this._level);
			MethodBase method = this._caller.GetMethod();
			Type declaringType = method.DeclaringType;
			string str1 = string.Format("{0}{1}.{2}|", str, declaringType.Name, method.Name);
			string[] strArrays = this._message.Replace("\r\n", "\n").TrimEnd(new char[] { '\n' }).Split(new char[] { '\n' });
			if ((int)strArrays.Length <= 1)
			{
				return string.Format("{0}{1}", str1, this._message);
			}
			StringBuilder stringBuilder = new StringBuilder(string.Format("{0}{1}\n", str1, strArrays[0]), 64);
			string str2 = string.Format("{{0,{0}}}{{1}}\n", str.Length);
			for (int i = 1; i < (int)strArrays.Length; i++)
			{
				stringBuilder.AppendFormat(str2, "", strArrays[i]);
			}
			StringBuilder length = stringBuilder;
			length.Length = length.Length - 1;
			return stringBuilder.ToString();
		}
	}
}