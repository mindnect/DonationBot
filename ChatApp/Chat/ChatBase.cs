using System;
using System.Runtime.InteropServices;
using Database;
using LiteDB;
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

        protected void Init(string platform)
        {
            _platform = platform;
            isReady = false;
            isActive = true;
        }

        protected void AddChatData(ChatData chatData)
        {
            Console.WriteLine(chatData);
            ChatDB.Chats.Insert(chatData);
        }

        protected int GetElementUniqueNumber(IHTMLElement element)
        {
            return ((IHTMLUniqueName) element).uniqueNumber;
        }

        protected UserData GetUserData(string username, string rank = null)
        {
            var ret = ChatDB.Users.FindOne(x => (x.NickName == username) && (x.Platform == _platform));
            if (ret != null)
            {
                if (rank!=null) ret.Rank = rank;
                ChatDB.Users.Update(ret);
                return ret;
            }

            var user = new UserData
            {
                Platform = _platform,
                Rank = rank,
                NickName = username
            };

            OnUserAdded(user);
            ChatDB.Users.Insert(user);
            return user;
        }

        protected virtual void OnUserAdded(UserData newUserData)
        {
            Console.WriteLine($"{"New",-8} {newUserData}");
        }

        protected virtual void PrepareUpdate()
        {
        }
    }
}