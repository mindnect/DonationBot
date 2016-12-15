using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Verse.AI
{
	public class JobDriver_HaulToContainer : JobDriver
	{
		private const TargetIndex CarryThingIndex = TargetIndex.A;

		private const TargetIndex DestIndex = TargetIndex.B;

		public override string GetReport()
		{
			Thing thing;
			if (this.pawn.carryTracker.CarriedThing != null)
			{
				thing = this.pawn.carryTracker.CarriedThing;
			}
			else
			{
				thing = base.TargetThingA;
			}
			return "ReportHaulingTo".Translate(new object[]
			{
				thing.LabelCap,
				base.CurJob.targetB.Thing.LabelShort
			});
		}

		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_HaulToContainer.<MakeNewToils>c__Iterator199 <MakeNewToils>c__Iterator = new JobDriver_HaulToContainer.<MakeNewToils>c__Iterator199();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_HaulToContainer.<MakeNewToils>c__Iterator199 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
