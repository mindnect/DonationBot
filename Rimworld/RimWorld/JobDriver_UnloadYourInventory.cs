using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse.AI;

namespace RimWorld
{
	public class JobDriver_UnloadYourInventory : JobDriver
	{
		private const TargetIndex ItemToHaulInd = TargetIndex.A;

		private const TargetIndex StoreCellInd = TargetIndex.B;

		private const int UnloadDuration = 10;

		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_UnloadYourInventory.<MakeNewToils>c__Iterator40 <MakeNewToils>c__Iterator = new JobDriver_UnloadYourInventory.<MakeNewToils>c__Iterator40();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_UnloadYourInventory.<MakeNewToils>c__Iterator40 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
