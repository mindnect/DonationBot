using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public class Building_BlastingCharge : Building
	{
		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			Building_BlastingCharge.<GetGizmos>c__Iterator12B <GetGizmos>c__Iterator12B = new Building_BlastingCharge.<GetGizmos>c__Iterator12B();
			<GetGizmos>c__Iterator12B.<>f__this = this;
			Building_BlastingCharge.<GetGizmos>c__Iterator12B expr_0E = <GetGizmos>c__Iterator12B;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		private void Command_Detonate()
		{
			base.GetComp<CompExplosive>().StartWick(null);
		}
	}
}
