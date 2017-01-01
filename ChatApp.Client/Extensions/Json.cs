﻿using ChatAppLib.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace ChatAppLib.Extensions
{
    public static class JSon
    {
        public static string Serialize(this Packet _this)
        {
            return JsonConvert.SerializeObject(_this);
        }

        public static string PrettySerialize(this Packet _this)
        {
            return JsonConvert.SerializeObject(_this, Formatting.Indented);
        }

        public static T DeSerialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}