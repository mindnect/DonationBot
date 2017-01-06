namespace ChatApp.Server.Models
{
    public class Message
    {
        public Message(string message)
        {
            SelfParse(message);
        }

        private Message() { }

        public string text { get; set; }

        public string param { get; set; }

        public override string ToString()
        {
            return $"{param,10} {text,16}";
        }

        private void SelfParse(string message)
        {
            if (message.StartsWith("@") || message.StartsWith("!"))
            {
                var split = message.Split(new[] {' '}, 2);
                param = split[0];
                if (split.Length > 1) text = split[1];
            }
            else
            {
                text = message;
            }
        }
    }
}