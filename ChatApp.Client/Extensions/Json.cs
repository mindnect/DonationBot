using ChatApp.Client.Models;
using Newtonsoft.Json;

namespace ChatApp.Client.Extensions
{
    public static class JSon
    {
        public static string Serialize(this Packet _this)
        {
            return JsonConvert.SerializeObject(_this);
        }

        public static Packet DeSerialize(string json)
        {
            return JsonConvert.DeserializeObject<Packet>(json);
        }
    }
}