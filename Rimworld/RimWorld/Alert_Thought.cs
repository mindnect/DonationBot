using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
	public abstract class Alert_Thought : Alert
	{
		protected string explanationKey;

		private static List<Thought> tmpThoughts = new List<Thought>();

		protected abstract ThoughtDef Thought
		{
			get;
		}

		[DebuggerHidden]
		private IEnumerable<Pawn> AffectedPawns()
		{
			Alert_Thought.<AffectedPawns>c__Iterator16E <AffectedPawns>c__Iterator16E = new Alert_Thought.<AffectedPawns>c__Iterator16E();
			<AffectedPawns>c__Iterator16E.<>f__this = this;
			Alert_Thought.<AffectedPawns>c__Iterator16E expr_0E = <AffectedPawns>c__Iterator16E;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		public override AlertReport GetReport()
		{
			Pawn pawn = this.AffectedPawns().FirstOrDefault<Pawn>();
			if (pawn != null)
			{
				return AlertReport.CulpritIs(pawn);
			}
			return AlertReport.Inactive;
		}

		public override string GetExplanation()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Pawn current in this.AffectedPawns())
			{
				stringBuilder.AppendLine("    " + current.NameStringShort);
			}
			return this.explanationKey.Translate(new object[]
			{
				stringBuilder.ToString()
			});
		}
	}
}
