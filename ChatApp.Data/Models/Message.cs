using Newtonsoft.Json;

namespace ChatAppLib.Models
{
    public class Message
    {
        public Message(string message)
        {
            SelfParse(message);
        }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Param { get; set; }

        public string Text { get; set; }

        private void SelfParse(string message)
        {
            if (message.StartsWith("@"))
            {
                var split = message.Split(new[] {' '}, 2);
                Param = split[0];
                if (split.Length > 1) Text = split[1];
            }
            else
            {
                Text = message;
            }
        }

        public override string ToString()
        {
            return $"{Param,10} {Text,16}";
        }
    }
}