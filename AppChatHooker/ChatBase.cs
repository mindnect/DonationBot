using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using mshtml;

namespace AlcoholV
{
    internal class ChatBase
    {
        protected IHTMLDocument2 document;

        protected List<ChatData> listNewChat;

        protected Dictionary<string, UserData> mapUser;

        protected bool isReady;

        protected bool isActive;

        protected void AddChatData(ChatData chatData)
        {
            listNewChat.Add(chatData);
        }

        protected void AddChatDataFront(ChatData chatData)
        {
            listNewChat.Insert(0, chatData);
        }

        protected IHTMLElement FindClassFromChild(IHTMLElement parent, string className)
        {
            IHTMLElement variable;
            var enumerator = ((IEnumerable) parent.children).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    var current = (IHTMLElement) enumerator.Current;
                    if (current.className != className)
                        continue;
                    variable = current;
                    return variable;
                }
                return null;
            }
            finally
            {
                var disposable = enumerator as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
            return variable;
        }

        protected int GetElementUniqueNumber(IHTMLElement element)
        {
            return ((IHTMLUniqueName) element).uniqueNumber;
        }

        public List<ChatData> GetNewChatList()
        {
            return listNewChat;
        }

        protected UserData GetUserData(string username)
        {
            if (mapUser.ContainsKey(username))
                return mapUser[username];
            var userDatum = new UserData
            {
                username = username
            };
            mapUser.Add(username, userDatum);
            OnUserAdded(userDatum);
            return userDatum;
        }

        public virtual void Initialize()
        {
            listNewChat = new List<ChatData>();
            mapUser = new Dictionary<string, UserData>();
            isReady = false;
            isActive = true;
        }

        protected virtual void OnUserAdded(UserData newUser)
        {
        }

        protected virtual void PrepareUpdate()
        {
        }

        public virtual void Reset()
        {
            isReady = false;
            if (document != null)
                Marshal.ReleaseComObject(document);
            document = null;
            listNewChat.Clear();
            mapUser.Clear();
        }

        public void SetActive(bool active)
        {
            isActive = active;
            if (active)
                Reset();
        }

        public virtual bool Update()
        {
            if (!isActive)
                return false;
            if (isReady)
                return true;
            PrepareUpdate();
            return false;
        }
    }
}