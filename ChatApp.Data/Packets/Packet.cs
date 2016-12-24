using System;
using System.Xml.Serialization;

namespace Comm.Packets
{
    [Serializable]
    public enum MessageType
    {
        Notice,
        Chat,
        Whisper,
        Donation,
        Order
    }
    [Serializable]
    public class User
    {
        public string nickname;
        public string platform;
        public string rank;

        public override string ToString()
        {
            return $"{platform, 8} {rank,4} {nickname, 12}";
        }
    }
    [Serializable]
    [XmlInclude(typeof(Notice))]
    [XmlInclude(typeof(Chat))]
    [XmlInclude(typeof(Donation))]
    [XmlInclude(typeof(Order))]
    public class Packet
    {
        public MessageType messageType;
        public string message;
    }
    [Serializable]
    public class Notice : Packet
    {
        public User user;

        public Notice()
        {
            messageType = MessageType.Notice;
        }
    }
    [Serializable]
    [XmlInclude(typeof(Donation))]
    public class Chat : Packet
    {
        public User user;

        public Chat()
        {
            messageType = MessageType.Chat;
        }

        public override string ToString()
        {
            return $"{user, 24} {message, 24}";
        }
    }

    //@200 ¸Þ¼¼Áö
    public class Order : Packet
    {
        public User user;
        public string command;
        public string parameter;

        public Order()
        {
            messageType = MessageType.Order;
        }

        public override string ToString()
        {
            return $"{user, 24} {command, 8} {message, 24}";
        }

    }

    [Serializable]
    public class Donation : Chat
    {
        public string amount;

        public Donation()
        {
            messageType = MessageType.Donation;
        }

        public override string ToString()
        {
            return base.ToString() + $"{amount, 8}";
        }
    }
}