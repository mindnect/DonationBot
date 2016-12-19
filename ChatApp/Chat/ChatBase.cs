using System;
using System.Collections;
using System.Runtime.InteropServices;
using Database;
using mshtml;

namespace ChatApp.Chat
{
    internal class ChatBase
    {
        //protected List<ChatData> listNewChat;

        //protected Dictionary<string, UserData> mapUser;
        private string _platform;
        protected HTMLDocument document;
        protected bool isActive;
        protected bool isReady;

        protected void AddChatData(ChatData chatData)
        {
            Console.WriteLine(chatData);
            ChatDB.Chats.Insert(chatData);
        }

        protected IHTMLElement FindClassFromChild(IHTMLElement parent, string className)
        {
            var enumerator = ((IEnumerable) parent.children).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    var current = (IHTMLElement) enumerator.Current;
                    if (current.className != className)
                        continue;
                    var variable = current;
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
        }

        protected int GetElementUniqueNumber(IHTMLElement element)
        {
            return ((IHTMLUniqueName) element).uniqueNumber;;
        }

        protected UserData GetUserData(string username)
        {
            var t = ChatDB.Users.FindOne(x => (x.NickName == username) && (x.Platform == _platform));
            if (t != null)
                return t;

            var user = new UserData
            {
                Platform = _platform,
                NickName = username
            };

            OnUserAdded(user);

            ChatDB.Users.Insert(user);
            return user;
        }

        public virtual void Initialize(string platform)
        {
            //listNewChat = new List<ChatData>();
            //mapUser = new Dictionary<string, UserData>();
            _platform = platform;
            isReady = false;
            isActive = true;
        }

        protected virtual void OnUserAdded(UserData newUserData)
        {
            Console.WriteLine($"{"New",-8} {newUserData}");
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