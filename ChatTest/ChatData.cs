using LiteDB;

namespace ChatTest
{
    public enum MessageType
    {
        Error,
        Msg,
        Whisper,
        Notice,
        Donation
    }

    public class UserData
    {
        public int Id { get; set; }
        public string Platform { get; set; }
        public string Rank { get; set; }
        public string NickName { get; set; }

        public override string ToString()
        {
            return string.Format("{0,-8} {1,4} {2,-12}", Platform, Rank, NickName);

            //return messageType.ToString() + user.user.nickName,
        }
    }

    public class ChatData
    {
        public int Id { get; set; }
        public MessageType MessageType { get; set; }

        [BsonRef("Users")]
        public UserData User { get; set; }

        public string Message { get; set; }
        public int Amount { get; set; }

        public override string ToString()
        {
            return string.Format("{0,-8} {1} {2,-32} {3}", MessageType, User, Message, Amount);

            //return messageType.ToString() + user.user.nickName,
        }
    }
}