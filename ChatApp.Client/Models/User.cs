namespace ChatApp.Client.Models
{
    public class User
    {
        public Platform platform { get; set; }
        public string grade { get; set; }
        public string nickname { get; set; }

        public override string ToString()
        {
            return $"{platform,8} {grade,4} {nickname,-12}";
        }
    }
}