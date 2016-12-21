using Newtonsoft.Json;

namespace Data
{
    public static class DataExtension
    {
        public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects};

        public static string ToSerialize(this object _this)
        {
            return JsonConvert.SerializeObject(_this, JsonSerializerSettings);
        }
    }

    public class UserEntity
    {
        public string Platform { get; set; }
        public string Rank { get; set; }
        public string Nickname { get; set; }

        public override string ToString()
        {
            return $"{Platform,-8} {Rank,4} {Nickname,-12}";
        }
    }

    public class MessageEntity
    {
        public string Message { get; set; }
    }

    public class ChatEntity : MessageEntity
    {
        public UserEntity UserEntity { get; set; }

        public override string ToString()
        {
            return $"{UserEntity,-24} {Message,-24}";
        }
    }

    public class DonationEntity : ChatEntity
    {
        public int Amount { get; set; } = 0;

        public override string ToString()
        {
            return base.ToString() + $"{Amount,-8}";
        }
    }
}