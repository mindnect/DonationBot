namespace ChatAppLib.Models
{
    public class Message
    {

        public string Text { get; set; }

        public string Param { get; set; }

        public override string ToString()
        {
            return $"{Param,10} {Text,16}";
        }
    }
}