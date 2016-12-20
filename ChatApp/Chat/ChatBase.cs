using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Database;
using mshtml;

namespace ChatApp.Chat
{
    internal class ChatBase
    {
        private string _platform;
        protected HTMLDocument document;
        protected bool isActive;
        protected bool isReady;
        //protected List<ChatData> listNewChat;
        protected Dictionary<string, UserData> mapUser;

        public virtual void Reset()
        {
            isReady = false;
            mapUser.Clear();
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
            mapUser = new Dictionary<string, UserData>();
            _platform = platform;
            isReady = false;
            isActive = true;
        }

        protected void AddChatData(ChatData chatData)
        {
            Console.WriteLine(chatData.ToString());
        }

        protected int GetElementUniqueNumber(IHTMLElement element)
        {
            return ((IHTMLUniqueName) element).uniqueNumber;
        }

        protected UserData GetUserData(string username, string rank = null)
        {
            if (mapUser.ContainsKey(username))
                return mapUser[username];
            var user = new UserData
            {
                Platform = _platform,
                Rank = rank,
                NickName = username
            };

            OnUserAdded(user);
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