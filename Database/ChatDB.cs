﻿using System;
using System.IO;
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

    public static class ChatDB
    {
        public const string DbFilePath = @"c:\DonationBot\";
        public const string DbFileName = @"Donations.DB";

        static ChatDB()
        {
            // Watch 초기화
            var watcher = new FileSystemWatcher
            {
                Path = DbFilePath,
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = DbFileName,
                EnableRaisingEvents = true
            };
        }

        public static readonly LiteDatabase DB = new LiteDatabase(DbFilePath+DbFileName);
        public static FileSystemWatcher Watcher { get; }

        public static LiteCollection<UserData> Users { get; } = DB.GetCollection<UserData>("Users");
        public static LiteCollection<ChatData> Chats { get; } = DB.GetCollection<ChatData>("Chats").Include(x => x.UserData);
        public static LiteCollection<DonationData> Donations { get; } = DB.GetCollection<DonationData>("Donations").Include(x => x.UserData);
    }
}