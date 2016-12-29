using MSHTML;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace JSAssist
{
	internal class ChatBase
	{
		protected IHTMLDocument2 document;

		protected List<ChatData> listNewChat;

		protected Dictionary<string, UserData> mapUser;

		protected bool isReady;

		protected bool isActive;

		protected ChatManager manager;

		public ChatBase()
		{
		}

		protected void AddChatData(ChatData chatData)
		{
			this.listNewChat.Add(chatData);
		}

		protected void AddChatDataFront(ChatData chatData)
		{
			this.listNewChat.Insert(0, chatData);
		}

		protected IHTMLElement FindClassFromChild(IHTMLElement parent, string className)
		{
			IHTMLElement variable;
			IEnumerator enumerator = ((IEnumerable)((dynamic)parent.children)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					IHTMLElement current = (IHTMLElement)((dynamic)enumerator.Current);
					if (current.className != className)
					{
						continue;
					}
					variable = current;
					return variable;
				}
				return null;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return variable;
		}

		protected int GetElementUniqueNumber(IHTMLElement element)
		{
			return ((IHTMLUniqueName)element).uniqueNumber;
		}

		public List<ChatData> GetNewChatList()
		{
			return this.listNewChat;
		}

		protected UserData GetUserData(string username)
		{
			if (this.mapUser.ContainsKey(username))
			{
				return this.mapUser[username];
			}
			UserData userDatum = new UserData()
			{
				username = username
			};
			this.mapUser.Add(username, userDatum);
			this.OnUserAdded(userDatum);
			return userDatum;
		}

		public virtual void Initialize(ChatManager manager)
		{
			this.listNewChat = new List<ChatData>();
			this.mapUser = new Dictionary<string, UserData>();
			this.isReady = false;
			this.isActive = true;
			this.manager = manager;
		}

		protected virtual void OnUserAdded(UserData newUser)
		{
		}

		protected virtual void PrepareUpdate()
		{
		}

		public virtual void Reset()
		{
			this.isReady = false;
			if (this.document != null)
			{
				Marshal.ReleaseComObject(this.document);
			}
			this.document = null;
			this.listNewChat.Clear();
			this.mapUser.Clear();
		}

		public void SetActive(bool active)
		{
			this.isActive = active;
			if (active)
			{
				this.Reset();
			}
		}

		public virtual bool Update()
		{
			if (!this.isActive)
			{
				return false;
			}
			if (this.isReady)
			{
				return true;
			}
			this.PrepareUpdate();
			return false;
		}
	}
}