namespace ChatAppLib.Models
{
    public class Message
    {
        public Message(string message)
        {
            SelfParse(message);
        }

        public string Text { get; set; }

        public string Param { get; set; }

        public override string ToString()
        {
            return $"{Param,10} {Text,16}";
        }

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
    }
}