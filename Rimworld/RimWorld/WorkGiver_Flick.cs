using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class WorkGiver_Flick : WorkGiver_Scanner
	{
		[DebuggerHidden]
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			WorkGiver_Flick.<PotentialWorkThingsGlobal>c__Iterator65 <PotentialWorkThingsGlobal>c__Iterator = new WorkGiver_Flick.<PotentialWorkThingsGlobal>c__Iterator65();
			<PotentialWorkThingsGlobal>c__Iterator.pawn = pawn;
			<PotentialWorkThingsGlobal>c__Iterator.<$>pawn = pawn;
			WorkGiver_Flick.<PotentialWorkThingsGlobal>c__Iterator65 expr_15 = <PotentialWorkThingsGlobal>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t)
		{
			return pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Flick) != null && pawn.CanReserveAndReach(t, PathEndMode.Touch, pawn.NormalMaxDanger(), 1);
		}

		public override Job JobOnThing(Pawn pawn, Thing t)
		{
			return new Job(JobDefOf.Flick, t);
		}
	}
}
