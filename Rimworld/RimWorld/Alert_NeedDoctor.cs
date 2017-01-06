using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
	public class Alert_NeedDoctor : Alert
	{
		private IEnumerable<Pawn> Patients
		{
			get
			{
				Alert_NeedDoctor.<>c__Iterator16B <>c__Iterator16B = new Alert_NeedDoctor.<>c__Iterator16B();
				Alert_NeedDoctor.<>c__Iterator16B expr_07 = <>c__Iterator16B;
				expr_07.$PC = -2;
				return expr_07;
			}
		}

		public Alert_NeedDoctor()
		{
			this.defaultLabel = "NeedDoctor".Translate();
			this.defaultPriority = AlertPriority.High;
		}

		public override string GetExplanation()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Pawn current in this.Patients)
			{
				stringBuilder.AppendLine("    " + current.NameStringShort);
			}
			return string.Format("NeedDoctorDesc".Translate(), stringBuilder.ToString());
		}

		public override AlertReport GetReport()
		{
			if (Find.AnyPlayerHomeMap == null)
			{
				return false;
			}
			Pawn pawn = this.Patients.FirstOrDefault<Pawn>();
			if (pawn == null)
			{
				return false;
			}
			return AlertReport.CulpritIs(pawn);
		}
	}
}
