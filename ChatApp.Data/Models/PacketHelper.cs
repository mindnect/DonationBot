namespace ChatAppLib.Models
{
    public static class PacketHelper
    {
        public static Packet CreateLog(string message)
        {
            return new Packet(PacketType.LOG, new Message(message));
        }

        public static Packet CreateEnter(User user)
        {
            return new Packet(PacketType.ENTER, user);
        }

        public static Packet CreateExit(User user)
        {
            return new Packet(PacketType.EXIT, user);
        }

        public static Packet CreateNotice(User user, string message)
        {
            return new Packet(PacketType.NOTICE, user, new Message(message));
        }

        public static Packet CreateWhisper(User user, string message)
        {
            return new Packet(PacketType.WHISPER, user, new Message(message));
        }

        public static Packet CreateChat(User user, string message)
        {
            return new Packet(PacketType.CHAT, user, new Message(message));
        }

        public static Packet CreateSpon(User user, string message, string amount)
        {
            return new Packet(PacketType.SPON, user, new Message(message), amount);
        }
    }
}