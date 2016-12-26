using System;
using System.Collections.Generic;
using System.Linq;
using ChatAppLib;
using ChatAppLib.Data;
using RimWorld;
using Verse;

namespace AlcoholV.Manager
{
    public static class PacketManager
    {
        private static readonly Queue<Action> ActionQueue = new Queue<Action>();

        public static void Init()
        {
            Client.OnPacket += OnPacket;
        }

        private static void OnPacket(BasePacket basePacket)
        {
            LogManager.Enqueue(basePacket.ToString());
            if (Current.ProgramState != ProgramState.Playing) return; // 게임 중일때만
            var @switch = new Dictionary<Type, Action>
            {
                {typeof(SponBasePacket), () => OnSpon((SponBasePacket) basePacket)},
                {typeof(CommandBasePacket), () => OnCommand((CommandBasePacket) basePacket)}
            };

            @switch[basePacket.GetType()]();
        }

        private static void OnSpon(SponBasePacket don)
        {
            if (!AcDonationBot.sponSetting.Value) return; // 스폰 이벤트 가능할때만

            //if (string.IsNullOrEmpty(don.amount)) don.amount = "";
            // 조건 일치 하면서 실행가능한 것들 선택
            var customDefs = DataManager.Datas.FindAll(
                x => (x.condition == don.amount) &&
                     x.def.Worker.CanFireNow(Find.VisibleMap));

            // 일치하는 금액이 없거나 발생가능한 이벤트가 없을 경우
            if (customDefs.NullOrEmpty())
            {
                var t = DataManager.Datas.Find(x => x.condition == don.amount);
                if (t == null) return; // 단순 후원의 경우 리턴
                Action noEventToExcute = () => { Messages.Message(don.user.nickname + "님 가능한 이벤트가 없네요. ㅠㅠ \"" + don.message + "\"", MessageSound.RejectInput); };
                Enqueue(noEventToExcute);
                return;
            }

            // 랜덤 선택
            var customDef = customDefs.RandomElement();
            var sound = MessageSound.Standard;
            switch (customDef.eventType)
            {
                case EventType.DISEASE:
                    sound = MessageSound.Negative;
                    break;
                case EventType.FRIENDLY:
                    sound = MessageSound.Benefit;
                    break;
                case EventType.HOSTILE:
                    sound = MessageSound.SeriousAlert;
                    break;
                case EventType.NATURAL:
                    sound = MessageSound.Benefit;
                    break;
                case EventType.WEATHER:
                    sound = MessageSound.Negative;
                    break;
            }

            Action p = () =>
            {
                Messages.Message(don.user.nickname + "님이 " + customDef.def.label + " 이벤트를 발생시키며 \"" + don.message + "\"", sound);
                var incidentParms = new IncidentParms {target = Find.VisibleMap};
                if (customDef.def.pointsScaleable)
                {
                    var storytellerComp = Find.Storyteller.storytellerComps.First(x => x is StorytellerComp_ThreatCycle || x is StorytellerComp_RandomMain);
                    incidentParms = storytellerComp.GenerateParms(customDef.def.category, Find.VisibleMap);
                }
                customDef.def.Worker.TryExecute(incidentParms);
            };
            Enqueue(p);
        }


        private static void OnCommand(CommandBasePacket com)
        {
            if ((com.user.rank != "PD") && (!AcDonationBot.adCommandSetting.Value || !com.user.rank.Equals("AD"))) return; // PD 명령 가능, 옵션이 꺼졌거나 AD가 아니면 리턴

            var localDef = DataManager.Datas.Find(x => x.command == com.command)?.def;
            if (localDef == null) return; // 제대로 된 명령일때만
            if (!localDef.Worker.CanFireNow(Find.VisibleMap)) // 실행 가능할때만
            {
                LogManager.Enqueue("Can't fire now");
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
            Enqueue(p);
        }

        public static void Enqueue(Action act)
        {
            ActionQueue.Enqueue(act);
        }

        public static Action Dequeue()
        {
            return ActionQueue.Dequeue();
        }

        public static int Count()
        {
            return ActionQueue.Count;
        }

        public static void Clear()
        {
            ActionQueue.Clear();
        }

    }
}