using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse.AI;

namespace RimWorld
{
	public class JobDriver_Flick : JobDriver
	{
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_Flick.<MakeNewToils>c__Iterator2C <MakeNewToils>c__Iterator2C = new JobDriver_Flick.<MakeNewToils>c__Iterator2C();
			<MakeNewToils>c__Iterator2C.<>f__this = this;
			JobDriver_Flick.<MakeNewToils>c__Iterator2C expr_0E = <MakeNewToils>c__Iterator2C;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
