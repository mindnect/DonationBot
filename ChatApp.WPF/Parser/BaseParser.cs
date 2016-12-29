#region

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ChatAppLib.Models;
using mshtml;

#endregion

namespace ChatApp.Parser
{
    public class BaseParser
    {
        private string _platform;
        protected HTMLDocument document;
        protected bool isActive;
        protected bool isReady;
        protected Dictionary<string, UserModel> mapUser;

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

        public string ParseCommand(ref string message)
        {
            var param = "";
            var commands = message.Split(' ');
            var idx = 0;
            foreach (var c in commands)
            {
                if (c.StartsWith("@"))
                {
                    idx += c.Length;
                    param += c + " ";
                }
                else
                {
                    break;
                }
            }

            message = message.Remove(0, param.Length);
            return param;
        }

        protected void Init(string platform)
        {
            mapUser = new Dictionary<string, UserModel>();
            _platform = platform;
            isReady = false;
            isActive = true;
        }

        protected int GetElementUniqueNumber(IHTMLElement element)
        {
            return ((IHTMLUniqueName) element).uniqueNumber;
        }

        protected UserModel GetUserData(string username, string rank = null)
        {
            if (mapUser.ContainsKey(username))
                return mapUser[username];

            var user = new UserModel
            {
                Platform = _platform,
                Rank = rank,
                Nickname = username
            };
            mapUser.Add(username, user);
            OnUserAdded(user);
            return user;
        }

        protected virtual void OnUserAdded(UserModel newUserModel)
        {
            Console.WriteLine("New User");
        }

        protected virtual void PrepareUpdate()
        {
        }
    }
}