namespace ChatApp.Client.Models
{
    public class Packet
    {
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