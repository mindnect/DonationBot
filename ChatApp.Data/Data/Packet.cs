using System.Xml.Serialization;

namespace ChatAppLib.Data
{
    public enum PacketType
    {
        None,
        Notice,
        Chat,
        Whisper,
        Donation,
        Command
    }

    public class User
    {
        public string nickname;
        public string platform;
        public string rank;

        public override string ToString()
        {
            return $"{platform,8} {rank,8} {nickname,12}";
        }
    }

    [XmlInclude(typeof(MessagePacket))]
    public class Packet
    {
        public string message;
        public PacketType packetType;

        public Packet(PacketType packetType, string message)
        {
            this.packetType = packetType;
            this.message = message;
        }

        protected Packet()
        {
            
        }
    }

    [XmlInclude(typeof(DonationPacket))]
    [XmlInclude(typeof(CommandPacket))]
    public class MessagePacket : Packet
    {
        public User user;

        public MessagePacket(PacketType packetType, User user, string message) : base(packetType, message)
        {
            this.user = user;
        }

        public override string ToString()
        {
            return $"{packetType,10}" + $"{user}" + $"{message,32}";
        }

        protected MessagePacket()
        {
            
        }
    }

    public class CommandPacket : MessagePacket
    {
        public string command;

        public CommandPacket(PacketType packetType, User user, string command, string message) : base(packetType, user, message)
        {
            this.command = command;
        }

        public override string ToString()
        {
            return $"{packetType,10}" + $"{user}" + $"{command,8}" + $"{message,32}";
        }

        protected CommandPacket() { }
    }


    public class DonationPacket : MessagePacket
    {
        public string amount;

        public DonationPacket(PacketType packetType, User user, string amount, string message) : base(packetType, user, message)
        {
            this.amount = amount;
        }

        public override string ToString()
        {
            return base.ToString() + $"{amount,8}";
        }

        protected DonationPacket()
        {
        }
    }
}