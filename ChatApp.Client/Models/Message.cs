namespace ChatApp.Client.Models
{
    public class Message
    {
        public string msg { get; set; }

        public string param { get; set; }

        public override string ToString()
        {
            return $"{param,10} {msg,16}";
        }
    }
}