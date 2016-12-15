using System;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using WebSocket;

namespace WebSocket.Net
{
	internal class AuthenticationResponse : AuthenticationBase
	{
		private uint _nonceCount;

		public string Cnonce
		{
			get
			{
				return this.Parameters["cnonce"];
			}
		}

		public string Nc
		{
			get
			{
				return this.Parameters["nc"];
			}
		}

		internal uint NonceCount
		{
			get
			{
				if (this._nonceCount >= -1)
				{
					return (uint)0;
				}
				return this._nonceCount;
			}
		}

		public string Password
		{
			get
			{
				return this.Parameters["password"];
			}
		}

		public string Response
		{
			get
			{
				return this.Parameters["response"];
			}
		}

		public string Uri
		{
			get
			{
				return this.Parameters["uri"];
			}
		}

		public string UserName
		{
			get
			{
				return this.Parameters["username"];
			}
		}

		private AuthenticationResponse(AuthenticationSchemes scheme, NameValueCollection parameters) : base(scheme, parameters)
		{
		}

		internal AuthenticationResponse(NetworkCredential credentials) : this(AuthenticationSchemes.Basic, new NameValueCollection(), credentials, 0)
		{
		}

		internal AuthenticationResponse(AuthenticationChallenge challenge, NetworkCredential credentials, uint nonceCount) : this(challenge.Scheme, challenge.Parameters, credentials, nonceCount)
		{
		}

		internal AuthenticationResponse(AuthenticationSchemes scheme, NameValueCollection parameters, NetworkCredential credentials, uint nonceCount) : base(scheme, parameters)
		{
			this.Parameters["username"] = credentials.UserName;
			this.Parameters["password"] = credentials.Password;
			this.Parameters["uri"] = credentials.Domain;
			this._nonceCount = nonceCount;
			if (scheme == AuthenticationSchemes.Digest)
			{
				this.initAsDigest();
			}
		}

		private static string createA1(string username, string password, string realm)
		{
			return string.Format("{0}:{1}:{2}", username, realm, password);
		}

		private static string createA1(string username, string password, string realm, string nonce, string cnonce)
		{
			return string.Format("{0}:{1}:{2}", AuthenticationResponse.hash(AuthenticationResponse.createA1(username, password, realm)), nonce, cnonce);
		}

		private static string createA2(string method, string uri)
		{
			return string.Format("{0}:{1}", method, uri);
		}

		private static string createA2(string method, string uri, string entity)
		{
			return string.Format("{0}:{1}:{2}", method, uri, AuthenticationResponse.hash(entity));
		}

		internal static string CreateRequestDigest(NameValueCollection parameters)
		{
			string str;
			string item = parameters["username"];
			string item1 = parameters["password"];
			string str1 = parameters["realm"];
			string item2 = parameters["nonce"];
			string str2 = parameters["uri"];
			string item3 = parameters["algorithm"];
			string str3 = parameters["qop"];
			string item4 = parameters["cnonce"];
			string str4 = parameters["nc"];
			string item5 = parameters["method"];
			str = (item3 == null || !(item3.ToLower() == "md5-sess") ? AuthenticationResponse.createA1(item, item1, str1) : AuthenticationResponse.createA1(item, item1, str1, item2, item4));
			string str5 = (str3 == null || !(str3.ToLower() == "auth-int") ? AuthenticationResponse.createA2(item5, str2) : AuthenticationResponse.createA2(item5, str2, parameters["entity"]));
			string str6 = AuthenticationResponse.hash(str);
			return AuthenticationResponse.hash(string.Format("{0}:{1}", str6, (str3 != null ? string.Format("{0}:{1}:{2}:{3}:{4}", new object[] { item2, str4, item4, str3, AuthenticationResponse.hash(str5) }) : string.Format("{0}:{1}", item2, AuthenticationResponse.hash(str5)))));
		}

		private static string hash(string value)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(value);
			byte[] numArray = MD5.Create().ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder(64);
			byte[] numArray1 = numArray;
			for (int i = 0; i < (int)numArray1.Length; i++)
			{
				byte num = numArray1[i];
				stringBuilder.Append(num.ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		private void initAsDigest()
		{
			string item = this.Parameters["qop"];
			if (item != null)
			{
				if (!item.Split(new char[] { ',' }).Contains<string>((string qop) => qop.Trim().ToLower() == "auth"))
				{
					this.Parameters["qop"] = null;
				}
				else
				{
					this.Parameters["qop"] = "auth";
					this.Parameters["cnonce"] = AuthenticationBase.CreateNonceValue();
					uint num = this._nonceCount + 1;
					this._nonceCount = num;
					this.Parameters["nc"] = string.Format("{0:x8}", num);
				}
			}
			this.Parameters["method"] = "GET";
			this.Parameters["response"] = AuthenticationResponse.CreateRequestDigest(this.Parameters);
		}

		internal static AuthenticationResponse Parse(string value)
		{
			AuthenticationResponse authenticationResponse;
			AuthenticationResponse authenticationResponse1;
			try
			{
				string[] strArrays = value.Split(new char[] { ' ' }, 2);
				if ((int)strArrays.Length == 2)
				{
					string lower = strArrays[0].ToLower();
					if (lower == "basic")
					{
						authenticationResponse1 = new AuthenticationResponse(AuthenticationSchemes.Basic, AuthenticationResponse.ParseBasicCredentials(strArrays[1]));
					}
					else if (lower == "digest")
					{
						authenticationResponse1 = new AuthenticationResponse(AuthenticationSchemes.Digest, AuthenticationBase.ParseParameters(strArrays[1]));
					}
					else
					{
						authenticationResponse1 = null;
					}
					authenticationResponse = authenticationResponse1;
				}
				else
				{
					authenticationResponse = null;
				}
			}
			catch
			{
				return null;
			}
			return authenticationResponse;
		}

		internal static NameValueCollection ParseBasicCredentials(string value)
		{
			string str = Encoding.Default.GetString(Convert.FromBase64String(value));
			int num = str.IndexOf(':');
			string str1 = str.Substring(0, num);
			string str2 = (num < str.Length - 1 ? str.Substring(num + 1) : string.Empty);
			num = str1.IndexOf('\\');
			if (num > -1)
			{
				str1 = str1.Substring(num + 1);
			}
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["username"] = str1;
			nameValueCollection["password"] = str2;
			return nameValueCollection;
		}

		internal override string ToBasicString()
		{
			string str = string.Format("{0}:{1}", this.Parameters["username"], this.Parameters["password"]);
			string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
			return string.Concat("Basic ", base64String);
		}

		internal override string ToDigestString()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			stringBuilder.AppendFormat("Digest username=\"{0}\", realm=\"{1}\", nonce=\"{2}\", uri=\"{3}\", response=\"{4}\"", new object[] { this.Parameters["username"], this.Parameters["realm"], this.Parameters["nonce"], this.Parameters["uri"], this.Parameters["response"] });
			string item = this.Parameters["opaque"];
			if (item != null)
			{
				stringBuilder.AppendFormat(", opaque=\"{0}\"", item);
			}
			string str = this.Parameters["algorithm"];
			if (str != null)
			{
				stringBuilder.AppendFormat(", algorithm={0}", str);
			}
			string item1 = this.Parameters["qop"];
			if (item1 != null)
			{
				stringBuilder.AppendFormat(", qop={0}, cnonce=\"{1}\", nc={2}", item1, this.Parameters["cnonce"], this.Parameters["nc"]);
			}
			return stringBuilder.ToString();
		}

		public IIdentity ToIdentity()
		{
			IIdentity httpDigestIdentity;
			AuthenticationSchemes scheme = base.Scheme;
			if (scheme != AuthenticationSchemes.Basic)
			{
				if (scheme == AuthenticationSchemes.Digest)
				{
					httpDigestIdentity = new HttpDigestIdentity(this.Parameters);
				}
				else
				{
					httpDigestIdentity = null;
				}
				return httpDigestIdentity;
			}
			IIdentity httpBasicIdentity = new HttpBasicIdentity(this.Parameters["username"], this.Parameters["password"]);
			return httpBasicIdentity;
		}
	}
}