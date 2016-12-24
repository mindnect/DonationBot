using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ChatAppLib.Data;
using mshtml;

namespace ChatApp.Parser
{
    internal class BaseParser
    {
        private string _platform;
        protected HTMLDocument document;
        protected bool isActive;
        protected bool isReady;
        protected Dictionary<string, User> mapUser;

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
            mapUser = new Dictionary<string, User>();
            _platform = platform;
            isReady = false;
            isActive = true;
        }

        protected int GetElementUniqueNumber(IHTMLElement element)
        {
            return ((IHTMLUniqueName) element).uniqueNumber;
        }

        protected User GetUserData(string username, string rank = null)
        {
            if (mapUser.ContainsKey(username))
                return mapUser[username];

            var user = new User
            {
                platform = _platform,
                rank = rank,
                nickname = username
            };
            mapUser.Add(username, user);
            OnUserAdded(user);
            return user;
        }

        protected virtual void OnUserAdded(User newUser)
        {
            Console.WriteLine("New User");
        }

        protected virtual void PrepareUpdate()
        {
        }
    }
}