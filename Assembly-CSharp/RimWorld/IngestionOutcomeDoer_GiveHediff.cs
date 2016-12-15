using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public class IngestionOutcomeDoer_GiveHediff : IngestionOutcomeDoer
	{
		public HediffDef hediffDef;

		public float severity = -1f;

		public ChemicalDef toleranceChemical;

		private bool divideByBodySize;

		protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
		{
			Hediff hediff = HediffMaker.MakeHediff(this.hediffDef, pawn, null);
			float num;
			if (this.severity > 0f)
			{
				num = this.severity;
			}
			else
			{
				num = this.hediffDef.initialSeverity;
			}
			if (this.divideByBodySize)
			{
				num /= pawn.BodySize;
			}
			AddictionUtility.ModifyChemicalEffectForToleranceAndBodySize(pawn, this.toleranceChemical, ref num);
			hediff.Severity = num;
			pawn.health.AddHediff(hediff, null, null);
		}

		[DebuggerHidden]
		public override IEnumerable<StatDrawEntry> SpecialDisplayStats(ThingDef parentDef)
		{
			IngestionOutcomeDoer_GiveHediff.<SpecialDisplayStats>c__Iterator7E <SpecialDisplayStats>c__Iterator7E = new IngestionOutcomeDoer_GiveHediff.<SpecialDisplayStats>c__Iterator7E();
			<SpecialDisplayStats>c__Iterator7E.parentDef = parentDef;
			<SpecialDisplayStats>c__Iterator7E.<$>parentDef = parentDef;
			<SpecialDisplayStats>c__Iterator7E.<>f__this = this;
			IngestionOutcomeDoer_GiveHediff.<SpecialDisplayStats>c__Iterator7E expr_1C = <SpecialDisplayStats>c__Iterator7E;
			expr_1C.$PC = -2;
			return expr_1C;
		}
	}
}
