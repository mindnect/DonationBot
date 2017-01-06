using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
	public class Alert_ColonistNeedsTend : Alert
	{
		private IEnumerable<Pawn> NeedingColonists
		{
			get
			{
				Alert_ColonistNeedsTend.<>c__Iterator16C <>c__Iterator16C = new Alert_ColonistNeedsTend.<>c__Iterator16C();
				Alert_ColonistNeedsTend.<>c__Iterator16C expr_07 = <>c__Iterator16C;
				expr_07.$PC = -2;
				return expr_07;
			}
		}

		public Alert_ColonistNeedsTend()
		{
			this.defaultLabel = "ColonistNeedsTreatment".Translate();
			this.defaultPriority = AlertPriority.High;
		}

		public override string GetExplanation()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Pawn current in this.NeedingColonists)
			{
				stringBuilder.AppendLine("    " + current.NameStringShort);
			}
			return string.Format("ColonistNeedsTreatmentDesc".Translate(), stringBuilder.ToString());
		}

		public override AlertReport GetReport()
		{
			Pawn pawn = this.NeedingColonists.FirstOrDefault<Pawn>();
			if (pawn == null)
			{
				return false;
			}
			return AlertReport.CulpritIs(pawn);
		}
	}
}
