namespace ChatAppLib.Models
{
    public class User
    {
        public User(string nickname, Platform platform, string grade)
        {
            Nickname = nickname;
            Platform = platform;
            Grade = grade;
        }

        public Platform Platform { get; set; }
        public string Grade { get; set; }
        public string Nickname { get; set; }

        public override string ToString()
        {
            return $"{Platform,8} {Grade,4} {Nickname,-12}";
        }
    }
}