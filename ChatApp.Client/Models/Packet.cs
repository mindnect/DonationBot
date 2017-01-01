namespace ChatAppLib.Models
{
    public class Packet
    {
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