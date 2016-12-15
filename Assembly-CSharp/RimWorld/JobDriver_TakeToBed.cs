using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class JobDriver_TakeToBed : JobDriver
	{
		private const TargetIndex TakeeIndex = TargetIndex.A;

		private const TargetIndex BedIndex = TargetIndex.B;

		protected Pawn Takee
		{
			get
			{
				return (Pawn)base.CurJob.GetTarget(TargetIndex.A).Thing;
			}
		}

		protected Building_Bed DropBed
		{
			get
			{
				return (Building_Bed)base.CurJob.GetTarget(TargetIndex.B).Thing;
			}
		}

		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_TakeToBed.<MakeNewToils>c__Iterator3D <MakeNewToils>c__Iterator3D = new JobDriver_TakeToBed.<MakeNewToils>c__Iterator3D();
			<MakeNewToils>c__Iterator3D.<>f__this = this;
			JobDriver_TakeToBed.<MakeNewToils>c__Iterator3D expr_0E = <MakeNewToils>c__Iterator3D;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
