using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class WorkGiver_HunterHunt : WorkGiver_Scanner
	{
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.OnCell;
			}
		}

		[DebuggerHidden]
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			WorkGiver_HunterHunt.<PotentialWorkThingsGlobal>c__Iterator5F <PotentialWorkThingsGlobal>c__Iterator5F = new WorkGiver_HunterHunt.<PotentialWorkThingsGlobal>c__Iterator5F();
			<PotentialWorkThingsGlobal>c__Iterator5F.pawn = pawn;
			<PotentialWorkThingsGlobal>c__Iterator5F.<$>pawn = pawn;
			WorkGiver_HunterHunt.<PotentialWorkThingsGlobal>c__Iterator5F expr_15 = <PotentialWorkThingsGlobal>c__Iterator5F;
			expr_15.$PC = -2;
			return expr_15;
		}

		public override bool ShouldSkip(Pawn pawn)
		{
			return !WorkGiver_HunterHunt.HasHuntingWeapon(pawn) || WorkGiver_HunterHunt.HasShieldAndRangedWeapon(pawn);
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t)
		{
			Pawn pawn2 = t as Pawn;
			return pawn2 != null && pawn2.RaceProps.Animal && pawn.CanReserve(t, 1) && pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Hunt) != null;
		}

		public override Job JobOnThing(Pawn pawn, Thing t)
		{
			return new Job(JobDefOf.Hunt, t);
		}

		public static bool HasHuntingWeapon(Pawn p)
		{
			return p.equipment.Primary != null && p.equipment.Primary.def.IsRangedWeapon;
		}

		public static bool HasShieldAndRangedWeapon(Pawn p)
		{
			if (p.equipment.Primary != null && !p.equipment.Primary.def.Verbs[0].MeleeRange)
			{
				List<Apparel> wornApparel = p.apparel.WornApparel;
				for (int i = 0; i < wornApparel.Count; i++)
				{
					if (wornApparel[i] is PersonalShield)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
