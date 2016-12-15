using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;

namespace RimWorld
{
	internal class Building_ShipComputerCore : Building
	{
		private bool CanLaunchNow
		{
			get
			{
				return !ShipUtility.LaunchFailReasons(this).Any<string>();
			}
		}

		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			Building_ShipComputerCore.<GetGizmos>c__Iterator12E <GetGizmos>c__Iterator12E = new Building_ShipComputerCore.<GetGizmos>c__Iterator12E();
			<GetGizmos>c__Iterator12E.<>f__this = this;
			Building_ShipComputerCore.<GetGizmos>c__Iterator12E expr_0E = <GetGizmos>c__Iterator12E;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		private void TryLaunch()
		{
			if (this.CanLaunchNow)
			{
				ShipCountdown.InitiateCountdown(this, -1);
			}
		}
	}
}
