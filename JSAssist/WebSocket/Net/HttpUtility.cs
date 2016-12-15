using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Security.Principal;
using System.Text;
using WebSocket;

namespace WebSocket.Net
{
	internal sealed class HttpUtility
	{
		private static Dictionary<string, char> _entities;

		private static char[] _hexChars;

		private static object _sync;

		static HttpUtility()
		{
			HttpUtility._hexChars = "0123456789abcdef".ToCharArray();
			HttpUtility._sync = new object();
		}

		public HttpUtility()
		{
		}

		internal static Uri CreateRequestUrl(string requestUri, string host, bool websocketRequest, bool secure)
		{
			Uri uri;
			Uri uri1;
			bool flag;
			if (requestUri == null || requestUri.Length == 0 || host == null || host.Length == 0)
			{
				return null;
			}
			string str = null;
			string pathAndQuery = null;
			if (requestUri.StartsWith("/"))
			{
				pathAndQuery = requestUri;
			}
			else if (requestUri.MaybeUri())
			{
				if (!Uri.TryCreate(requestUri, UriKind.Absolute, out uri1))
				{
					flag = false;
				}
				else
				{
					string scheme = uri1.Scheme;
					str = scheme;
					flag = (!scheme.StartsWith("http") || websocketRequest ? str.StartsWith("ws") & websocketRequest : true);
				}
				if (!flag)
				{
					return null;
				}
				host = uri1.Authority;
				pathAndQuery = uri1.PathAndQuery;
			}
			else if (requestUri != "*")
			{
				host = requestUri;
			}
			if (str == null)
			{
				str = string.Concat((websocketRequest ? "ws" : "http"), (secure ? "s" : string.Empty));
			}
			if (host.IndexOf(':') == -1)
			{
				host = string.Format("{0}:{1}", host, (str == "http" || str == "ws" ? 80 : 443));
			}
			if (!Uri.TryCreate(string.Format("{0}://{1}{2}", str, host, pathAndQuery), UriKind.Absolute, out uri))
			{
				return null;
			}
			return uri;
		}

		internal static IPrincipal CreateUser(string response, AuthenticationSchemes scheme, string realm, string method, Func<IIdentity, NetworkCredential> credentialsFinder)
		{
			if (response == null || response.Length == 0)
			{
				return null;
			}
			if (credentialsFinder == null)
			{
				return null;
			}
			if (scheme != AuthenticationSchemes.Basic && scheme != AuthenticationSchemes.Digest)
			{
				return null;
			}
			if (scheme == AuthenticationSchemes.Digest)
			{
				if (realm == null || realm.Length == 0)
				{
					return null;
				}
				if (method == null || method.Length == 0)
				{
					return null;
				}
			}
			if (!response.StartsWith(scheme.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}
			AuthenticationResponse authenticationResponse = AuthenticationResponse.Parse(response);
			if (authenticationResponse == null)
			{
				return null;
			}
			IIdentity identity = authenticationResponse.ToIdentity();
			if (identity == null)
			{
				return null;
			}
			NetworkCredential networkCredential = null;
			try
			{
				networkCredential = credentialsFinder(identity);
			}
			catch
			{
			}
			if (networkCredential == null)
			{
				return null;
			}
			if (scheme == AuthenticationSchemes.Basic && ((HttpBasicIdentity)identity).Password != networkCredential.Password)
			{
				return null;
			}
			if (scheme == AuthenticationSchemes.Digest && !((HttpDigestIdentity)identity).IsValid(networkCredential.Password, realm, method, null))
			{
				return null;
			}
			return new GenericPrincipal(identity, networkCredential.Roles);
		}

		private static int getChar(byte[] bytes, int offset, int length)
		{
			int num = 0;
			int num1 = length + offset;
			for (int i = offset; i < num1; i++)
			{
				int num2 = HttpUtility.getInt(bytes[i]);
				if (num2 == -1)
				{
					return -1;
				}
				num = (num << 4) + num2;
			}
			return num;
		}

		private static int getChar(string s, int offset, int length)
		{
			int num = 0;
			int num1 = length + offset;
			for (int i = offset; i < num1; i++)
			{
				char chr = s[i];
				if (chr > '\u007F')
				{
					return -1;
				}
				int num2 = HttpUtility.getInt((byte)chr);
				if (num2 == -1)
				{
					return -1;
				}
				num = (num << 4) + num2;
			}
			return num;
		}

		private static char[] getChars(MemoryStream buffer, Encoding encoding)
		{
			return encoding.GetChars(buffer.GetBuffer(), 0, (int)buffer.Length);
		}

		internal static Encoding GetEncoding(string contentType)
		{
			string[] strArrays = contentType.Split(new char[] { ';' });
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i].Trim();
				if (str.StartsWith("charset", StringComparison.OrdinalIgnoreCase))
				{
					return Encoding.GetEncoding(str.GetValue('=', true));
				}
			}
			return null;
		}

		private static Dictionary<string, char> getEntities()
		{
			Dictionary<string, char> strs;
			lock (HttpUtility._sync)
			{
				if (HttpUtility._entities == null)
				{
					HttpUtility.initEntities();
				}
				strs = HttpUtility._entities;
			}
			return strs;
		}

		private static int getInt(byte b)
		{
			char chr = (char)b;
			if (chr >= '0' && chr <= '9')
			{
				return chr - 48;
			}
			if (chr >= 'a' && chr <= 'f')
			{
				return chr - 97 + 10;
			}
			if (chr < 'A' || chr > 'F')
			{
				return -1;
			}
			return chr - 65 + 10;
		}

		public static string HtmlAttributeEncode(string s)
		{
			string str;
			if (s == null || s.Length == 0 || !s.Contains(new char[] { '&', '\"', '<', '>' }))
			{
				return s;
			}
			StringBuilder stringBuilder = new StringBuilder();
			string str1 = s;
			for (int i = 0; i < str1.Length; i++)
			{
				char chr = str1[i];
				StringBuilder stringBuilder1 = stringBuilder;
				if (chr == '&')
				{
					str = "&amp;";
				}
				else if (chr == '\"')
				{
					str = "&quot;";
				}
				else if (chr == '<')
				{
					str = "&lt;";
				}
				else
				{
					str = (chr == '>' ? "&gt;" : chr.ToString());
				}
				stringBuilder1.Append(str);
			}
			return stringBuilder.ToString();
		}

		public static void HtmlAttributeEncode(string s, TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write(HttpUtility.HtmlAttributeEncode(s));
		}

		public static string HtmlDecode(string s)
		{
			if (s != null && s.Length != 0)
			{
				if (s.Contains(new char[] { '&' }))
				{
					StringBuilder stringBuilder = new StringBuilder();
					StringBuilder stringBuilder1 = new StringBuilder();
					int num = 0;
					int num1 = 0;
					bool flag = false;
					string str = s;
					for (int i = 0; i < str.Length; i++)
					{
						char chr = str[i];
						if (num == 0)
						{
							if (chr != '&')
							{
								stringBuilder1.Append(chr);
							}
							else
							{
								stringBuilder.Append(chr);
								num = 1;
							}
						}
						else if (chr == '&')
						{
							num = 1;
							if (flag)
							{
								stringBuilder.Append(num1.ToString(CultureInfo.InvariantCulture));
								flag = false;
							}
							stringBuilder1.Append(stringBuilder.ToString());
							stringBuilder.Length = 0;
							stringBuilder.Append('&');
						}
						else if (num == 1)
						{
							if (chr != ';')
							{
								num1 = 0;
								num = (chr == '#' ? 3 : 2);
								stringBuilder.Append(chr);
							}
							else
							{
								num = 0;
								stringBuilder1.Append(stringBuilder.ToString());
								stringBuilder1.Append(chr);
								stringBuilder.Length = 0;
							}
						}
						else if (num == 2)
						{
							stringBuilder.Append(chr);
							if (chr == ';')
							{
								string str1 = stringBuilder.ToString();
								Dictionary<string, char> entities = HttpUtility.getEntities();
								if (str1.Length > 1 && entities.ContainsKey(str1.Substring(1, str1.Length - 2)))
								{
									char item = entities[str1.Substring(1, str1.Length - 2)];
									str1 = item.ToString();
								}
								stringBuilder1.Append(str1);
								num = 0;
								stringBuilder.Length = 0;
							}
						}
						else if (num == 3)
						{
							if (chr == ';')
							{
								if (num1 <= 65535)
								{
									stringBuilder1.Append((char)num1);
								}
								else
								{
									stringBuilder1.Append("&#");
									stringBuilder1.Append(num1.ToString(CultureInfo.InvariantCulture));
									stringBuilder1.Append(";");
								}
								num = 0;
								stringBuilder.Length = 0;
								flag = false;
							}
							else if (!char.IsDigit(chr))
							{
								num = 2;
								if (flag)
								{
									stringBuilder.Append(num1.ToString(CultureInfo.InvariantCulture));
									flag = false;
								}
								stringBuilder.Append(chr);
							}
							else
							{
								num1 = num1 * 10 + (chr - 48);
								flag = true;
							}
						}
					}
					if (stringBuilder.Length > 0)
					{
						stringBuilder1.Append(stringBuilder.ToString());
					}
					else if (flag)
					{
						stringBuilder1.Append(num1.ToString(CultureInfo.InvariantCulture));
					}
					return stringBuilder1.ToString();
				}
			}
			return s;
		}

		public static void HtmlDecode(string s, TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write(HttpUtility.HtmlDecode(s));
		}

		public static string HtmlEncode(string s)
		{
			if (s == null || s.Length == 0)
			{
				return s;
			}
			bool flag = false;
			string str = s;
			int i = 0;
			while (i < str.Length)
			{
				char chr = str[i];
				if (chr == '&' || chr == '\"' || chr == '<' || chr == '>' || chr > '\u009F')
				{
					flag = true;
					break;
				}
				else
				{
					i++;
				}
			}
			if (!flag)
			{
				return s;
			}
			StringBuilder stringBuilder = new StringBuilder();
			str = s;
			for (i = 0; i < str.Length; i++)
			{
				char chr1 = str[i];
				if (chr1 == '&')
				{
					stringBuilder.Append("&amp;");
				}
				else if (chr1 == '\"')
				{
					stringBuilder.Append("&quot;");
				}
				else if (chr1 == '<')
				{
					stringBuilder.Append("&lt;");
				}
				else if (chr1 == '>')
				{
					stringBuilder.Append("&gt;");
				}
				else if (chr1 <= '\u009F')
				{
					stringBuilder.Append(chr1);
				}
				else
				{
					stringBuilder.Append("&#");
					int num = chr1;
					stringBuilder.Append(num.ToString(CultureInfo.InvariantCulture));
					stringBuilder.Append(";");
				}
			}
			return stringBuilder.ToString();
		}

		public static void HtmlEncode(string s, TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write(HttpUtility.HtmlEncode(s));
		}

		private static void initEntities()
		{
			HttpUtility._entities = new Dictionary<string, char>()
			{
				{ "nbsp", '\u00A0' },
				{ "iexcl", '¡' },
				{ "cent", '¢' },
				{ "pound", '£' },
				{ "curren", '¤' },
				{ "yen", '¥' },
				{ "brvbar", '\u00A6' },
				{ "sect", '\u00A7' },
				{ "uml", '\u00A8' },
				{ "copy", '\u00A9' },
				{ "ordf", 'ª' },
				{ "laquo", '«' },
				{ "not", '¬' },
				{ "shy", '­' },
				{ "reg", '\u00AE' },
				{ "macr", '\u00AF' },
				{ "deg", '\u00B0' },
				{ "plusmn", '±' },
				{ "sup2", '\u00B2' },
				{ "sup3", '\u00B3' },
				{ "acute", '\u00B4' },
				{ "micro", 'µ' },
				{ "para", '\u00B6' },
				{ "middot", '·' },
				{ "cedil", '\u00B8' },
				{ "sup1", '\u00B9' },
				{ "ordm", 'º' },
				{ "raquo", '»' },
				{ "frac14", '\u00BC' },
				{ "frac12", '\u00BD' },
				{ "frac34", '\u00BE' },
				{ "iquest", '¿' },
				{ "Agrave", 'À' },
				{ "Aacute", 'Á' },
				{ "Acirc", 'Â' },
				{ "Atilde", 'Ã' },
				{ "Auml", 'Ä' },
				{ "Aring", 'Å' },
				{ "AElig", 'Æ' },
				{ "Ccedil", 'Ç' },
				{ "Egrave", 'È' },
				{ "Eacute", 'É' },
				{ "Ecirc", 'Ê' },
				{ "Euml", 'Ë' },
				{ "Igrave", 'Ì' },
				{ "Iacute", 'Í' },
				{ "Icirc", 'Î' },
				{ "Iuml", 'Ï' },
				{ "ETH", 'Ð' },
				{ "Ntilde", 'Ñ' },
				{ "Ograve", 'Ò' },
				{ "Oacute", 'Ó' },
				{ "Ocirc", 'Ô' },
				{ "Otilde", 'Õ' },
				{ "Ouml", 'Ö' },
				{ "times", '×' },
				{ "Oslash", 'Ø' },
				{ "Ugrave", 'Ù' },
				{ "Uacute", 'Ú' },
				{ "Ucirc", 'Û' },
				{ "Uuml", 'Ü' },
				{ "Yacute", 'Ý' },
				{ "THORN", 'Þ' },
				{ "szlig", 'ß' },
				{ "agrave", 'à' },
				{ "aacute", 'á' },
				{ "acirc", 'â' },
				{ "atilde", 'ã' },
				{ "auml", 'ä' },
				{ "aring", 'å' },
				{ "aelig", 'æ' },
				{ "ccedil", 'ç' },
				{ "egrave", 'è' },
				{ "eacute", 'é' },
				{ "ecirc", 'ê' },
				{ "euml", 'ë' },
				{ "igrave", 'ì' },
				{ "iacute", 'í' },
				{ "icirc", 'î' },
				{ "iuml", 'ï' },
				{ "eth", 'ð' },
				{ "ntilde", 'ñ' },
				{ "ograve", 'ò' },
				{ "oacute", 'ó' },
				{ "ocirc", 'ô' },
				{ "otilde", 'õ' },
				{ "ouml", 'ö' },
				{ "divide", '÷' },
				{ "oslash", 'ø' },
				{ "ugrave", 'ù' },
				{ "uacute", 'ú' },
				{ "ucirc", 'û' },
				{ "uuml", 'ü' },
				{ "yacute", 'ý' },
				{ "thorn", 'þ' },
				{ "yuml", 'ÿ' },
				{ "fnof", 'ƒ' },
				{ "Alpha", 'Α' },
				{ "Beta", 'Β' },
				{ "Gamma", 'Γ' },
				{ "Delta", 'Δ' },
				{ "Epsilon", 'Ε' },
				{ "Zeta", 'Ζ' },
				{ "Eta", 'Η' },
				{ "Theta", 'Θ' },
				{ "Iota", 'Ι' },
				{ "Kappa", 'Κ' },
				{ "Lambda", 'Λ' },
				{ "Mu", 'Μ' },
				{ "Nu", 'Ν' },
				{ "Xi", 'Ξ' },
				{ "Omicron", 'Ο' },
				{ "Pi", 'Π' },
				{ "Rho", 'Ρ' },
				{ "Sigma", 'Σ' },
				{ "Tau", 'Τ' },
				{ "Upsilon", 'Υ' },
				{ "Phi", 'Φ' },
				{ "Chi", 'Χ' },
				{ "Psi", 'Ψ' },
				{ "Omega", 'Ω' },
				{ "alpha", 'α' },
				{ "beta", 'β' },
				{ "gamma", 'γ' },
				{ "delta", 'δ' },
				{ "epsilon", 'ε' },
				{ "zeta", 'ζ' },
				{ "eta", 'η' },
				{ "theta", 'θ' },
				{ "iota", 'ι' },
				{ "kappa", 'κ' },
				{ "lambda", 'λ' },
				{ "mu", 'μ' },
				{ "nu", 'ν' },
				{ "xi", 'ξ' },
				{ "omicron", 'ο' },
				{ "pi", 'π' },
				{ "rho", 'ρ' },
				{ "sigmaf", 'ς' },
				{ "sigma", 'σ' },
				{ "tau", 'τ' },
				{ "upsilon", 'υ' },
				{ "phi", 'φ' },
				{ "chi", 'χ' },
				{ "psi", 'ψ' },
				{ "omega", 'ω' },
				{ "thetasym", 'ϑ' },
				{ "upsih", 'ϒ' },
				{ "piv", 'ϖ' },
				{ "bull", '•' },
				{ "hellip", '…' },
				{ "prime", '′' },
				{ "Prime", '″' },
				{ "oline", '‾' },
				{ "frasl", '⁄' },
				{ "weierp", '℘' },
				{ "image", 'ℑ' },
				{ "real", 'ℜ' },
				{ "trade", '\u2122' },
				{ "alefsym", '\u2135' },
				{ "larr", '←' },
				{ "uarr", '↑' },
				{ "rarr", '→' },
				{ "darr", '↓' },
				{ "harr", '↔' },
				{ "crarr", '\u21B5' },
				{ "lArr", '\u21D0' },
				{ "uArr", '\u21D1' },
				{ "rArr", '⇒' },
				{ "dArr", '\u21D3' },
				{ "hArr", '⇔' },
				{ "forall", '∀' },
				{ "part", '∂' },
				{ "exist", '∃' },
				{ "empty", '∅' },
				{ "nabla", '∇' },
				{ "isin", '∈' },
				{ "notin", '∉' },
				{ "ni", '∋' },
				{ "prod", '∏' },
				{ "sum", '∑' },
				{ "minus", '−' },
				{ "lowast", '∗' },
				{ "radic", '√' },
				{ "prop", '∝' },
				{ "infin", '∞' },
				{ "ang", '∠' },
				{ "and", '∧' },
				{ "or", '∨' },
				{ "cap", '∩' },
				{ "cup", '∪' },
				{ "int", '∫' },
				{ "there4", '∴' },
				{ "sim", '∼' },
				{ "cong", '≅' },
				{ "asymp", '≈' },
				{ "ne", '≠' },
				{ "equiv", '≡' },
				{ "le", '≤' },
				{ "ge", '≥' },
				{ "sub", '⊂' },
				{ "sup", '⊃' },
				{ "nsub", '⊄' },
				{ "sube", '⊆' },
				{ "supe", '⊇' },
				{ "oplus", '⊕' },
				{ "otimes", '⊗' },
				{ "perp", '⊥' },
				{ "sdot", '⋅' },
				{ "lceil", '⌈' },
				{ "rceil", '⌉' },
				{ "lfloor", '⌊' },
				{ "rfloor", '⌋' },
				{ "lang", '〈' },
				{ "rang", '〉' },
				{ "loz", '\u25CA' },
				{ "spades", '\u2660' },
				{ "clubs", '\u2663' },
				{ "hearts", '\u2665' },
				{ "diams", '\u2666' },
				{ "quot", '\"' },
				{ "amp", '&' },
				{ "lt", '<' },
				{ "gt", '>' },
				{ "OElig", 'Œ' },
				{ "oelig", 'œ' },
				{ "Scaron", 'Š' },
				{ "scaron", 'š' },
				{ "Yuml", 'Ÿ' },
				{ "circ", '\u02C6' },
				{ "tilde", '\u02DC' },
				{ "ensp", '\u2002' },
				{ "emsp", '\u2003' },
				{ "thinsp", '\u2009' },
				{ "zwnj", '\u200C' },
				{ "zwj", '\u200D' },
				{ "lrm", '\u200E' },
				{ "rlm", '\u200F' },
				{ "ndash", '–' },
				{ "mdash", '—' },
				{ "lsquo", '‘' },
				{ "rsquo", '’' },
				{ "sbquo", '‚' },
				{ "ldquo", '“' },
				{ "rdquo", '”' },
				{ "bdquo", '„' },
				{ "dagger", '†' },
				{ "Dagger", '‡' },
				{ "permil", '‰' },
				{ "lsaquo", '‹' },
				{ "rsaquo", '›' },
				{ "euro", '€' }
			};
		}

		internal static NameValueCollection InternalParseQueryString(string query, Encoding encoding)
		{
			if (query != null)
			{
				int length = query.Length;
				int num = length;
				if (length != 0 && (num != 1 || query[0] != '?'))
				{
					if (query[0] == '?')
					{
						query = query.Substring(1);
					}
					QueryStringCollection queryStringCollection = new QueryStringCollection();
					string[] strArrays = query.Split(new char[] { '&' });
					for (int i = 0; i < (int)strArrays.Length; i++)
					{
						string str = strArrays[i];
						int num1 = str.IndexOf('=');
						if (num1 <= -1)
						{
							queryStringCollection.Add(null, HttpUtility.UrlDecode(str, encoding));
						}
						else
						{
							string str1 = HttpUtility.UrlDecode(str.Substring(0, num1), encoding);
							queryStringCollection.Add(str1, (str.Length > num1 + 1 ? HttpUtility.UrlDecode(str.Substring(num1 + 1), encoding) : string.Empty));
						}
					}
					return queryStringCollection;
				}
			}
			return new NameValueCollection(1);
		}

		internal static string InternalUrlDecode(byte[] bytes, int offset, int count, Encoding encoding)
		{
			int chr;
			StringBuilder stringBuilder = new StringBuilder();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int num = count + offset;
				for (int i = offset; i < num; i++)
				{
					if (bytes[i] == 37 && i + 2 < count && bytes[i + 1] != 37)
					{
						if (bytes[i + 1] != 117 || i + 5 >= num)
						{
							int chr1 = HttpUtility.getChar(bytes, i + 1, 2);
							chr = chr1;
							if (chr1 == -1)
							{
								goto Label1;
							}
							memoryStream.WriteByte((byte)chr);
							i = i + 2;
							goto Label0;
						}
						else
						{
							if (memoryStream.Length > (long)0)
							{
								stringBuilder.Append(HttpUtility.getChars(memoryStream, encoding));
								memoryStream.SetLength((long)0);
							}
							chr = HttpUtility.getChar(bytes, i + 2, 4);
							if (chr == -1)
							{
								goto Label1;
							}
							stringBuilder.Append((char)chr);
							i = i + 5;
							goto Label0;
						}
					}
				Label1:
					if (memoryStream.Length > (long)0)
					{
						stringBuilder.Append(HttpUtility.getChars(memoryStream, encoding));
						memoryStream.SetLength((long)0);
					}
					if (bytes[i] != 43)
					{
						stringBuilder.Append((char)bytes[i]);
					}
					else
					{
						stringBuilder.Append(' ');
					}
				Label0:
				}
				if (memoryStream.Length > (long)0)
				{
					stringBuilder.Append(HttpUtility.getChars(memoryStream, encoding));
				}
			}
			return stringBuilder.ToString();
		}

		internal static byte[] InternalUrlDecodeToBytes(byte[] bytes, int offset, int count)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int num = offset + count;
				for (int i = offset; i < num; i++)
				{
					char chr = (char)bytes[i];
					if (chr == '+')
					{
						chr = ' ';
					}
					else if (chr == '%' && i < num - 2)
					{
						int chr1 = HttpUtility.getChar(bytes, i + 1, 2);
						if (chr1 != -1)
						{
							chr = (char)chr1;
							i = i + 2;
						}
					}
					memoryStream.WriteByte((byte)chr);
				}
				memoryStream.Close();
				array = memoryStream.ToArray();
			}
			return array;
		}

		internal static byte[] InternalUrlEncodeToBytes(byte[] bytes, int offset, int count)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int num = offset + count;
				for (int i = offset; i < num; i++)
				{
					HttpUtility.urlEncode(bytes[i], memoryStream, false);
				}
				memoryStream.Close();
				array = memoryStream.ToArray();
			}
			return array;
		}

		internal static byte[] InternalUrlEncodeUnicodeToBytes(string s)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				string str = s;
				for (int i = 0; i < str.Length; i++)
				{
					HttpUtility.urlEncode(str[i], memoryStream, true);
				}
				memoryStream.Close();
				array = memoryStream.ToArray();
			}
			return array;
		}

		private static bool notEncoded(char c)
		{
			if (c == '!' || c == '\'' || c == '(' || c == ')' || c == '*' || c == '-' || c == '.')
			{
				return true;
			}
			return c == '\u005F';
		}

		public static NameValueCollection ParseQueryString(string query)
		{
			return HttpUtility.ParseQueryString(query, Encoding.UTF8);
		}

		public static NameValueCollection ParseQueryString(string query, Encoding encoding)
		{
			if (query == null)
			{
				throw new ArgumentNullException("query");
			}
			return HttpUtility.InternalParseQueryString(query, encoding ?? Encoding.UTF8);
		}

		public static string UrlDecode(string s)
		{
			return HttpUtility.UrlDecode(s, Encoding.UTF8);
		}

		public static string UrlDecode(string s, Encoding encoding)
		{
			int chr;
			if (s != null && s.Length != 0)
			{
				if (s.Contains(new char[] { '%', '+' }))
				{
					if (encoding == null)
					{
						encoding = Encoding.UTF8;
					}
					List<byte> nums = new List<byte>();
					int length = s.Length;
					for (int i = 0; i < length; i++)
					{
						char chr1 = s[i];
						if (chr1 == '%' && i + 2 < length && s[i + 1] != '%')
						{
							if (s[i + 1] != 'u' || i + 5 >= length)
							{
								int num = HttpUtility.getChar(s, i + 1, 2);
								chr = num;
								if (num == -1)
								{
									HttpUtility.writeCharBytes('%', nums, encoding);
								}
								else
								{
									HttpUtility.writeCharBytes((char)chr, nums, encoding);
									i = i + 2;
								}
							}
							else
							{
								chr = HttpUtility.getChar(s, i + 2, 4);
								if (chr == -1)
								{
									HttpUtility.writeCharBytes('%', nums, encoding);
								}
								else
								{
									HttpUtility.writeCharBytes((char)chr, nums, encoding);
									i = i + 5;
								}
							}
						}
						else if (chr1 != '+')
						{
							HttpUtility.writeCharBytes(chr1, nums, encoding);
						}
						else
						{
							HttpUtility.writeCharBytes(' ', nums, encoding);
						}
					}
					return encoding.GetString(nums.ToArray());
				}
			}
			return s;
		}

		public static string UrlDecode(byte[] bytes, Encoding encoding)
		{
			if (bytes == null)
			{
				return null;
			}
			int length = (int)bytes.Length;
			int num = length;
			if (length == 0)
			{
				return string.Empty;
			}
			return HttpUtility.InternalUrlDecode(bytes, 0, num, encoding ?? Encoding.UTF8);
		}

		public static string UrlDecode(byte[] bytes, int offset, int count, Encoding encoding)
		{
			if (bytes == null)
			{
				return null;
			}
			int length = (int)bytes.Length;
			if (length == 0 || count == 0)
			{
				return string.Empty;
			}
			if (offset < 0 || offset >= length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || count > length - offset)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			return HttpUtility.InternalUrlDecode(bytes, offset, count, encoding ?? Encoding.UTF8);
		}

		public static byte[] UrlDecodeToBytes(byte[] bytes)
		{
			if (bytes != null)
			{
				int length = (int)bytes.Length;
				int num = length;
				if (length > 0)
				{
					return HttpUtility.InternalUrlDecodeToBytes(bytes, 0, num);
				}
			}
			return bytes;
		}

		public static byte[] UrlDecodeToBytes(string s)
		{
			return HttpUtility.UrlDecodeToBytes(s, Encoding.UTF8);
		}

		public static byte[] UrlDecodeToBytes(string s, Encoding encoding)
		{
			if (s == null)
			{
				return null;
			}
			if (s.Length == 0)
			{
				return new byte[0];
			}
			byte[] bytes = (encoding ?? Encoding.UTF8).GetBytes(s);
			return HttpUtility.InternalUrlDecodeToBytes(bytes, 0, (int)bytes.Length);
		}

		public static byte[] UrlDecodeToBytes(byte[] bytes, int offset, int count)
		{
			if (bytes != null)
			{
				int length = (int)bytes.Length;
				int num = length;
				if (length != 0)
				{
					if (count == 0)
					{
						return new byte[0];
					}
					if (offset < 0 || offset >= num)
					{
						throw new ArgumentOutOfRangeException("offset");
					}
					if (count < 0 || count > num - offset)
					{
						throw new ArgumentOutOfRangeException("count");
					}
					return HttpUtility.InternalUrlDecodeToBytes(bytes, offset, count);
				}
			}
			return bytes;
		}

		private static void urlEncode(char c, Stream result, bool unicode)
		{
			if (c > 'ÿ')
			{
				result.WriteByte(37);
				result.WriteByte(117);
				char chr = c;
				result.WriteByte((byte)HttpUtility._hexChars[chr >> '\f']);
				int num = chr >> '\b' & 15;
				result.WriteByte((byte)HttpUtility._hexChars[num]);
				num = chr >> '\u0004' & 15;
				result.WriteByte((byte)HttpUtility._hexChars[num]);
				result.WriteByte((byte)HttpUtility._hexChars[chr & '\u000F']);
				return;
			}
			if (c > ' ' && HttpUtility.notEncoded(c))
			{
				result.WriteByte((byte)c);
				return;
			}
			if (c == ' ')
			{
				result.WriteByte(43);
				return;
			}
			if (c >= '0' && (c >= 'A' || c <= '9') && (c <= 'Z' || c >= 'a') && c <= 'z')
			{
				result.WriteByte((byte)c);
				return;
			}
			if (!unicode || c <= '\u007F')
			{
				result.WriteByte(37);
			}
			else
			{
				result.WriteByte(37);
				result.WriteByte(117);
				result.WriteByte(48);
				result.WriteByte(48);
			}
			char chr1 = c;
			result.WriteByte((byte)HttpUtility._hexChars[chr1 >> '\u0004']);
			result.WriteByte((byte)HttpUtility._hexChars[chr1 & '\u000F']);
		}

		public static string UrlEncode(byte[] bytes)
		{
			if (bytes == null)
			{
				return null;
			}
			int length = (int)bytes.Length;
			int num = length;
			if (length == 0)
			{
				return string.Empty;
			}
			return Encoding.ASCII.GetString(HttpUtility.InternalUrlEncodeToBytes(bytes, 0, num));
		}

		public static string UrlEncode(string s)
		{
			return HttpUtility.UrlEncode(s, Encoding.UTF8);
		}

		public static string UrlEncode(string s, Encoding encoding)
		{
			if (s != null)
			{
				int length = s.Length;
				int num = length;
				if (length != 0)
				{
					bool flag = false;
					string str = s;
					int num1 = 0;
					while (num1 < str.Length)
					{
						char chr = str[num1];
						if ((chr < '0' || chr < 'A' && chr > '9' || chr > 'Z' && chr < 'a' || chr > 'z') && !HttpUtility.notEncoded(chr))
						{
							flag = true;
							break;
						}
						else
						{
							num1++;
						}
					}
					if (!flag)
					{
						return s;
					}
					if (encoding == null)
					{
						encoding = Encoding.UTF8;
					}
					byte[] numArray = new byte[encoding.GetMaxByteCount(num)];
					int bytes = encoding.GetBytes(s, 0, num, numArray, 0);
					return Encoding.ASCII.GetString(HttpUtility.InternalUrlEncodeToBytes(numArray, 0, bytes));
				}
			}
			return s;
		}

		public static string UrlEncode(byte[] bytes, int offset, int count)
		{
			byte[] numArray = HttpUtility.UrlEncodeToBytes(bytes, offset, count);
			if (numArray == null)
			{
				return null;
			}
			if (numArray.Length == 0)
			{
				return string.Empty;
			}
			return Encoding.ASCII.GetString(numArray);
		}

		public static byte[] UrlEncodeToBytes(byte[] bytes)
		{
			if (bytes != null)
			{
				int length = (int)bytes.Length;
				int num = length;
				if (length > 0)
				{
					return HttpUtility.InternalUrlEncodeToBytes(bytes, 0, num);
				}
			}
			return bytes;
		}

		public static byte[] UrlEncodeToBytes(string s)
		{
			return HttpUtility.UrlEncodeToBytes(s, Encoding.UTF8);
		}

		public static byte[] UrlEncodeToBytes(string s, Encoding encoding)
		{
			if (s == null)
			{
				return null;
			}
			if (s.Length == 0)
			{
				return new byte[0];
			}
			byte[] bytes = (encoding ?? Encoding.UTF8).GetBytes(s);
			return HttpUtility.InternalUrlEncodeToBytes(bytes, 0, (int)bytes.Length);
		}

		public static byte[] UrlEncodeToBytes(byte[] bytes, int offset, int count)
		{
			if (bytes != null)
			{
				int length = (int)bytes.Length;
				int num = length;
				if (length != 0)
				{
					if (count == 0)
					{
						return new byte[0];
					}
					if (offset < 0 || offset >= num)
					{
						throw new ArgumentOutOfRangeException("offset");
					}
					if (count < 0 || count > num - offset)
					{
						throw new ArgumentOutOfRangeException("count");
					}
					return HttpUtility.InternalUrlEncodeToBytes(bytes, offset, count);
				}
			}
			return bytes;
		}

		public static string UrlEncodeUnicode(string s)
		{
			if (s == null || s.Length <= 0)
			{
				return s;
			}
			return Encoding.ASCII.GetString(HttpUtility.InternalUrlEncodeUnicodeToBytes(s));
		}

		public static byte[] UrlEncodeUnicodeToBytes(string s)
		{
			if (s == null)
			{
				return null;
			}
			if (s.Length != 0)
			{
				return HttpUtility.InternalUrlEncodeUnicodeToBytes(s);
			}
			return new byte[0];
		}

		private static void urlPathEncode(char c, Stream result)
		{
			if (c >= '!' && c <= '~')
			{
				if (c != ' ')
				{
					result.WriteByte((byte)c);
					return;
				}
				result.WriteByte(37);
				result.WriteByte(50);
				result.WriteByte(48);
				return;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(c.ToString());
			for (int i = 0; i < (int)bytes.Length; i++)
			{
				byte num = bytes[i];
				result.WriteByte(37);
				result.WriteByte((byte)HttpUtility._hexChars[num >> 4]);
				result.WriteByte((byte)HttpUtility._hexChars[num & 15]);
			}
		}

		public static string UrlPathEncode(string s)
		{
			string str;
			if (s == null || s.Length == 0)
			{
				return s;
			}
			using (MemoryStream memoryStream = new MemoryStream())
			{
				str = s;
				for (int i = 0; i < str.Length; i++)
				{
					HttpUtility.urlPathEncode(str[i], memoryStream);
				}
				memoryStream.Close();
				str = Encoding.ASCII.GetString(memoryStream.ToArray());
			}
			return str;
		}

		private static void writeCharBytes(char c, IList buffer, Encoding encoding)
		{
			if (c <= 'ÿ')
			{
				buffer.Add((byte)c);
				return;
			}
			byte[] bytes = encoding.GetBytes(new char[] { c });
			for (int i = 0; i < (int)bytes.Length; i++)
			{
				buffer.Add(bytes[i]);
			}
		}
	}
}