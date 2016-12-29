using System.Collections.Generic;

namespace ChatAppLib.Models
{
    public static class UserManager
    {

        public static readonly Dictionary<Platform, Dictionary<string, User>> UserDB = new Dictionary<Platform, Dictionary<string, User>>();


        public static void AddPlatform(Platform platform)
        {
            if (!UserDB.ContainsKey(platform))
            {
                UserDB.Add(platform, new Dictionary<string, User>());
            }
        }

        public static void ClearaPlatform(Platform platform)
        {
            if (UserDB.ContainsKey(platform))
            {
                GetPlatform(platform).Clear();
            }
        }

        public static IDictionary<string, User> GetPlatform(Platform platform)
        {
            return UserDB[platform];
        }

        public static void AddUser(Platform platform, User user)
        {
            GetPlatform(platform).Add(user.Nickname, user);
        }
    }
}