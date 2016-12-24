using System;
using System.Collections.Generic;
using ChatAppLib;
using ChatAppLib.Data;
using Verse;

namespace AlcoholV
{
    public static class MessageManager
    {
        public static readonly Queue<Action> ShouldExcute;
        private static readonly Queue<string> LogQueue;

        static MessageManager()
        {
            ShouldExcute = new Queue<Action>();
            LogQueue = new Queue<string>();
        }

        public static void Init()
        {
            Client.OnPacket += OnPacket;
            Client.OnLogging += OnLogging;
        }

        public static void Update()
        {
            if (LogQueue.Count == 0) return;
            Log.Message(LogQueue.Dequeue());
        }

        private static void OnPacket(Packet obj)
        {
            var @switch = new Dictionary<Type, Action>
            {
                {typeof(DonationPacket), () => OnDonation((DonationPacket) obj)},
                {typeof(CommandPacket), () => OnCommand((CommandPacket) obj)}
            };

            @switch[obj.GetType()]();
        }

        private static void OnLogging(string str)
        {
            LogQueue.Enqueue(str);
        }

        private static void OnDonation(DonationPacket donationPacket)
        {
            LogQueue.Enqueue(donationPacket.ToString());
        }

        private static void OnCommand(CommandPacket command)
        {
            LogQueue.Enqueue(command.ToString());
        }
    }
}



//private static void OnOrder(Command order)
//{
//    //Log.MessagePacket(order.ToString());
//    //Action a = delegate
//    //{
//    var localDef = IncidentDefOf.RaidEnemy;
//    IncidentParms parms = null;
//    var t = new AcIncidentWorker_RaidEnemy {def = localDef};
//    if (localDef.pointsScaleable)
//    {
//        //스케일러블 레이드 (팩션 생성. RAID때는 우호 팩션 오류남)
//        var storytellerComp = Find.Storyteller.storytellerComps.First(x => x is StorytellerComp_ThreatCycle || x is StorytellerComp_RandomMain);
//        parms = storytellerComp.GenerateParms(localDef.category, Find.VisibleMap);
//    }
//    t.TryExecute(parms, order.user.nickname, order.command);
//    //};
//    //ShouldExcute.Add(a);
//}
