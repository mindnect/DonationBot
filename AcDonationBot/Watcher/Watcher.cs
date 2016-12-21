using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data.Incident;
using RimWorld;
using Verse;

namespace Data.Watcher
{
    internal class Watcher : MapComponent
    {
        private readonly AddedContentReader _fReader;
        private List<Action> _shouldExcute = new List<Action>();

        public Watcher(Map map) : base(map)
        {
            _fReader = new AddedContentReader(AcDonationBot.path);

            var watcher = new FileSystemWatcher
            {
                Path = "c:\\DonationBot\\",
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "Donation.txt"
            };
            watcher.Changed += OnChanged;
            watcher.EnableRaisingEvents = true;
        }

        public override void MapComponentUpdate()
        {
            base.MapComponentUpdate();
            foreach (var action in _shouldExcute)
            {
                action.Invoke();
            }
            _shouldExcute.Clear();
        }


        private void OnChanged(object source, FileSystemEventArgs e)
        {
            if (Find.Storyteller != null)
            {
                var addedContent = _fReader.GetAddedLines();
                var line = addedContent.Split('\n');

                foreach (var s in line)
                {
                    if (s.Length < 1) continue; // 임시 

                    var text = s.Split('/');

                    //Daum / Msg / PD / 알콜V / . / 0

                    var name = text[3];
                    var msg = text[4];
                    var amount = text[5];
                    var localDef = IncidentDefOf.RaidEnemy;
                    var money = int.Parse(amount);
                    
                    if ((money > 0)) // || msg.Contains(""))) //|| msg.Contains("!테스트"))
                    {
                        Action p = delegate
                        {
                            IncidentParms parms = null;
                            var t = new AcIncidentWorker_RaidEnemy { def = localDef };
                            if (localDef.pointsScaleable)
                            {
                                //스케일러블 레이드 (팩션 생성. RAID때는 우호 팩션 오류남)
                                var storytellerComp = Find.Storyteller.storytellerComps.First(x => x is StorytellerComp_ThreatCycle || x is StorytellerComp_RandomMain);
                                parms = storytellerComp.GenerateParms(localDef.category, Find.VisibleMap);

                                //FactionDef facDef = (from fa in DefDatabase<FactionDef>.AllDefs
                                //                     where fa.canMakeRandomly && Find.FactionManager.AllFactions.Count((Faction f) => f.def == fa) < fa.maxCountAtGameStart
                                //                     select fa).RandomElement<FactionDef>();
                                //Faction faction = FactionGenerator.NewGeneratedFaction(facDef);

                                //faction.Name = name;
                                //Find.FactionManager.Add(faction);
                                //parms.faction = faction;

                                //// 메카노이드 레이드
                                //StorytellerComp storytellerComp = Find.Storyteller.storytellerComps.First((StorytellerComp x) => x is StorytellerComp_ThreatCycle || x is StorytellerComp_RandomMain);
                                //parms = storytellerComp.GenerateParms(IncidentCategory.ThreatBig);
                                //parms.faction = Faction.OfMechanoids;
                                //parms.points = 100;
                                //parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
                            }
                            t.TryExecute(parms, name, msg);
                        };
                        _shouldExcute.Add(p);
                    }
                }
            }
        }
    }
}