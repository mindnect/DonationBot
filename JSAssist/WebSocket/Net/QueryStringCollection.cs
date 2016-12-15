using System;
using System.Collections.Specialized;
using System.Text;

namespace WebSocket.Net
{
	internal sealed class QueryStringCollection : NameValueCollection
	{
		public QueryStringCollection()
		{
		}

		public override string ToString()
		{
			int i;
			if (this.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			string[] allKeys = this.AllKeys;
			for (i = 0; i < (int)allKeys.Length; i++)
			{
				string str = allKeys[i];
				stringBuilder.AppendFormat("{0}={1}&", str, base[str]);
			}
			if (stringBuilder.Length > 0)
			{
				StringBuilder stringBuilder1 = stringBuilder;
				i = stringBuilder1.Length;
				stringBuilder1.Length = i - 1;
			}
			return stringBuilder.ToString();
		}
	}
}