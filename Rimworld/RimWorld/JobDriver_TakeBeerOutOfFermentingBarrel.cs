using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class JobDriver_TakeBeerOutOfFermentingBarrel : JobDriver
	{
		private const TargetIndex BarrelInd = TargetIndex.A;

		private const TargetIndex BeerToHaulInd = TargetIndex.B;

		private const TargetIndex StorageCellInd = TargetIndex.C;

		private const int Duration = 200;

		protected Building_FermentingBarrel Barrel
		{
			get
			{
				return (Building_FermentingBarrel)base.CurJob.GetTarget(TargetIndex.A).Thing;
			}
		}

		protected Thing Beer
		{
			get
			{
				return base.CurJob.GetTarget(TargetIndex.B).Thing;
			}
		}

		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_TakeBeerOutOfFermentingBarrel.<MakeNewToils>c__Iterator3C <MakeNewToils>c__Iterator3C = new JobDriver_TakeBeerOutOfFermentingBarrel.<MakeNewToils>c__Iterator3C();
			<MakeNewToils>c__Iterator3C.<>f__this = this;
			JobDriver_TakeBeerOutOfFermentingBarrel.<MakeNewToils>c__Iterator3C expr_0E = <MakeNewToils>c__Iterator3C;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
