using System;
using System.Collections.Generic;
using System.Linq;
using AlcoholV.Incident;
using Comm;
using Comm.Packets;
using RimWorld;
using Verse;

namespace AlcoholV.Event
{
    public static class MessageManager
    {
        public static readonly Queue<Action> ShouldExcute;
        private static readonly Queue<string> MessageQueue;

        static MessageManager()
        {
            ShouldExcute = new Queue<Action>();
            MessageQueue = new Queue<string>();
        }

        public static void Init()
        {
            Client.OnPacket += OnPacket;
            Client.OnLogging += OnLogging;
        }

        public static void Update()
        {
            if (MessageQueue.Count == 0) return;

            Log.Message(MessageQueue.Dequeue());
        }

        private static void OnPacket(Packet obj)
        {
            var @switch = new Dictionary<Type, Action>
            {
                {typeof(Chat), () => OnChat((Chat) obj)},
                {typeof(Donation), () => OnDonation((Donation) obj)},
                {typeof(Notice), () => OnNotice((Notice) obj)},
                {typeof(Order), () => OnOrder((Order) obj)}
            };

            @switch[obj.GetType()]();
        }

        private static void OnLogging(string str)
        {
            MessageQueue.Enqueue(str);
        }

        private static void OnDonation(Donation donation)
        {
            Log.Message(donation.ToString());
        }

        private static void OnChat(Chat chat)
        {
            Log.Message(chat.ToString());
        }

        private static void OnOrder(Order order)
        {
            //Log.Message(order.ToString());
            //Action a = delegate
            //{
            var localDef = IncidentDefOf.RaidEnemy;
            IncidentParms parms = null;
            var t = new AcIncidentWorker_RaidEnemy {def = localDef};
            if (localDef.pointsScaleable)
            {
                //스케일러블 레이드 (팩션 생성. RAID때는 우호 팩션 오류남)
                var storytellerComp = Find.Storyteller.storytellerComps.First(x => x is StorytellerComp_ThreatCycle || x is StorytellerComp_RandomMain);
                parms = storytellerComp.GenerateParms(localDef.category, Find.VisibleMap);
            }
            t.TryExecute(parms, order.user.nickname, order.command);
            //};
            //ShouldExcute.Add(a);
        }

        private static void OnNotice(Notice notice)
        {
        }


    }
}