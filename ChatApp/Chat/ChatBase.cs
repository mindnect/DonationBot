using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Data;
using mshtml;
using WebSocketSharp;

namespace ChatApp.Chat
{
    internal class ChatBase
    {
        private string _platform;
        protected HTMLDocument document;
        protected bool isActive;
        protected bool isReady;
        protected Dictionary<string, UserEntity> mapUser;

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
            mapUser = new Dictionary<string, UserEntity>();
            _platform = platform;
            isReady = false;
            isActive = true;
        }

        protected int GetElementUniqueNumber(IHTMLElement element)
        {
            return ((IHTMLUniqueName) element).uniqueNumber;
        }

        protected UserEntity GetUserData(string username, string rank = null)
        {
            if (mapUser.ContainsKey(username))
                return mapUser[username];

            var user = new UserEntity
            {
                Platform = _platform,
                Rank = rank,
                Nickname = username
            };
            mapUser.Add(username,user);
            OnUserAdded(user);
            return user;
        }

        protected virtual void OnUserAdded(UserEntity newUserEntity)
        {
            Console.WriteLine($"{"New",-8} {newUserEntity}");
        }

        protected virtual void PrepareUpdate()
        {
        }
    }
}