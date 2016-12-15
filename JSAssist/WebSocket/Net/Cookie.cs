using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using WebSocket;

namespace WebSocket.Net
{
	[Serializable]
	public sealed class Cookie
	{
		private string _comment;

		private Uri _commentUri;

		private bool _discard;

		private string _domain;

		private DateTime _expires;

		private bool _httpOnly;

		private string _name;

		private string _path;

		private string _port;

		private int[] _ports;

		private readonly static char[] _reservedCharsForName;

		private readonly static char[] _reservedCharsForValue;

		private bool _secure;

		private DateTime _timestamp;

		private string _value;

		private int _version;

		public string Comment
		{
			get
			{
				return this._comment;
			}
			set
			{
				this._comment = value ?? string.Empty;
			}
		}

		public Uri CommentUri
		{
			get
			{
				return this._commentUri;
			}
			set
			{
				this._commentUri = value;
			}
		}

		public bool Discard
		{
			get
			{
				return this._discard;
			}
			set
			{
				this._discard = value;
			}
		}

		public string Domain
		{
			get
			{
				return this._domain;
			}
			set
			{
				if (value.IsNullOrEmpty())
				{
					this._domain = string.Empty;
					this.ExactDomain = true;
					return;
				}
				this._domain = value;
				this.ExactDomain = value[0] != '.';
			}
		}

		internal bool ExactDomain
		{
			get;
			set;
		}

		public bool Expired
		{
			get
			{
				if (this._expires == DateTime.MinValue)
				{
					return false;
				}
				return this._expires <= DateTime.Now;
			}
			set
			{
				this._expires = (value ? DateTime.Now : DateTime.MinValue);
			}
		}

		public DateTime Expires
		{
			get
			{
				return this._expires;
			}
			set
			{
				this._expires = value;
			}
		}

		public bool HttpOnly
		{
			get
			{
				return this._httpOnly;
			}
			set
			{
				this._httpOnly = value;
			}
		}

		internal int MaxAge
		{
			get
			{
				if (this._expires == DateTime.MinValue)
				{
					return 0;
				}
				TimeSpan timeSpan = (this._expires.Kind != DateTimeKind.Local ? this._expires.ToLocalTime() : this._expires) - DateTime.Now;
				if (timeSpan <= TimeSpan.Zero)
				{
					return 0;
				}
				return (int)timeSpan.TotalSeconds;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				string str;
				if (!Cookie.canSetName(value, out str))
				{
					throw new CookieException(str);
				}
				this._name = value;
			}
		}

		public string Path
		{
			get
			{
				return this._path;
			}
			set
			{
				this._path = value ?? string.Empty;
			}
		}

		public string Port
		{
			get
			{
				return this._port;
			}
			set
			{
				string str;
				if (value.IsNullOrEmpty())
				{
					this._port = string.Empty;
					this._ports = new int[0];
					return;
				}
				if (!value.IsEnclosedIn('\"'))
				{
					throw new CookieException("The value specified for the Port attribute isn't enclosed in double quotes.");
				}
				if (!Cookie.tryCreatePorts(value, out this._ports, out str))
				{
					throw new CookieException(string.Format("The value specified for the Port attribute contains an invalid value: {0}", str));
				}
				this._port = value;
			}
		}

		internal int[] Ports
		{
			get
			{
				return this._ports;
			}
		}

		public bool Secure
		{
			get
			{
				return this._secure;
			}
			set
			{
				this._secure = value;
			}
		}

		public DateTime TimeStamp
		{
			get
			{
				return this._timestamp;
			}
		}

		public string Value
		{
			get
			{
				return this._value;
			}
			set
			{
				string str;
				if (!Cookie.canSetValue(value, out str))
				{
					throw new CookieException(str);
				}
				this._value = (value.Length > 0 ? value : "\"\"");
			}
		}

		public int Version
		{
			get
			{
				return this._version;
			}
			set
			{
				if (value < 0 || value > 1)
				{
					throw new ArgumentOutOfRangeException("value", "Not 0 or 1.");
				}
				this._version = value;
			}
		}

		static Cookie()
		{
			Cookie._reservedCharsForName = new char[] { ' ', '=', ';', ',', '\n', '\r', '\t' };
			Cookie._reservedCharsForValue = new char[] { ';', ',' };
		}

		public Cookie()
		{
			this._comment = string.Empty;
			this._domain = string.Empty;
			this._expires = DateTime.MinValue;
			this._name = string.Empty;
			this._path = string.Empty;
			this._port = string.Empty;
			this._ports = new int[0];
			this._timestamp = DateTime.Now;
			this._value = string.Empty;
			this._version = 0;
		}

		public Cookie(string name, string value) : this()
		{
			this.Name = name;
			this.Value = value;
		}

		public Cookie(string name, string value, string path) : this(name, value)
		{
			this.Path = path;
		}

		public Cookie(string name, string value, string path, string domain) : this(name, value, path)
		{
			this.Domain = domain;
		}

		private static bool canSetName(string name, out string message)
		{
			if (name.IsNullOrEmpty())
			{
				message = "The value specified for the Name is null or empty.";
				return false;
			}
			if (name[0] != '$' && !name.Contains(Cookie._reservedCharsForName))
			{
				message = string.Empty;
				return true;
			}
			message = "The value specified for the Name contains an invalid character.";
			return false;
		}

		private static bool canSetValue(string value, out string message)
		{
			if (value == null)
			{
				message = "The value specified for the Value is null.";
				return false;
			}
			if (value.Contains(Cookie._reservedCharsForValue) && !value.IsEnclosedIn('\"'))
			{
				message = "The value specified for the Value contains an invalid character.";
				return false;
			}
			message = string.Empty;
			return true;
		}

		public override bool Equals(object comparand)
		{
			Cookie cookie = comparand as Cookie;
			if (cookie == null || !this._name.Equals(cookie.Name, StringComparison.InvariantCultureIgnoreCase) || !this._value.Equals(cookie.Value, StringComparison.InvariantCulture) || !this._path.Equals(cookie.Path, StringComparison.InvariantCulture) || !this._domain.Equals(cookie.Domain, StringComparison.InvariantCultureIgnoreCase))
			{
				return false;
			}
			return this._version == cookie.Version;
		}

		public override int GetHashCode()
		{
			return Cookie.hash(StringComparer.InvariantCultureIgnoreCase.GetHashCode(this._name), this._value.GetHashCode(), this._path.GetHashCode(), StringComparer.InvariantCultureIgnoreCase.GetHashCode(this._domain), this._version);
		}

		private static int hash(int i, int j, int k, int l, int m)
		{
			return i ^ (j << 13 | j >> 19) ^ (k << 26 | k >> 6) ^ (l << 7 | l >> 25) ^ (m << 20 | m >> 12);
		}

		internal string ToRequestString(Uri uri)
		{
			if (this._name.Length == 0)
			{
				return string.Empty;
			}
			if (this._version == 0)
			{
				return string.Format("{0}={1}", this._name, this._value);
			}
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("$Version={0}; {1}={2}", this._version, this._name, this._value);
			if (!this._path.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; $Path={0}", this._path);
			}
			else if (uri == null)
			{
				stringBuilder.Append("; $Path=/");
			}
			else
			{
				stringBuilder.AppendFormat("; $Path={0}", uri.GetAbsolutePath());
			}
			if ((uri == null ? true : uri.Host != this._domain) && !this._domain.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; $Domain={0}", this._domain);
			}
			if (!this._port.IsNullOrEmpty())
			{
				if (this._port != "\"\"")
				{
					stringBuilder.AppendFormat("; $Port={0}", this._port);
				}
				else
				{
					stringBuilder.Append("; $Port");
				}
			}
			return stringBuilder.ToString();
		}

		internal string ToResponseString()
		{
			if (this._name.Length <= 0)
			{
				return string.Empty;
			}
			if (this._version != 0)
			{
				return this.toResponseStringVersion1();
			}
			return this.toResponseStringVersion0();
		}

		private string toResponseStringVersion0()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("{0}={1}", this._name, this._value);
			if (this._expires != DateTime.MinValue)
			{
				DateTime universalTime = this._expires.ToUniversalTime();
				stringBuilder.AppendFormat("; Expires={0}", universalTime.ToString("ddd, dd'-'MMM'-'yyyy HH':'mm':'ss 'GMT'", CultureInfo.CreateSpecificCulture("en-US")));
			}
			if (!this._path.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; Path={0}", this._path);
			}
			if (!this._domain.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; Domain={0}", this._domain);
			}
			if (this._secure)
			{
				stringBuilder.Append("; Secure");
			}
			if (this._httpOnly)
			{
				stringBuilder.Append("; HttpOnly");
			}
			return stringBuilder.ToString();
		}

		private string toResponseStringVersion1()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("{0}={1}; Version={2}", this._name, this._value, this._version);
			if (this._expires != DateTime.MinValue)
			{
				stringBuilder.AppendFormat("; Max-Age={0}", this.MaxAge);
			}
			if (!this._path.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; Path={0}", this._path);
			}
			if (!this._domain.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; Domain={0}", this._domain);
			}
			if (!this._port.IsNullOrEmpty())
			{
				if (this._port != "\"\"")
				{
					stringBuilder.AppendFormat("; Port={0}", this._port);
				}
				else
				{
					stringBuilder.Append("; Port");
				}
			}
			if (!this._comment.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; Comment={0}", this._comment.UrlEncode());
			}
			if (this._commentUri != null)
			{
				string originalString = this._commentUri.OriginalString;
				stringBuilder.AppendFormat("; CommentURL={0}", (originalString.IsToken() ? originalString : originalString.Quote()));
			}
			if (this._discard)
			{
				stringBuilder.Append("; Discard");
			}
			if (this._secure)
			{
				stringBuilder.Append("; Secure");
			}
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			return this.ToRequestString(null);
		}

		private static bool tryCreatePorts(string value, out int[] result, out string parseError)
		{
			string[] strArrays = value.Trim(new char[] { '\"' }).Split(new char[] { ',' });
			int length = (int)strArrays.Length;
			int[] numArray = new int[length];
			for (int i = 0; i < length; i++)
			{
				numArray[i] = -2147483648;
				string str = strArrays[i].Trim();
				if (str.Length != 0 && !int.TryParse(str, out numArray[i]))
				{
					result = new int[0];
					parseError = str;
					return false;
				}
			}
			result = numArray;
			parseError = string.Empty;
			return true;
		}
	}
}