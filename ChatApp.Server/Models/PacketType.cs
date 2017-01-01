namespace ChatApp.Server.Models
{
    public enum PacketType
    {
        log,
        notice,
        enter,
        rename,
        exit,
        chat,
        whisper,
        spon
    }
}