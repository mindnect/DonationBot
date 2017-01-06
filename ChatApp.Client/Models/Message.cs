namespace ChatApp.Client.Models
{
    public class Message
    {
        public string text { get; set; }

        public string param { get; set; }

        public override string ToString()
        {
            return $"{param,10} {text,16}";
        }
    }
}