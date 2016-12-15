using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Verse
{
	public class PriorityWork : IExposable
	{
		private const int Timeout = 30000;

		private IntVec3 prioritizedCell = IntVec3.Invalid;

		private WorkTypeDef prioritizedWorkType;

		private int prioritizeTick = Find.TickManager.TicksGame;

		public bool IsPrioritized
		{
			get
			{
				if (this.prioritizedCell.IsValid)
				{
					if (Find.TickManager.TicksGame < this.prioritizeTick + 30000)
					{
						return true;
					}
					this.Clear();
				}
				return false;
			}
		}

		public IntVec3 Cell
		{
			get
			{
				return this.prioritizedCell;
			}
		}

		public WorkTypeDef WorkType
		{
			get
			{
				return this.prioritizedWorkType;
			}
		}

		public void ExposeData()
		{
			Scribe_Values.LookValue<IntVec3>(ref this.prioritizedCell, "prioritizedCell", default(IntVec3), false);
			Scribe_Defs.LookDef<WorkTypeDef>(ref this.prioritizedWorkType, "prioritizedWorkType");
			Scribe_Values.LookValue<int>(ref this.prioritizeTick, "prioritizeTick", 0, false);
		}

		public void Set(IntVec3 prioritizedCell, WorkTypeDef prioritizedWorkType)
		{
			this.prioritizedCell = prioritizedCell;
			this.prioritizedWorkType = prioritizedWorkType;
			this.prioritizeTick = Find.TickManager.TicksGame;
		}

		public void Clear()
		{
			this.prioritizedCell = IntVec3.Invalid;
			this.prioritizedWorkType = null;
			this.prioritizeTick = 0;
		}

		public void DrawExtraSelectionOverlays(Pawn pawn)
		{
			if (this.IsPrioritized)
			{
				GenDraw.DrawLineBetween(pawn.DrawPos, this.Cell.ToVector3Shifted());
			}
		}

		[DebuggerHidden]
		public IEnumerable<Gizmo> GetGizmos(Pawn pawn)
		{
			PriorityWork.<GetGizmos>c__Iterator19D <GetGizmos>c__Iterator19D = new PriorityWork.<GetGizmos>c__Iterator19D();
			<GetGizmos>c__Iterator19D.pawn = pawn;
			<GetGizmos>c__Iterator19D.<$>pawn = pawn;
			<GetGizmos>c__Iterator19D.<>f__this = this;
			PriorityWork.<GetGizmos>c__Iterator19D expr_1C = <GetGizmos>c__Iterator19D;
			expr_1C.$PC = -2;
			return expr_1C;
		}
	}
}
