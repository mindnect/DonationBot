using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Verse.AI
{
	public class JobDriver_Kill : JobDriver
	{
		private const TargetIndex VictimInd = TargetIndex.A;

		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_Kill.<MakeNewToils>c__Iterator195 <MakeNewToils>c__Iterator = new JobDriver_Kill.<MakeNewToils>c__Iterator195();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_Kill.<MakeNewToils>c__Iterator195 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
