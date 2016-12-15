using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ChatApp.Incident
{
    public class AcIncidentWorker_RaidEnemy : IncidentWorker_RaidEnemy
    {
        private string _name;
        private string _message;

        public bool TryExecute(IncidentParms parms, string name, string msg)
        {
            _name = name;
            _message = msg;
            return base.TryExecute(parms);
        }

        protected override string GetLetterLabel(IncidentParms parms)
        {
            var text = base.GetLetterLabel(parms);
            return _name + "�� " + text; // xxx�� ���� (ȭ�� ���� �޼��� ����)
        }

        protected override string GetLetterText(IncidentParms parms, List<Pawn> pawns)
        {
            string text = null;
            switch (parms.raidArrivalMode)
            {
                case PawnsArriveMode.EdgeWalkIn:
                    text = "EnemyRaidWalkIn".Translate("\"" + _name + "\"", parms.faction.Name);
                    break;
                case PawnsArriveMode.EdgeDrop:
                    text = "EnemyRaidEdgeDrop".Translate("\"" + _name + "\"", parms.faction.Name);
                    break;
                case PawnsArriveMode.CenterDrop:
                    text = "EnemyRaidCenterDrop".Translate("\"" + _name + "\"", parms.faction.Name);
                    break;
            }
            text += "\n\n" + "\"" + _message + "\"";

            text += "\n\n";
            text += parms.raidStrategy.arrivalTextEnemy;

            Pawn pawn;

            // �� �۸�
            for (var i = 0; i < pawns.Count; i++)
            {
                pawn = pawns[i];

                var nick = _name + " " + i + "��";
                if (i == 0) nick = _name;

                if (pawn.Name != null)
                {
                    var originName = (NameTriple) pawn.Name;
                    pawn.Name = new NameTriple(originName.First, nick, originName.Last);
                }
                else
                {
                    pawn.Name = new NameSingle(nick);
                }
            }

            pawn = pawns.Find(x => x.Faction.leader == x);
            if (pawn != null)
            {
                text += "\n\n";
                text += "EnemyRaidLeaderPresent".Translate(pawn.Faction.def.pawnsPlural, pawn.LabelShort);
            }

            return text;
        }
    }
}