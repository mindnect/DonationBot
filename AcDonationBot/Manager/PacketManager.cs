using System;
using System.Collections.Generic;
using AlcoholV;
using AlcoholV.Manager;
using ChatApp.Client;
using ChatApp.Client.Models;
using RimWorld;
using Verse;

namespace Ac.DonationBot.Manager
{
    public static class PacketManager
    {
        private static readonly Queue<Action> Queue = new Queue<Action>();

        public static void Init()
        {
            Client.OnOpen += () => { Log("Open"); };
            Client.OnRetry += () => { Log("Retry"); };
            Client.OnError += Log;
            Client.OnSpon += OnSpon;
            Client.OnChat += OnChat;
        }

        public static int Count()
        {
            return Queue.Count;
        }

        public static void Clear()
        {
            lock (Queue)
            {
                Queue.Clear();
            }
        }

        public static void Update()
        {
            while (Queue.Count > 0)
                lock (Queue)
                {
                    Queue.Dequeue().Invoke();
                }
        }

        public static void Message(string str, MessageSound sound)
        {
            Enqueue(() => { Messages.Message(str, sound); });
        }


        private static void OnChat(Packet packet)
        {
        }

        private static void Enqueue(Action action)
        {
            if (action == null) return;

            lock (Queue)
            {
                Queue.Enqueue(action);
            }
        }

        private static void Log(string str)
        {
            Enqueue(() => { Verse.Log.Message(str); });
        }

        private static void OnSpon(Packet packet)
        {
            if (Current.ProgramState != ProgramState.Playing || !AcDonationBot.sponEventEnabled.Value) return;

            var amount = Convert.ToInt32(packet.amount);
            var customDefs = DataManager.FindAll(amount); // 조건 만족하는 이벤트 모두 선택
            var commands = ParseCommand(packet.message.param); // 명령어 파싱
            Action act = null;

            if (customDefs.NullOrEmpty())
            {
                // 단순 후원
                if (commands.NullOrEmpty())
                    Message(packet.user.nickname + "님 가능한 이벤트가 없네요. ㅠㅠ ", MessageSound.RejectInput);
            }
            else
            {
                CustomIncident selectedIncident = null;
                IIncidentTarget target = null;

                // 이벤트 지정 명령어가 있을 경우
                if (!commands.NullOrEmpty())
                {
                    var foundIncdent = customDefs.Find(x => x.command == commands[0]);
                    if (foundIncdent != null)
                    {
                        target = foundIncdent.def.category == IncidentCategory.CaravanTarget
                            ? (IIncidentTarget) Find.WorldObjects.Caravans.RandomElement()
                            : Find.VisibleMap;
                        selectedIncident = foundIncdent.def.Worker.CanFireNow(target) ? foundIncdent : null;
                    }
                }

                // 이벤트 지정 명령어가 없을 경우는 랜덤 선택
                else
                {
                    // 현재 지도에서 가능한 것들 선택
                    var possibleDefs = customDefs.FindAll(x => x.def.Worker.CanFireNow(Find.VisibleMap));
                    selectedIncident = possibleDefs.RandomElement();
                    target = Find.VisibleMap;
                }


                if (selectedIncident != null)
                {
                    var sound = MessageSound.Standard;
                    switch (selectedIncident.eventType)
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
                            sound = MessageSound.Standard;
                            break;
                        case EventType.WEATHER:
                            sound = MessageSound.Negative;
                            break;
                    }

                    act = () =>
                    {
                        Messages.Message(
                            packet.user.nickname + "님이 " + selectedIncident.def.label + " 이벤트를 발생시켰습니다. " +
                            packet.message.text, sound);

                        var incidentParms = new IncidentParms();
                        incidentParms.target = target;
                        if (selectedIncident.def.pointsScaleable)
                            incidentParms.points = amount / 4f;
                        selectedIncident.def.Worker.TryExecute(incidentParms);
                    };
                }
                else
                {
                    Message(packet.user.nickname + "님 가능한 이벤트가 없네요. ㅠㅠ ", MessageSound.RejectInput);
                }
            }

            Enqueue(act);
        }

        private static string[] ParseCommand(string param)
        {
            // 명령어 파싱 @습격/공병
            if (param.NullOrEmpty()) return null;

            var commands = param.Split('/');
            commands[0] = commands[0].TrimStart('@');
            return commands;
        }
    }
}