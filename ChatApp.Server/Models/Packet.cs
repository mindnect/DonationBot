namespace ChatApp.Server.Models
{
    public class Packet
    {
        public Packet(PacketType type, User user = null, Message message = null, string amount = null)
        {
            this.type = type;
            this.user = user;
            this.message = message;
            this.amount = amount;
        }

        public PacketType type { get; set; }
        public User user { get; set; }
        public Message message { get; set; }
        public string amount { get; set; }

        public override string ToString()
        {
            return $"{type,8}" + user + $"{message,-24}" + $"{amount,-12}";
        }
    }
}