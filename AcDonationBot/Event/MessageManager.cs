using System;
using System.Collections.Generic;
using Comm.Packets;
using Verse;

namespace AlcoholV.Event
{
    public static class MessageManager
    {
        public static void OnMessage(Packet obj)
        {
            var @switch = new Dictionary<Type, Action>
            {
                {typeof(Chat), () => OnChat((Chat) obj)},
                {typeof(Donation), () => OnDonation((Donation) obj)},
                {typeof(Notice), () => OnNotice((Notice) obj)}
            };

            @switch[obj.GetType()]();
        }

        private static void OnDonation(Donation donation)
        {
            Log.Message(donation.amount);
        }

        private static void OnChat(Chat chat)
        {
            Log.Message(chat.user.nickname);
        }

        private static void OnNotice(Notice notice)
        {
        }
    }
}