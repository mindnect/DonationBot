using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld
{
	public class Pawn_DraftController : IExposable
	{
		public Pawn pawn;

		private bool draftedInt;

		private bool allowFiringInt = true;

		private AutoUndrafter autoUndrafter;

		public bool Drafted
		{
			get
			{
				return this.draftedInt;
			}
			set
			{
				if (value == this.draftedInt)
				{
					return;
				}
				this.pawn.mindState.priorityWork.Clear();
				this.allowFiringInt = true;
				this.draftedInt = value;
				if (!value && this.pawn.Spawned)
				{
					this.pawn.Map.pawnDestinationManager.UnreserveAllFor(this.pawn);
				}
				if (this.pawn.jobs.curJob != null && this.pawn.jobs.CanTakeOrderedJob())
				{
					this.pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, true);
				}
				if (this.draftedInt)
				{
					foreach (Pawn current in PawnUtility.SpawnedMasteredPawns(this.pawn))
					{
						current.jobs.Notify_MasterDrafted();
					}
					Lord lord = this.pawn.GetLord();
					if (lord != null && lord.LordJob is LordJob_VoluntarilyJoinable)
					{
						lord.Notify_PawnLost(this.pawn, PawnLostCondition.Drafted);
					}
				}
				else if (this.pawn.playerSettings != null)
				{
					this.pawn.playerSettings.animalsReleased = false;
				}
			}
		}

		public bool AllowFiring
		{
			get
			{
				return this.allowFiringInt;
			}
			set
			{
				this.allowFiringInt = value;
				if (!this.allowFiringInt && this.pawn.stances.curStance is Stance_Warmup)
				{
					this.pawn.stances.CancelBusyStanceSoft();
				}
			}
		}

		public Pawn_DraftController(Pawn pawn)
		{
			this.pawn = pawn;
			this.autoUndrafter = new AutoUndrafter(pawn);
		}

		public void ExposeData()
		{
			Scribe_Values.LookValue<bool>(ref this.draftedInt, "drafted", false, false);
			Scribe_Values.LookValue<bool>(ref this.allowFiringInt, "allowFiring", true, false);
			Scribe_Deep.LookDeep<AutoUndrafter>(ref this.autoUndrafter, "autoUndrafter", new object[]
			{
				this.pawn
			});
		}

		public void DraftControllerTick()
		{
			this.autoUndrafter.AutoUndraftTick();
		}

		[DebuggerHidden]
		internal IEnumerable<Gizmo> GetGizmos()
		{
			Pawn_DraftController.<GetGizmos>c__IteratorDC <GetGizmos>c__IteratorDC = new Pawn_DraftController.<GetGizmos>c__IteratorDC();
			<GetGizmos>c__IteratorDC.<>f__this = this;
			Pawn_DraftController.<GetGizmos>c__IteratorDC expr_0E = <GetGizmos>c__IteratorDC;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		internal void Notify_PrimaryWeaponChanged()
		{
			this.allowFiringInt = true;
		}
	}
}
