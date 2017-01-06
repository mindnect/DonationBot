namespace ChatApp.Server.Models
{
    public class Packet
    {
        private Packet(PacketType type, User user = null, Message message = null, string amount = null)
        {
            this.type = type;
            this.user = user;
            this.message = message;
            this.amount = amount;
        }

        private Packet() { }

        public PacketType type { get; set; }
        public User user { get; set; }
        public Message message { get; set; }
        public string amount { get; set; }

        public override string ToString()
        {
            return $"{type,8}" + user + $"{message,-24}" + $"{amount,-12}";
        }

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