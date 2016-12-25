using System;
using System.Collections.Generic;
using System.Linq;
using ChatAppLib;
using ChatAppLib.Data;
using RimWorld;
using Verse;

namespace AlcoholV
{
    public class PacketManager
    {
        public static readonly Queue<string> LogQueue = new Queue<string>();
        public static readonly Queue<Action> ShouldExcuteEvent = new Queue<Action>();

        private PacketManager()
        {
        }

        public void Init()
        {
            Client.OnPacket += OnPacket;
            Client.OnLogging += OnLogging;
        }

        public void Update()
        {
            if (LogQueue.Count == 0) return;
            Log.Message(LogQueue.Dequeue());
        }

        protected void OnPacket(Packet packet)
        {
            if (Current.ProgramState != ProgramState.Playing) return; // 게임 중일때만
            var @switch = new Dictionary<Type, Action>
            {
                {typeof(SponPacket), () => OnSpon((SponPacket) packet)},
                {typeof(CommandPacket), () => OnCommand((CommandPacket) packet)}
            };

            @switch[packet.GetType()]();
        }

        protected void OnLogging(string str)
        {
            LogQueue.Enqueue(str);
        }

        protected void OnSpon(SponPacket don)
        {
            if (!AcDonationBot.SponEnable.Value) return; // 스폰 이벤트 가능할때만

            var defName = IncidentManager.Datas.FindAll(x => x.condition == don.amount).RandomElement().defsName;
            var localDef = IncidentDef.Named(defName);

            if (!localDef.Worker.CanFireNow(Find.VisibleMap))
                LogQueue.Enqueue("Can't fire now");

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
            ShouldExcuteEvent.Enqueue(p);
        }


        protected void OnCommand(CommandPacket com)
        {
            if ((com.user.rank != "PD") && (!AcDonationBot.AdCommandEnable.Value || !com.user.rank.Equals("AD"))) return; // PD 명령 가능, 옵션이 꺼졌거나 AD가 아니면 리턴

            var defName = IncidentManager.Datas.Find(x => x.command == com.command)?.defsName;
            if (string.IsNullOrEmpty(defName)) return; // 제대로 된 명령일때만

            var localDef = IncidentDef.Named(defName);

            if (!localDef.Worker.CanFireNow(Find.VisibleMap)) // 실행 가능할때만
            {
                LogQueue.Enqueue("Can't fire now");
                return;
            }
                

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
            ShouldExcuteEvent.Enqueue(p);
        }

        #region Singleton

        private static PacketManager _instance;
        public static PacketManager Instance => _instance ?? (_instance = new PacketManager());

        #endregion
    }
}