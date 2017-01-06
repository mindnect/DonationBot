namespace ChatApp.Server.Models
{
    public class User
    {
        public User(string nickname, Platform platform, string grade=null)
        {
            this.nickname = nickname;
            this.platform = platform;
            this.grade = grade;
        }

        private User()
        {
            
        }

        public Platform platform { get; set; }
        public string grade { get; set; }
        public string nickname { get; set; }

        public override string ToString()
        {
            return $"{platform,8} {grade,4} {nickname,-12}";
        }
    }
}