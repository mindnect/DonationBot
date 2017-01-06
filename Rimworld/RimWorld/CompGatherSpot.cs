using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public class CompGatherSpot : ThingComp
	{
		private bool active = true;

		public bool Active
		{
			get
			{
				return this.active;
			}
			set
			{
				if (value == this.active)
				{
					return;
				}
				this.active = value;
				if (this.parent.Spawned)
				{
					if (this.active)
					{
						this.parent.Map.gatherSpotLister.RegisterActivated(this);
					}
					else
					{
						this.parent.Map.gatherSpotLister.RegisterDeactivated(this);
					}
				}
			}
		}

		public override void PostExposeData()
		{
			Scribe_Values.LookValue<bool>(ref this.active, "active", false, false);
		}

		public override void PostSpawnSetup()
		{
			base.PostSpawnSetup();
			if (this.Active)
			{
				this.parent.Map.gatherSpotLister.RegisterActivated(this);
			}
		}

		public override void PostDeSpawn(Map map)
		{
			base.PostDeSpawn(map);
			if (this.Active)
			{
				map.gatherSpotLister.RegisterDeactivated(this);
			}
		}

		[DebuggerHidden]
		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			CompGatherSpot.<CompGetGizmosExtra>c__Iterator14B <CompGetGizmosExtra>c__Iterator14B = new CompGatherSpot.<CompGetGizmosExtra>c__Iterator14B();
			<CompGetGizmosExtra>c__Iterator14B.<>f__this = this;
			CompGatherSpot.<CompGetGizmosExtra>c__Iterator14B expr_0E = <CompGetGizmosExtra>c__Iterator14B;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
