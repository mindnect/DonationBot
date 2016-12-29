namespace ChatAppLib.Models
{
    public class Packet
    {
        public Packet(PacketType type, Message message)
        {
            Type = type;
            Message = message;
        }

        public Packet(PacketType type, User user = null, Message message = null, string amount = null)
        {
            Type = type;
            User = user;
            Message = message;
            Amount = amount;
        }

        public PacketType Type { get; set; }
        public User User { get; set; }
        public Message Message { get; set; }
        public string Amount { get; set; }

        public override string ToString()
        {
            return $"{Type,8}" + User + $"{Message,-24}" + $"{Amount,-12}";
        }
    }
}