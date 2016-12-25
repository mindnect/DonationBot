using System;
using System.Collections.Generic;
using System.Linq;
using ChatAppLib;
using ChatAppLib.Data;
using RimWorld;
using Verse;

namespace AlcoholV
{
    public class MessageManager
    {
        public readonly Queue<string> logQueue = new Queue<string>();
        public readonly Queue<Action> shouldExcute = new Queue<Action>();

        private MessageManager()
        {
        }

        public void Init()
        {
            Client.OnPacket += OnPacket;
            Client.OnLogging += OnLogging;
        }

        public void Update()
        {
            if (logQueue.Count == 0) return;
            Log.Message(logQueue.Dequeue());
        }

        protected void OnPacket(Packet obj)
        {
            var @switch = new Dictionary<Type, Action>
            {
                {typeof(DonationPacket), () => OnDonation((DonationPacket) obj)},
                {typeof(CommandPacket), () => OnCommand((CommandPacket) obj)}
            };

            @switch[obj.GetType()]();
        }

        protected void OnLogging(string str)
        {
            logQueue.Enqueue(str);
        }

        protected void OnDonation(DonationPacket don)
        {
            if (!AcDonationBot.Instance.donationHandle.Value) return;

            var defName = DataManager.Instance.datas.FindAll(x => x.condition == don.amount).RandomElement().defsName;
            var localDef = IncidentDef.Named(defName);


            if (!localDef.Worker.CanFireNow(Find.VisibleMap))
                logQueue.Enqueue("Can't fire now");

            Action p = () =>
            {
                var incidentParms = new IncidentParms {target = Find.VisibleMap};
                if (localDef.pointsScaleable)
                {
                    var storytellerComp = Find.Storyteller.storytellerComps.First(x => x is StorytellerComp_ThreatCycle || x is StorytellerComp_RandomMain);
                    incidentParms = storytellerComp.GenerateParms(localDef.category, Find.VisibleMap);
                }
                localDef.Worker.TryExecute(incidentParms);
            };
            shouldExcute.Enqueue(p);
        }

        protected void OnCommand(CommandPacket com)
        {
            if(com.user.rank == "PD" || (AcDonationBot.Instance.adExcuteHandle.Value && com.user.rank.Equals("AD")))
            {
                // com.command
                var defName = DataManager.Instance.datas.Find(x => x.command == com.command).defsName;
                var localDef = IncidentDef.Named(defName);


                if (!localDef.Worker.CanFireNow(Find.VisibleMap))
                    logQueue.Enqueue("Can't fire now");

                Action p = () =>
                {
                    var incidentParms = new IncidentParms {target = Find.VisibleMap};
                    if (localDef.pointsScaleable)
                    {
                        var storytellerComp = Find.Storyteller.storytellerComps.First(x => x is StorytellerComp_ThreatCycle || x is StorytellerComp_RandomMain);
                        incidentParms = storytellerComp.GenerateParms(localDef.category, Find.VisibleMap);
                    }
                    localDef.Worker.TryExecute(incidentParms);
                };
                shouldExcute.Enqueue(p);
            }
        }

        #region Singleton

        private static MessageManager _instance;
        public static MessageManager Instance => _instance ?? (_instance = new MessageManager());

        #endregion
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
//    t.TryExecute(parms, order.user.nickname, command.command);
//    //};
//    //ShouldExcute.Add(a);
//}