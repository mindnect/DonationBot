using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public abstract class WorkGiver_Grower : WorkGiver_Scanner
	{
		protected static ThingDef wantedPlantDef;

		protected virtual bool ExtraRequirements(IPlantToGrowSettable settable, Pawn pawn)
		{
			return true;
		}

		[DebuggerHidden]
		public override IEnumerable<IntVec3> PotentialWorkCellsGlobal(Pawn pawn)
		{
			WorkGiver_Grower.<PotentialWorkCellsGlobal>c__Iterator5E <PotentialWorkCellsGlobal>c__Iterator5E = new WorkGiver_Grower.<PotentialWorkCellsGlobal>c__Iterator5E();
			<PotentialWorkCellsGlobal>c__Iterator5E.pawn = pawn;
			<PotentialWorkCellsGlobal>c__Iterator5E.<$>pawn = pawn;
			<PotentialWorkCellsGlobal>c__Iterator5E.<>f__this = this;
			WorkGiver_Grower.<PotentialWorkCellsGlobal>c__Iterator5E expr_1C = <PotentialWorkCellsGlobal>c__Iterator5E;
			expr_1C.$PC = -2;
			return expr_1C;
		}

		public static ThingDef CalculateWantedPlantDef(IntVec3 c, Map map)
		{
			IPlantToGrowSettable plantToGrowSettable = c.GetEdifice(map) as IPlantToGrowSettable;
			if (plantToGrowSettable == null)
			{
				plantToGrowSettable = (map.zoneManager.ZoneAt(c) as IPlantToGrowSettable);
			}
			if (plantToGrowSettable == null)
			{
				return null;
			}
			return plantToGrowSettable.GetPlantDefToGrow();
		}
	}
}
