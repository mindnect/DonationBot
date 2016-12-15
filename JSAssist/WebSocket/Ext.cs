using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using WebSocket.Net;
using WebSocket.Net.WebSockets;
using WebSocket.Server;

namespace WebSocket
{
	public static class Ext
	{
		private readonly static byte[] _last;

		private readonly static int _retry;

		private const string _tspecials = "()<>@,;:\\\"/[]?={} \t";

		static Ext()
		{
			Ext._last = new byte[1];
			Ext._retry = 5;
		}

		internal static byte[] Append(this ushort code, string reason)
		{
			byte[] byteArray = code.InternalToByteArray(ByteOrder.Big);
			if (reason != null && reason.Length > 0)
			{
				List<byte> nums = new List<byte>(byteArray);
				nums.AddRange(Encoding.UTF8.GetBytes(reason));
				byteArray = nums.ToArray();
			}
			return byteArray;
		}

		internal static string CheckIfAvailable(this ServerState state, bool ready, bool start, bool shutting)
		{
			if ((ready || state != ServerState.Ready && state != ServerState.Stop) && (start || state != ServerState.Start) && (shutting || state != ServerState.ShuttingDown))
			{
				return null;
			}
			return string.Concat("This operation isn't available in: ", state.ToString().ToLower());
		}

		internal static string CheckIfAvailable(this WebSocketState state, bool connecting, bool open, bool closing, bool closed)
		{
			if ((connecting || state != WebSocketState.Connecting) && (open || state != WebSocketState.Open) && (closing || state != WebSocketState.Closing) && (closed || state != WebSocketState.Closed))
			{
				return null;
			}
			return string.Concat("This operation isn't available in: ", state.ToString().ToLower());
		}

		internal static string CheckIfValidProtocols(this string[] protocols)
		{
			if (protocols.Contains<string>((string protocol) => {
				if (protocol == null || protocol.Length == 0)
				{
					return true;
				}
				return !protocol.IsToken();
			}))
			{
				return "Contains an invalid value.";
			}
			if (!protocols.ContainsTwice())
			{
				return null;
			}
			return "Contains a value twice.";
		}

		internal static string CheckIfValidServicePath(this string path)
		{
			if (path == null || path.Length == 0)
			{
				return "'path' is null or empty.";
			}
			if (path[0] != '/')
			{
				return "'path' isn't an absolute path.";
			}
			if (path.IndexOfAny(new char[] { '?', '#' }) <= -1)
			{
				return null;
			}
			return "'path' includes either or both query and fragment components.";
		}

		internal static string CheckIfValidSessionID(this string id)
		{
			if (id != null && id.Length != 0)
			{
				return null;
			}
			return "'id' is null or empty.";
		}

		internal static string CheckIfValidWaitTime(this TimeSpan time)
		{
			if (time > TimeSpan.Zero)
			{
				return null;
			}
			return "A wait time is zero or less.";
		}

		internal static bool CheckWaitTime(this TimeSpan time, out string message)
		{
			message = null;
			if (time > TimeSpan.Zero)
			{
				return true;
			}
			message = "A wait time is zero or less.";
			return false;
		}

		internal static void Close(this WebSocket.Net.HttpListenerResponse response, WebSocket.Net.HttpStatusCode code)
		{
			response.StatusCode = (int)code;
			response.OutputStream.Close();
		}

		internal static void CloseWithAuthChallenge(this WebSocket.Net.HttpListenerResponse response, string challenge)
		{
			response.Headers.InternalSet("WWW-Authenticate", challenge, true);
			response.Close(WebSocket.Net.HttpStatusCode.Unauthorized);
		}

		private static byte[] compress(this byte[] data)
		{
			byte[] array;
			if ((long)data.Length == 0)
			{
				return data;
			}
			using (MemoryStream memoryStream = new MemoryStream(data))
			{
				array = memoryStream.compressToArray();
			}
			return array;
		}

		private static MemoryStream compress(this Stream stream)
		{
			MemoryStream memoryStream;
			MemoryStream memoryStream1 = new MemoryStream();
			if (stream.Length == 0)
			{
				return memoryStream1;
			}
			stream.Position = (long)0;
			using (DeflateStream deflateStream = new DeflateStream(memoryStream1, CompressionMode.Compress, true))
			{
				stream.CopyTo(deflateStream, 1024);
				deflateStream.Close();
				memoryStream1.Write(Ext._last, 0, 1);
				memoryStream1.Position = (long)0;
				memoryStream = memoryStream1;
			}
			return memoryStream;
		}

		internal static byte[] Compress(this byte[] data, CompressionMethod method)
		{
			if (method != CompressionMethod.Deflate)
			{
				return data;
			}
			return data.compress();
		}

		internal static Stream Compress(this Stream stream, CompressionMethod method)
		{
			if (method != CompressionMethod.Deflate)
			{
				return stream;
			}
			return stream.compress();
		}

		private static byte[] compressToArray(this Stream stream)
		{
			byte[] array;
			using (MemoryStream memoryStream = stream.compress())
			{
				memoryStream.Close();
				array = memoryStream.ToArray();
			}
			return array;
		}

		internal static byte[] CompressToArray(this Stream stream, CompressionMethod method)
		{
			if (method != CompressionMethod.Deflate)
			{
				return stream.ToByteArray();
			}
			return stream.compressToArray();
		}

		internal static bool Contains<T>(this IEnumerable<T> source, Func<T, bool> condition)
		{
			bool flag;
			using (IEnumerator<T> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!condition(enumerator.Current))
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			return flag;
		}

		public static bool Contains(this string value, params char[] chars)
		{
			if (chars == null || chars.Length == 0)
			{
				return true;
			}
			if (value == null || value.Length == 0)
			{
				return false;
			}
			return value.IndexOfAny(chars) > -1;
		}

		public static bool Contains(this NameValueCollection collection, string name)
		{
			if (collection == null || collection.Count <= 0)
			{
				return false;
			}
			return collection[name] != null;
		}

		public static bool Contains(this NameValueCollection collection, string name, string value)
		{
			if (collection == null || collection.Count == 0)
			{
				return false;
			}
			string item = collection[name];
			if (item == null)
			{
				return false;
			}
			string[] strArrays = item.Split(new char[] { ',' });
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				if (strArrays[i].Trim().Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		internal static bool ContainsTwice(this string[] values)
		{
			int length = (int)values.Length;
			Func<int, bool> func = null;
			func = (int idx) => {
				if (idx >= this.len - 1)
				{
					return false;
				}
				for (int i = idx + 1; i < this.len; i++)
				{
					if (this.values[i] == this.values[idx])
					{
						return true;
					}
				}
				int num = idx + 1;
				idx = num;
				return this.contains(num);
			};
			return func(0);
		}

		internal static T[] Copy<T>(this T[] source, long length)
		{
			T[] tArray = new T[checked((IntPtr)length)];
			Array.Copy(source, (long)0, tArray, (long)0, length);
			return tArray;
		}

		internal static void CopyTo(this Stream source, Stream destination, int bufferLength)
		{
			byte[] numArray = new byte[bufferLength];
			int num = 0;
			while (true)
			{
				int num1 = source.Read(numArray, 0, bufferLength);
				num = num1;
				if (num1 <= 0)
				{
					break;
				}
				destination.Write(numArray, 0, num);
			}
		}

		internal static void CopyToAsync(this Stream source, Stream destination, int bufferLength, Action completed, Action<Exception> error)
		{
			byte[] numArray = new byte[bufferLength];
			AsyncCallback asyncCallback = null;
			asyncCallback = (IAsyncResult ar) => {
				try
				{
					int num = this.source.EndRead(ar);
					if (num > 0)
					{
						this.destination.Write(this.buff, 0, num);
						this.source.BeginRead(this.buff, 0, this.bufferLength, this.callback, null);
					}
					else if (this.completed != null)
					{
						this.completed();
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					if (this.error != null)
					{
						this.error(exception);
					}
				}
			};
			try
			{
				source.BeginRead(numArray, 0, bufferLength, asyncCallback, null);
			}
			catch (Exception exception3)
			{
				Exception exception2 = exception3;
				if (error != null)
				{
					error(exception2);
				}
			}
		}

		private static byte[] decompress(this byte[] data)
		{
			byte[] array;
			if ((long)data.Length == 0)
			{
				return data;
			}
			using (MemoryStream memoryStream = new MemoryStream(data))
			{
				array = memoryStream.decompressToArray();
			}
			return array;
		}

		private static MemoryStream decompress(this Stream stream)
		{
			MemoryStream memoryStream;
			MemoryStream memoryStream1 = new MemoryStream();
			if (stream.Length == 0)
			{
				return memoryStream1;
			}
			stream.Position = (long)0;
			using (DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress, true))
			{
				deflateStream.CopyTo(memoryStream1, 1024);
				memoryStream1.Position = (long)0;
				memoryStream = memoryStream1;
			}
			return memoryStream;
		}

		internal static byte[] Decompress(this byte[] data, CompressionMethod method)
		{
			if (method != CompressionMethod.Deflate)
			{
				return data;
			}
			return data.decompress();
		}

		internal static Stream Decompress(this Stream stream, CompressionMethod method)
		{
			if (method != CompressionMethod.Deflate)
			{
				return stream;
			}
			return stream.decompress();
		}

		private static byte[] decompressToArray(this Stream stream)
		{
			byte[] array;
			using (MemoryStream memoryStream = stream.decompress())
			{
				memoryStream.Close();
				array = memoryStream.ToArray();
			}
			return array;
		}

		internal static byte[] DecompressToArray(this Stream stream, CompressionMethod method)
		{
			if (method != CompressionMethod.Deflate)
			{
				return stream.ToByteArray();
			}
			return stream.decompressToArray();
		}

		public static void Emit(this EventHandler eventHandler, object sender, EventArgs e)
		{
			if (eventHandler != null)
			{
				eventHandler(sender, e);
			}
		}

		public static void Emit<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object sender, TEventArgs e)
		where TEventArgs : EventArgs
		{
			if (eventHandler != null)
			{
				eventHandler(sender, e);
			}
		}

		internal static bool EqualsWith(this int value, char c, Action<int> action)
		{
			action(value);
			return value == c;
		}

		internal static string GetAbsolutePath(this Uri uri)
		{
			if (uri.IsAbsoluteUri)
			{
				return uri.AbsolutePath;
			}
			string originalString = uri.OriginalString;
			if (originalString[0] != '/')
			{
				return null;
			}
			int num = originalString.IndexOfAny(new char[] { '?', '#' });
			if (num <= 0)
			{
				return originalString;
			}
			return originalString.Substring(0, num);
		}

		public static WebSocket.Net.CookieCollection GetCookies(this NameValueCollection headers, bool response)
		{
			string str = (response ? "Set-Cookie" : "Cookie");
			if (headers == null || !headers.Contains(str))
			{
				return new WebSocket.Net.CookieCollection();
			}
			return WebSocket.Net.CookieCollection.Parse(headers[str], response);
		}

		public static string GetDescription(this WebSocket.Net.HttpStatusCode code)
		{
			return ((int)code).GetStatusDescription();
		}

		internal static string GetMessage(this CloseStatusCode code)
		{
			if (code == CloseStatusCode.ProtocolError)
			{
				return "A WebSocket protocol error has occurred.";
			}
			if (code == CloseStatusCode.UnsupportedData)
			{
				return "Unsupported data has been received.";
			}
			if (code == CloseStatusCode.Abnormal)
			{
				return "An exception has occurred.";
			}
			if (code == CloseStatusCode.InvalidData)
			{
				return "Invalid data has been received.";
			}
			if (code == CloseStatusCode.PolicyViolation)
			{
				return "A policy violation has occurred.";
			}
			if (code == CloseStatusCode.TooBig)
			{
				return "A too big message has been received.";
			}
			if (code == CloseStatusCode.MandatoryExtension)
			{
				return "WebSocket client didn't receive expected extension(s).";
			}
			if (code == CloseStatusCode.ServerError)
			{
				return "WebSocket server got an internal error.";
			}
			if (code != CloseStatusCode.TlsHandshakeFailure)
			{
				return string.Empty;
			}
			return "An error has occurred during a TLS handshake.";
		}

		internal static string GetName(this string nameAndValue, char separator)
		{
			int num = nameAndValue.IndexOf(separator);
			if (num <= 0)
			{
				return null;
			}
			return nameAndValue.Substring(0, num).Trim();
		}

		public static string GetStatusDescription(this int code)
		{
			if (code > 207)
			{
				switch (code)
				{
					case 300:
					{
						return "Multiple Choices";
					}
					case 301:
					{
						return "Moved Permanently";
					}
					case 302:
					{
						return "Found";
					}
					case 303:
					{
						return "See Other";
					}
					case 304:
					{
						return "Not Modified";
					}
					case 305:
					{
						return "Use Proxy";
					}
					case 306:
					{
						break;
					}
					case 307:
					{
						return "Temporary Redirect";
					}
					default:
					{
						switch (code)
						{
							case 400:
							{
								return "Bad Request";
							}
							case 401:
							{
								return "Unauthorized";
							}
							case 402:
							{
								return "Payment Required";
							}
							case 403:
							{
								return "Forbidden";
							}
							case 404:
							{
								return "Not Found";
							}
							case 405:
							{
								return "Method Not Allowed";
							}
							case 406:
							{
								return "Not Acceptable";
							}
							case 407:
							{
								return "Proxy Authentication Required";
							}
							case 408:
							{
								return "Request Timeout";
							}
							case 409:
							{
								return "Conflict";
							}
							case 410:
							{
								return "Gone";
							}
							case 411:
							{
								return "Length Required";
							}
							case 412:
							{
								return "Precondition Failed";
							}
							case 413:
							{
								return "Request Entity Too Large";
							}
							case 414:
							{
								return "Request-Uri Too Long";
							}
							case 415:
							{
								return "Unsupported Media Type";
							}
							case 416:
							{
								return "Requested Range Not Satisfiable";
							}
							case 417:
							{
								return "Expectation Failed";
							}
							case 418:
							case 419:
							case 420:
							case 421:
							{
								break;
							}
							case 422:
							{
								return "Unprocessable Entity";
							}
							case 423:
							{
								return "Locked";
							}
							case 424:
							{
								return "Failed Dependency";
							}
							default:
							{
								switch (code)
								{
									case 500:
									{
										return "Internal Server Error";
									}
									case 501:
									{
										return "Not Implemented";
									}
									case 502:
									{
										return "Bad Gateway";
									}
									case 503:
									{
										return "Service Unavailable";
									}
									case 504:
									{
										return "Gateway Timeout";
									}
									case 505:
									{
										return "Http Version Not Supported";
									}
									case 507:
									{
										return "Insufficient Storage";
									}
								}
								break;
							}
						}
						break;
					}
				}
			}
			else
			{
				switch (code)
				{
					case 100:
					{
						return "Continue";
					}
					case 101:
					{
						return "Switching Protocols";
					}
					case 102:
					{
						return "Processing";
					}
					default:
					{
						switch (code)
						{
							case 200:
							{
								return "OK";
							}
							case 201:
							{
								return "Created";
							}
							case 202:
							{
								return "Accepted";
							}
							case 203:
							{
								return "Non-Authoritative Information";
							}
							case 204:
							{
								return "No Content";
							}
							case 205:
							{
								return "Reset Content";
							}
							case 206:
							{
								return "Partial Content";
							}
							case 207:
							{
								return "Multi-Status";
							}
						}
						break;
					}
				}
			}
			return string.Empty;
		}

		internal static string GetValue(this string nameAndValue, char separator)
		{
			int num = nameAndValue.IndexOf(separator);
			if (num <= -1 || num >= nameAndValue.Length - 1)
			{
				return null;
			}
			return nameAndValue.Substring(num + 1).Trim();
		}

		internal static string GetValue(this string nameAndValue, char separator, bool unquote)
		{
			int num = nameAndValue.IndexOf(separator);
			if (num < 0 || num == nameAndValue.Length - 1)
			{
				return null;
			}
			string str = nameAndValue.Substring(num + 1).Trim();
			if (!unquote)
			{
				return str;
			}
			return str.Unquote();
		}

		internal static TcpListenerWebSocketContext GetWebSocketContext(this TcpClient tcpClient, string protocol, bool secure, ServerSslConfiguration sslConfig, Logger logger)
		{
			return new TcpListenerWebSocketContext(tcpClient, protocol, secure, sslConfig, logger);
		}

		internal static byte[] InternalToByteArray(this ushort value, ByteOrder order)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (!order.IsHostOrder())
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}

		internal static byte[] InternalToByteArray(this ulong value, ByteOrder order)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (!order.IsHostOrder())
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}

		public static bool IsCloseStatusCode(this ushort value)
		{
			if (value <= 999)
			{
				return false;
			}
			return value < 5000;
		}

		internal static bool IsCompressionExtension(this string value, CompressionMethod method)
		{
			return value.StartsWith(method.ToExtensionString(new string[0]));
		}

		internal static bool IsControl(this byte opcode)
		{
			if (opcode <= 7)
			{
				return false;
			}
			return opcode < 16;
		}

		internal static bool IsControl(this Opcode opcode)
		{
			return opcode >= Opcode.Close;
		}

		internal static bool IsData(this byte opcode)
		{
			if (opcode == 1)
			{
				return true;
			}
			return opcode == 2;
		}

		internal static bool IsData(this Opcode opcode)
		{
			if (opcode == Opcode.Text)
			{
				return true;
			}
			return opcode == Opcode.Binary;
		}

		public static bool IsEnclosedIn(this string value, char c)
		{
			if (value == null || value.Length <= 1 || value[0] != c)
			{
				return false;
			}
			return value[value.Length - 1] == c;
		}

		public static bool IsHostOrder(this ByteOrder order)
		{
			return BitConverter.IsLittleEndian == order == ByteOrder.Little;
		}

		public static bool IsLocal(this IPAddress address)
		{
			if (address == null)
			{
				return false;
			}
			if (address.Equals(IPAddress.Any))
			{
				return true;
			}
			if (address.Equals(IPAddress.Loopback))
			{
				return true;
			}
			if (Socket.OSSupportsIPv6)
			{
				if (address.Equals(IPAddress.IPv6Any))
				{
					return true;
				}
				if (address.Equals(IPAddress.IPv6Loopback))
				{
					return true;
				}
			}
			IPAddress[] hostAddresses = Dns.GetHostAddresses(Dns.GetHostName());
			for (int i = 0; i < (int)hostAddresses.Length; i++)
			{
				if (address.Equals(hostAddresses[i]))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsNullOrEmpty(this string value)
		{
			if (value == null)
			{
				return true;
			}
			return value.Length == 0;
		}

		internal static bool IsPortNumber(this int value)
		{
			if (value <= 0)
			{
				return false;
			}
			return value < 65536;
		}

		public static bool IsPredefinedScheme(this string value)
		{
			if (value == null || value.Length < 2)
			{
				return false;
			}
			char chr = value[0];
			if (chr == 'h')
			{
				if (value == "http")
				{
					return true;
				}
				return value == "https";
			}
			if (chr == 'w')
			{
				if (value == "ws")
				{
					return true;
				}
				return value == "wss";
			}
			if (chr == 'f')
			{
				if (value == "file")
				{
					return true;
				}
				return value == "ftp";
			}
			if (chr != 'n')
			{
				if (chr == 'g' && value == "gopher")
				{
					return true;
				}
				if (chr != 'm')
				{
					return false;
				}
				return value == "mailto";
			}
			chr = value[1];
			if (chr != 'e')
			{
				return value == "nntp";
			}
			if (value == "news" || value == "net.pipe")
			{
				return true;
			}
			return value == "net.tcp";
		}

		internal static bool IsReserved(this ushort code)
		{
			if (code == 1004 || code == 1005 || code == 1006)
			{
				return true;
			}
			return code == 1015;
		}

		internal static bool IsReserved(this CloseStatusCode code)
		{
			if (code == CloseStatusCode.Undefined || code == CloseStatusCode.NoStatus || code == CloseStatusCode.Abnormal)
			{
				return true;
			}
			return code == CloseStatusCode.TlsHandshakeFailure;
		}

		internal static bool IsSupported(this byte opcode)
		{
			return Enum.IsDefined(typeof(Opcode), opcode);
		}

		internal static bool IsText(this string value)
		{
			int length = value.Length;
			for (int i = 0; i < length; i++)
			{
				char chr = value[i];
				if (chr < ' ')
				{
					if (!"\r\n\t".Contains(new char[] { chr }))
					{
						return false;
					}
				}
				if (chr == '\u007F')
				{
					return false;
				}
				if (chr == '\n')
				{
					int num = i + 1;
					i = num;
					if (num < length)
					{
						chr = value[i];
						if (!" \t".Contains(new char[] { chr }))
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		internal static bool IsToken(this string value)
		{
			string str = value;
			int num = 0;
			while (true)
			{
				if (num >= str.Length)
				{
					return true;
				}
				char chr = str[num];
				if (chr < ' ' || chr >= '\u007F')
				{
					break;
				}
				if ("()<>@,;:\\\"/[]?={} \t".Contains(new char[] { chr }))
				{
					break;
				}
				num++;
			}
			return false;
		}

		public static bool IsUpgradeTo(this WebSocket.Net.HttpListenerRequest request, string protocol)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			if (protocol == null)
			{
				throw new ArgumentNullException("protocol");
			}
			if (protocol.Length == 0)
			{
				throw new ArgumentException("An empty string.", "protocol");
			}
			if (!request.Headers.Contains("Upgrade", protocol))
			{
				return false;
			}
			return request.Headers.Contains("Connection", "Upgrade");
		}

		public static bool MaybeUri(this string value)
		{
			if (value == null || value.Length == 0)
			{
				return false;
			}
			int num = value.IndexOf(':');
			if (num == -1)
			{
				return false;
			}
			if (num >= 10)
			{
				return false;
			}
			return value.Substring(0, num).IsPredefinedScheme();
		}

		internal static string Quote(this string value)
		{
			return string.Format("\"{0}\"", value.Replace("\"", "\\\""));
		}

		internal static byte[] ReadBytes(this Stream stream, int length)
		{
			byte[] numArray = new byte[length];
			int num = 0;
			try
			{
				int num1 = 0;
				while (length > 0)
				{
					num1 = stream.Read(numArray, num, length);
					if (num1 == 0)
					{
						break;
					}
					num = num + num1;
					length = length - num1;
				}
			}
			catch
			{
			}
			return numArray.SubArray<byte>(0, num);
		}

		internal static byte[] ReadBytes(this Stream stream, long length, int bufferLength)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				try
				{
					byte[] numArray = new byte[bufferLength];
					int num = 0;
					while (length > (long)0)
					{
						if (length < (long)bufferLength)
						{
							bufferLength = (int)length;
						}
						num = stream.Read(numArray, 0, bufferLength);
						if (num == 0)
						{
							break;
						}
						memoryStream.Write(numArray, 0, num);
						length = length - (long)num;
					}
				}
				catch
				{
				}
				memoryStream.Close();
				array = memoryStream.ToArray();
			}
			return array;
		}

		internal static void ReadBytesAsync(this Stream stream, int length, Action<byte[]> completed, Action<Exception> error)
		{
			byte[] numArray = new byte[length];
			int num1 = 0;
			AsyncCallback asyncCallback = null;
			asyncCallback = (IAsyncResult ar) => {
				try
				{
					int num = this.stream.EndRead(ar);
					if (num == 0 && this.retry < Ext._retry)
					{
						this.retry = this.retry + 1;
						this.stream.BeginRead(this.buff, this.offset, this.length, this.callback, null);
					}
					else if (num != 0 && num != this.length)
					{
						this.retry = 0;
						this.offset = this.offset + num;
						this.length = this.length - num;
						this.stream.BeginRead(this.buff, this.offset, this.length, this.callback, null);
					}
					else if (this.completed != null)
					{
						this.completed(this.buff.SubArray<byte>(0, this.offset + num));
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					if (this.error != null)
					{
						this.error(exception);
					}
				}
			};
			try
			{
				stream.BeginRead(numArray, num1, length, asyncCallback, null);
			}
			catch (Exception exception3)
			{
				Exception exception2 = exception3;
				if (error != null)
				{
					error(exception2);
				}
			}
		}

		internal static void ReadBytesAsync(this Stream stream, long length, int bufferLength, Action<byte[]> completed, Action<Exception> error)
		{
			MemoryStream memoryStream = new MemoryStream();
			byte[] numArray = new byte[bufferLength];
			Action<long> action = null;
			action = (long len) => {
				if (len < (long)this.bufferLength)
				{
					this.bufferLength = (int)len;
				}
				this.stream.BeginRead(this.buff, 0, this.bufferLength, (IAsyncResult ar) => {
					try
					{
						int num = this.stream.EndRead(ar);
						if (num > 0)
						{
							this.dest.Write(this.buff, 0, num);
						}
						if (num == 0 && this.retry < Ext._retry)
						{
							int cSu0024u003cu003e8_locals1 = this.retry;
							this.retry = cSu0024u003cu003e8_locals1 + 1;
							this.read(len);
						}
						else if (num == 0 || (long)num == len)
						{
							if (this.completed != null)
							{
								this.dest.Close();
								this.completed(this.dest.ToArray());
							}
							this.dest.Dispose();
						}
						else
						{
							this.retry = 0;
							this.read(len - (long)num);
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						this.dest.Dispose();
						if (this.error != null)
						{
							this.error(exception);
						}
					}
				}, null);
			};
			try
			{
				action(length);
			}
			catch (Exception exception3)
			{
				Exception exception2 = exception3;
				memoryStream.Dispose();
				if (error != null)
				{
					error(exception2);
				}
			}
		}

		internal static string RemovePrefix(this string value, params string[] prefixes)
		{
			int length = 0;
			string[] strArrays = prefixes;
			int num = 0;
			while (num < (int)strArrays.Length)
			{
				string str = strArrays[num];
				if (!value.StartsWith(str))
				{
					num++;
				}
				else
				{
					length = str.Length;
					break;
				}
			}
			if (length <= 0)
			{
				return value;
			}
			return value.Substring(length);
		}

		internal static T[] Reverse<T>(this T[] array)
		{
			int length = (int)array.Length;
			T[] tArray = new T[length];
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				tArray[i] = array[num - i];
			}
			return tArray;
		}

		internal static IEnumerable<string> SplitHeaderValue(string value, params char[] separators)
		{
			int length = value.Length;
			string str = new string(separators);
			StringBuilder stringBuilder = new StringBuilder(32);
			bool flag = false;
			bool flag1 = false;
			for (int i = 0; i < length; i++)
			{
				char chr = value[i];
				if (chr == '\"')
				{
					if (!flag)
					{
						flag1 = !flag1;
					}
					else
					{
						flag = !flag;
					}
				}
				else if (chr != '\\')
				{
					string str1 = str;
					char[] chrArray = new char[] { chr };
					if (!str1.Contains(chrArray) || flag1)
					{
						goto Label1;
					}
					yield return stringBuilder.ToString();
					stringBuilder.Length = 0;
					goto Label0;
				}
				else if (i < length - 1 && value[i + 1] == '\"')
				{
					flag = true;
				}
			Label1:
				stringBuilder.Append(chr);
			Label0:
			}
			if (stringBuilder.Length > 0)
			{
				yield return stringBuilder.ToString();
			}
		}

		public static T[] SubArray<T>(this T[] array, int startIndex, int length)
		{
			if (array != null)
			{
				int num = (int)array.Length;
				int num1 = num;
				if (num != 0)
				{
					if (startIndex < 0 || length <= 0 || startIndex + length > num1)
					{
						return new T[0];
					}
					if (startIndex == 0 && length == num1)
					{
						return array;
					}
					T[] tArray = new T[length];
					Array.Copy(array, startIndex, tArray, 0, length);
					return tArray;
				}
			}
			return new T[0];
		}

		public static T[] SubArray<T>(this T[] array, long startIndex, long length)
		{
			if (array != null)
			{
				long num = (long)array.Length;
				long num1 = num;
				if (num != 0)
				{
					if (startIndex < (long)0 || length <= (long)0 || startIndex + length > num1)
					{
						return new T[0];
					}
					if (startIndex == 0 && length == num1)
					{
						return array;
					}
					T[] tArray = new T[checked((IntPtr)length)];
					Array.Copy(array, startIndex, tArray, (long)0, length);
					return tArray;
				}
			}
			return new T[0];
		}

		private static void times(this ulong n, Action action)
		{
			for (ulong i = (ulong)0; i < n; i = i + (long)1)
			{
				action();
			}
		}

		public static void Times(this int n, Action action)
		{
			if (n > 0 && action != null)
			{
				((ulong)n).times(action);
			}
		}

		public static void Times(this long n, Action action)
		{
			if (n > (long)0 && action != null)
			{
				((ulong)n).times(action);
			}
		}

		public static void Times(this uint n, Action action)
		{
			if (n > 0 && action != null)
			{
				((ulong)n).times(action);
			}
		}

		public static void Times(this ulong n, Action action)
		{
			if (n > (long)0 && action != null)
			{
				n.times(action);
			}
		}

		public static void Times(this int n, Action<int> action)
		{
			if (n > 0 && action != null)
			{
				for (int i = 0; i < n; i++)
				{
					action(i);
				}
			}
		}

		public static void Times(this long n, Action<long> action)
		{
			if (n > (long)0 && action != null)
			{
				for (long i = (long)0; i < n; i = i + (long)1)
				{
					action(i);
				}
			}
		}

		public static void Times(this uint n, Action<uint> action)
		{
			if (n > 0 && action != null)
			{
				for (uint i = 0; i < n; i++)
				{
					action(i);
				}
			}
		}

		public static void Times(this ulong n, Action<ulong> action)
		{
			if (n > (long)0 && action != null)
			{
				for (ulong i = (ulong)0; i < n; i = i + (long)1)
				{
					action(i);
				}
			}
		}

		public static T To<T>(this byte[] source, ByteOrder sourceOrder)
		where T : struct
		{
			T t;
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (source.Length == 0)
			{
				t = default(T);
				return t;
			}
			Type type = typeof(T);
			byte[] hostOrder = source.ToHostOrder(sourceOrder);
			if (type == typeof(bool))
			{
				return (T)(object)BitConverter.ToBoolean(hostOrder, 0);
			}
			if (type == typeof(char))
			{
				return (T)(object)BitConverter.ToChar(hostOrder, 0);
			}
			if (type == typeof(double))
			{
				return (T)(object)BitConverter.ToDouble(hostOrder, 0);
			}
			if (type == typeof(short))
			{
				return (T)(object)BitConverter.ToInt16(hostOrder, 0);
			}
			if (type == typeof(int))
			{
				return (T)(object)BitConverter.ToInt32(hostOrder, 0);
			}
			if (type == typeof(long))
			{
				return (T)(object)BitConverter.ToInt64(hostOrder, 0);
			}
			if (type == typeof(float))
			{
				return (T)(object)BitConverter.ToSingle(hostOrder, 0);
			}
			if (type == typeof(ushort))
			{
				return (T)(object)BitConverter.ToUInt16(hostOrder, 0);
			}
			if (type == typeof(uint))
			{
				return (T)(object)BitConverter.ToUInt32(hostOrder, 0);
			}
			if (type != typeof(ulong))
			{
				t = default(T);
				return t;
			}
			return (T)(object)BitConverter.ToUInt64(hostOrder, 0);
		}

		internal static byte[] ToByteArray(this Stream stream)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				stream.Position = (long)0;
				stream.CopyTo(memoryStream, 1024);
				memoryStream.Close();
				array = memoryStream.ToArray();
			}
			return array;
		}

		public static byte[] ToByteArray<T>(this T value, ByteOrder order)
		where T : struct
		{
			byte[] bytes;
			Type type = typeof(T);
			if (type == typeof(bool))
			{
				bytes = BitConverter.GetBytes((bool)(object)value);
			}
			else if (type == typeof(byte))
			{
				bytes = new byte[] { (byte)(object)value };
			}
			else if (type == typeof(char))
			{
				bytes = BitConverter.GetBytes((char)(object)value);
			}
			else if (type == typeof(double))
			{
				bytes = BitConverter.GetBytes((double)(object)value);
			}
			else if (type == typeof(short))
			{
				bytes = BitConverter.GetBytes((short)(object)value);
			}
			else if (type == typeof(int))
			{
				bytes = BitConverter.GetBytes((int)(object)value);
			}
			else if (type == typeof(long))
			{
				bytes = BitConverter.GetBytes((long)(object)value);
			}
			else if (type == typeof(float))
			{
				bytes = BitConverter.GetBytes((float)(object)value);
			}
			else if (type == typeof(ushort))
			{
				bytes = BitConverter.GetBytes((ushort)(object)value);
			}
			else if (type == typeof(uint))
			{
				bytes = BitConverter.GetBytes((uint)(object)value);
			}
			else
			{
				bytes = (type == typeof(ulong) ? BitConverter.GetBytes((ulong)(object)value) : WebSocket.EmptyBytes);
			}
			byte[] numArray = bytes;
			if ((int)numArray.Length > 1 && !order.IsHostOrder())
			{
				Array.Reverse(numArray);
			}
			return numArray;
		}

		internal static CompressionMethod ToCompressionMethod(this string value)
		{
			CompressionMethod compressionMethod;
			IEnumerator enumerator = Enum.GetValues(typeof(CompressionMethod)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					CompressionMethod current = (CompressionMethod)enumerator.Current;
					if (current.ToExtensionString(new string[0]) != value)
					{
						continue;
					}
					compressionMethod = current;
					return compressionMethod;
				}
				return CompressionMethod.None;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return compressionMethod;
		}

		internal static string ToExtensionString(this CompressionMethod method, params string[] parameters)
		{
			if (method == CompressionMethod.None)
			{
				return string.Empty;
			}
			string str = string.Format("permessage-{0}", method.ToString().ToLower());
			if (parameters == null || parameters.Length == 0)
			{
				return str;
			}
			return string.Format("{0}; {1}", str, parameters.ToString<string>("; "));
		}

		public static byte[] ToHostOrder(this byte[] source, ByteOrder sourceOrder)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if ((int)source.Length <= 1 || sourceOrder.IsHostOrder())
			{
				return source;
			}
			return source.Reverse<byte>();
		}

		internal static IPAddress ToIPAddress(this string hostnameOrAddress)
		{
			IPAddress pAddress;
			IPAddress hostAddresses;
			if (IPAddress.TryParse(hostnameOrAddress, out pAddress))
			{
				return pAddress;
			}
			try
			{
				hostAddresses = Dns.GetHostAddresses(hostnameOrAddress)[0];
			}
			catch
			{
				hostAddresses = null;
			}
			return hostAddresses;
		}

		internal static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
		{
			return new List<TSource>(source);
		}

		public static string ToString<T>(this T[] array, string separator)
		{
			string empty = separator;
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			int length = (int)array.Length;
			if (length == 0)
			{
				return string.Empty;
			}
			if (empty == null)
			{
				empty = string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(64);
			(length - 1).Times((int i) => stringBuilder.AppendFormat("{0}{1}", array[i].ToString(), empty));
			stringBuilder.Append(array[length - 1].ToString());
			return stringBuilder.ToString();
		}

		internal static ushort ToUInt16(this byte[] source, ByteOrder sourceOrder)
		{
			return BitConverter.ToUInt16(source.ToHostOrder(sourceOrder), 0);
		}

		internal static ulong ToUInt64(this byte[] source, ByteOrder sourceOrder)
		{
			return BitConverter.ToUInt64(source.ToHostOrder(sourceOrder), 0);
		}

		public static Uri ToUri(this string uriString)
		{
			Uri uri;
			Uri.TryCreate(uriString, (uriString.MaybeUri() ? UriKind.Absolute : UriKind.Relative), out uri);
			return uri;
		}

		internal static string TrimEndSlash(this string value)
		{
			value = value.TrimEnd(new char[] { '/' });
			if (value.Length <= 0)
			{
				return "/";
			}
			return value;
		}

		internal static bool TryCreateWebSocketUri(this string uriString, out Uri result, out string message)
		{
			Uri uri;
			result = null;
			Uri uri1 = uriString.ToUri();
			if (uri1 == null)
			{
				message = string.Concat("An invalid URI string: ", uriString);
				return false;
			}
			if (!uri1.IsAbsoluteUri)
			{
				message = string.Concat("Not an absolute URI: ", uriString);
				return false;
			}
			string scheme = uri1.Scheme;
			if (!(scheme == "ws") && !(scheme == "wss"))
			{
				message = string.Concat("The scheme part isn't 'ws' or 'wss': ", uriString);
				return false;
			}
			if (uri1.Fragment.Length > 0)
			{
				message = string.Concat("Includes the fragment component: ", uriString);
				return false;
			}
			int port = uri1.Port;
			if (port == 0)
			{
				message = string.Concat("The port part is zero: ", uriString);
				return false;
			}
			if (port != -1)
			{
				uri = uri1;
			}
			else
			{
				object[] host = new object[] { scheme, uri1.Host, null, null };
				host[2] = (scheme == "ws" ? 80 : 443);
				host[3] = uri1.PathAndQuery;
				uri = new Uri(string.Format("{0}://{1}:{2}{3}", host));
			}
			result = uri;
			message = string.Empty;
			return true;
		}

		internal static string Unquote(this string value)
		{
			int num = value.IndexOf('\"');
			if (num < 0)
			{
				return value;
			}
			int num1 = value.LastIndexOf('\"') - num - 1;
			if (num1 < 0)
			{
				return value;
			}
			if (num1 == 0)
			{
				return string.Empty;
			}
			return value.Substring(num + 1, num1).Replace("\\\"", "\"");
		}

		public static string UrlDecode(this string value)
		{
			if (value == null || value.Length <= 0)
			{
				return value;
			}
			return HttpUtility.UrlDecode(value);
		}

		public static string UrlEncode(this string value)
		{
			if (value == null || value.Length <= 0)
			{
				return value;
			}
			return HttpUtility.UrlEncode(value);
		}

		internal static string UTF8Decode(this byte[] bytes)
		{
			string str;
			try
			{
				str = Encoding.UTF8.GetString(bytes);
			}
			catch
			{
				str = null;
			}
			return str;
		}

		internal static byte[] UTF8Encode(this string s)
		{
			return Encoding.UTF8.GetBytes(s);
		}

		internal static void WriteBytes(this Stream stream, byte[] bytes, int bufferLength)
		{
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				memoryStream.CopyTo(stream, bufferLength);
			}
		}

		internal static void WriteBytesAsync(this Stream stream, byte[] bytes, int bufferLength, Action completed, Action<Exception> error)
		{
			MemoryStream memoryStream = new MemoryStream(bytes);
			memoryStream.CopyToAsync(stream, bufferLength, () => {
				if (completed != null)
				{
					completed();
				}
				memoryStream.Dispose();
			}, (Exception ex) => {
				memoryStream.Dispose();
				if (error != null)
				{
					error(ex);
				}
			});
		}

		public static void WriteContent(this WebSocket.Net.HttpListenerResponse response, byte[] content)
		{
			if (response == null)
			{
				throw new ArgumentNullException("response");
			}
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			long length = (long)content.Length;
			if (length == 0)
			{
				response.Close();
				return;
			}
			response.ContentLength64 = length;
			Stream outputStream = response.OutputStream;
			if (length > (long)2147483647)
			{
				outputStream.WriteBytes(content, 1024);
			}
			else
			{
				outputStream.Write(content, 0, (int)length);
			}
			outputStream.Close();
		}
	}
}