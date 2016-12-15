namespace ChatApp
{
    internal class ChatData
    {
        public bool isWhisper = false;
        public bool isNotice = false;
        public bool isDonation = false;
        public int amount = 0;
        public UserData user = new UserData();
        public string message = "";
    }
}