﻿using System;
using Verse;

namespace AlcoholV.Event
{
    internal class EventComponent : MapComponent
    {
        //public override void MapComponentUpdate()
        //{
        //    foreach (var action in MessageManager.ShouldExcute.ToArray())
        //    {
        //        MessageManager.ShouldExcute.Remove(action);
        //        action.Invoke();

        //    }
        //}

        public EventComponent(Map map) : base(map)
        {
           // MessageManager.ShouldExcute.Clear();
        }


        //    {
        //    if (Find.Storyteller != null)

        //{

        //private void OnChanged(object source, FileSystemEventArgs e)

        //        var addedContent = _fReader.GetAddedLines();
        //        var line = addedContent.Split('\n');

        //        foreach (var s in line)
        //        {
        //            if (s.Length < 1) continue; // 임시 

        //            var text = s.Split('/');

        //            //Daum / Msg / PD / 알콜V / . / 0

        //            var name = text[3];
        //            var message = text[4];
        //            var amount = text[5];
        //            var localDef = IncidentDefOf.RaidEnemy;
        //            var money = int.Parse(amount);

        //            if ((money > 0)) // || message.Contains(""))) //|| message.Contains("!테스트"))
        //            {
        //                Action p = delegate
        //                {
        //                    IncidentParms parms = null;
        //                    var t = new AcIncidentWorker_RaidEnemy { def = localDef };
        //                    if (localDef.pointsScaleable)
        //                    {
        //                        //스케일러블 레이드 (팩션 생성. RAID때는 우호 팩션 오류남)
        //                        var storytellerComp = Find.Storyteller.storytellerComps.First(x => x is StorytellerComp_ThreatCycle || x is StorytellerComp_RandomMain);
        //                        parms = storytellerComp.GenerateParms(localDef.category, Find.VisibleMap);

        //                        //FactionDef facDef = (from fa in DefDatabase<FactionDef>.AllDefs
        //                        //                     where fa.canMakeRandomly && Find.FactionManager.AllFactions.Count((Faction f) => f.def == fa) < fa.maxCountAtGameStart
        //                        //                     select fa).RandomElement<FactionDef>();
        //                        //Faction faction = FactionGenerator.NewGeneratedFaction(facDef);

        //                        //faction.Name = name;
        //                        //Find.FactionManager.Add(faction);
        //                        //parms.faction = faction;

        //                        //// 메카노이드 레이드
        //                        //StorytellerComp storytellerComp = Find.Storyteller.storytellerComps.First((StorytellerComp x) => x is StorytellerComp_ThreatCycle || x is StorytellerComp_RandomMain);
        //                        //parms = storytellerComp.GenerateParms(IncidentCategory.ThreatBig);
        //                        //parms.faction = Faction.OfMechanoids;
        //                        //parms.points = 100;
        //                        //parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
        //                    }
        //                    t.TryExecute(parms, name, message);
        //                };
        //                _shouldExcute.Add(p);
        //            }
        //        }
        //    }
        //}


        private void OnMessage(object sender, string str)
        {
            // Log.MessagePacket(packetType +"," + message);

            //Log.MessagePacket(str);

            //var name = data["name"];
            //var message = text[4];
            //var amount = text[5];
            //var localDef = IncidentDefOf.RaidEnemy;
            //var money = int.Parse(amount);

            //if (money > 0) // || message.Contains(""))) //|| message.Contains("!테스트"))
            //{
            //    Action p = delegate
            //    {
            //        IncidentParms parms = null;
            //        var t = new AcIncidentWorker_RaidEnemy {def = localDef};
            //        if (localDef.pointsScaleable)
            //        {
            //            //스케일러블 레이드 (팩션 생성. RAID때는 우호 팩션 오류남)
            //            var storytellerComp = Find.Storyteller.storytellerComps.First(x => x is StorytellerComp_ThreatCycle || x is StorytellerComp_RandomMain);
            //            parms = storytellerComp.GenerateParms(localDef.category, Find.VisibleMap);

            //            //FactionDef facDef = (from fa in DefDatabase<FactionDef>.AllDefs
            //            //                     where fa.canMakeRandomly && Find.FactionManager.AllFactions.Count((Faction f) => f.def == fa) < fa.maxCountAtGameStart
            //            //                     select fa).RandomElement<FactionDef>();
            //            //Faction faction = FactionGenerator.NewGeneratedFaction(facDef);

            //            //faction.Name = name;
            //            //Find.FactionManager.Add(faction);
            //            //parms.faction = faction;

            //            //// 메카노이드 레이드
            //            //StorytellerComp storytellerComp = Find.Storyteller.storytellerComps.First((StorytellerComp x) => x is StorytellerComp_ThreatCycle || x is StorytellerComp_RandomMain);
            //            //parms = storytellerComp.GenerateParms(IncidentCategory.ThreatBig);
            //            //parms.faction = Faction.OfMechanoids;
            //            //parms.points = 100;
            //            //parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
            //        }
            //        t.TryExecute(parms, name, message);
            //    };
            //    _shouldExcute.Add(p);
            //}
        }
    }
}