using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using LiteDB;

namespace Database
{
    public class UserData
    {
        public int Id { get; set; }

        public string Platform { get; set; }
        public string Rank { get; set; }

        [BsonIndex(true)]
        public string NickName { get; set; }

        public override string ToString()
        {
            return $"{Platform,-8} {Rank,4} {NickName,-12}";
        }
    }

    public class ChatData
    {
        public int Id { get; set; }

        [BsonRef("Users")]
        public UserData UserData { get; set; }

        public string Message { get; set; }

        public override string ToString()
        {
            return $"{UserData,-24} {Message}";
        }
    }

    public class DonationData : ChatData
    {
        public int Amount { get; set; }

        public override string ToString()
        {
            return base.ToString() + $"{Amount,-8}";
        }
    }

    public class ChatDB
    {
        public const string FilePath = @"c:\ChatDB\";
        public const string FileName = @"database.db";
        public const string FullPath = FilePath + FileName;
        public static LiteDatabase DB { get; private set; }

        static ChatDB()
        {
            try
            {
                Directory.CreateDirectory(FilePath);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            var watcher = new FileSystemWatcher
            {
                Path = FilePath,
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = FileName,
                EnableRaisingEvents = true
            };
            
            DB = new LiteDatabase(FullPath);
            //var t = "filename=" + FullPath + ";Autocommit=false";

        }

        public static FileSystemWatcher Watcher { get; }

        public static LiteCollection<UserData> Users => DB.GetCollection<UserData>("Users");
        public static LiteCollection<ChatData> Chats => DB.GetCollection<ChatData>("Chats").Include(x => x.UserData);

        public static void Reset()
        {
            
            if (File.Exists(FullPath))
            {
                DB.DropCollection("Users");
                DB.DropCollection("Chats");
                DB.DropCollection("Donations");
            }
        }
    }
}