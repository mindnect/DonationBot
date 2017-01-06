using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public class CompMannable : ThingComp
	{
		private int lastManTick = -1;

		private Pawn lastManPawn;

		public bool MannedNow
		{
			get
			{
				return Find.TickManager.TicksGame - this.lastManTick <= 1 && this.lastManPawn != null && this.lastManPawn.Spawned;
			}
		}

		public Pawn ManningPawn
		{
			get
			{
				if (!this.MannedNow)
				{
					return null;
				}
				return this.lastManPawn;
			}
		}

		public CompProperties_Mannable Props
		{
			get
			{
				return (CompProperties_Mannable)this.props;
			}
		}

		public void ManForATick(Pawn pawn)
		{
			this.lastManTick = Find.TickManager.TicksGame;
			this.lastManPawn = pawn;
			pawn.mindState.lastMannedThing = this.parent;
		}

		[DebuggerHidden]
		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn pawn)
		{
			CompMannable.<CompFloatMenuOptions>c__Iterator14D <CompFloatMenuOptions>c__Iterator14D = new CompMannable.<CompFloatMenuOptions>c__Iterator14D();
			<CompFloatMenuOptions>c__Iterator14D.pawn = pawn;
			<CompFloatMenuOptions>c__Iterator14D.<$>pawn = pawn;
			<CompFloatMenuOptions>c__Iterator14D.<>f__this = this;
			CompMannable.<CompFloatMenuOptions>c__Iterator14D expr_1C = <CompFloatMenuOptions>c__Iterator14D;
			expr_1C.$PC = -2;
			return expr_1C;
		}
	}
}
