using ChatApp.Server.Models;

namespace ChatApp.Server
{
    public static class PacketHelper
    {
        public static Packet CreateLog(string message)
        {
            return new Packet(PacketType.log, null, new Message(message));
        }

        public static Packet CreateEnter(User user)
        {
            return new Packet(PacketType.enter, user);
        }

        public static Packet CreateExit(User user)
        {
            return new Packet(PacketType.exit, user);
        }

        public static Packet CreateNotice(User user, string message)
        {
            return new Packet(PacketType.notice, user, new Message(message));
        }

        public static Packet CreateWhisper(User user, string message)
        {
            return new Packet(PacketType.whisper, user, new Message(message));
        }

        public static Packet CreateChat(User user, string message)
        {
            return new Packet(PacketType.chat, user, new Message(message));
        }

        public static Packet CreateSpon(User user, string message, string amount)
        {
            return new Packet(PacketType.spon, user, new Message(message), amount);
        }
    }
}