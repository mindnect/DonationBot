namespace ChatApp.Server.Models
{
    public class Message
    {
        public Message(string message)
        {
            SelfParse(message);
        }

        public string msg { get; set; }

        public string param { get; set; }

        public override string ToString()
        {
            return $"{param,10} {msg,16}";
        }

        private void SelfParse(string message)
        {
            if (message.StartsWith("@"))
            {
                var split = message.Split(new[] {' '}, 2);
                param = split[0];
                if (split.Length > 1) msg = split[1];
            }
            else
            {
                msg = message;
            }
        }
    }
}