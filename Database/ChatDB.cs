namespace Database
{
    public class UserData
    {
        public string Platform { get; set; }
        public string Rank { get; set; }

        public string NickName { get; set; }

        public override string ToString()
        {
            return $"{Platform,-8} {Rank,4} {NickName,-12}";
        }
    }

    public class ChatData
    {
        public UserData UserData { get; set; }

        public string Message { get; set; }

        public override string ToString()
        {
            return $"{UserData,-24} {Message,-24}";
        }
    }

    public class DonationData : ChatData
    {
        public int Amount { get; set; }

        public override string ToString()
        {
            return base.ToString() + $"{Amount,-8}";
        }
    }
}