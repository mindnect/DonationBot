#region

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ChatAppLib.Models;
using mshtml;

#endregion

namespace ChatAppLib.Brokers
{
    public class Broker
    {
        private Platform _platform;
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

  

        protected void Init(Platform platform)
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

        protected User GetUserData(string nickname, string grade = null)
        {
            if (mapUser.ContainsKey(nickname))
                return mapUser[nickname];

            var user = new User(nickname, _platform, grade);

            mapUser.Add(nickname, user);
            OnUserAdded(user);
            return user;
        }

        protected virtual void OnUserAdded(User newUser)
        {
            //Console.Write("New");
        }

        protected virtual void PrepareUpdate()
        {
        }
    }
}