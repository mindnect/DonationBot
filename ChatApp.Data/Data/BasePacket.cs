using System.Xml.Serialization;

namespace ChatAppLib.Data
{
    public enum PacketType
    {
        NONE,
        NOTICE,
        MESSAGE,
        WHISPER,
        SPON,
        COMMAND
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

    [XmlInclude(typeof(MessageBasePacket))]
    public class BasePacket
    {
        public string message;
        public PacketType packetType;

        public BasePacket(PacketType packetType, string message)
        {
            this.packetType = packetType;
            this.message = message;
        }

        protected BasePacket()
        {
        }
    }

    [XmlInclude(typeof(SponBasePacket))]
    [XmlInclude(typeof(CommandBasePacket))]
    public class MessageBasePacket : BasePacket
    {
        public User user;

        public MessageBasePacket(PacketType packetType, User user, string message) : base(packetType, message)
        {
            this.user = user;
        }

        protected MessageBasePacket()
        {
        }

        public override string ToString()
        {
            return $"{packetType,10}" + $"{user}" + $"{message,32}";
        }
    }

    public class CommandBasePacket : MessageBasePacket
    {
        public string command;

        public CommandBasePacket(PacketType packetType, User user, string message, string command) : base(packetType, user, message)
        {
            this.command = command;
        }

        protected CommandBasePacket()
        {
        }

        public override string ToString()
        {
            return $"{packetType,10}" + $"{user}" + $"{command,8}" + $"{message,32}";
        }
    }


    public class SponBasePacket : MessageBasePacket
    {
        public string amount;

        public SponBasePacket(PacketType packetType, User user, string message, string amount) : base(packetType, user, message)
        {
            this.amount = amount;
        }

        protected SponBasePacket()
        {
        }

        public override string ToString()
        {
            return base.ToString() + $"{amount,8}";
        }
    }
}