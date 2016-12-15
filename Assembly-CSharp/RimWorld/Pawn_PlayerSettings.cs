using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public class Pawn_PlayerSettings : IExposable
	{
		private Pawn pawn;

		private Area areaAllowedInt;

		public int joinTick = -1;

		public Pawn master;

		public bool followDrafted = true;

		public bool followFieldwork = true;

		public bool animalsReleased;

		public MedicalCareCategory medCare = MedicalCareCategory.NoMeds;

		public HostilityResponseMode hostilityResponse = HostilityResponseMode.Flee;

		public Area AreaRestrictionInPawnCurrentMap
		{
			get
			{
				if (this.areaAllowedInt != null && this.areaAllowedInt.Map != this.pawn.MapHeld)
				{
					return null;
				}
				return this.AreaRestriction;
			}
		}

		public Area AreaRestriction
		{
			get
			{
				if (!this.RespectsAllowedArea)
				{
					return null;
				}
				return this.areaAllowedInt;
			}
			set
			{
				this.areaAllowedInt = value;
			}
		}

		public bool RespectsAllowedArea
		{
			get
			{
				return this.pawn.Faction == Faction.OfPlayer && this.pawn.HostFaction == null;
			}
		}

		public bool UsesConfigurableHostilityResponse
		{
			get
			{
				return this.pawn.IsColonist && this.pawn.HostFaction == null;
			}
		}

		public Pawn_PlayerSettings(Pawn pawn)
		{
			this.pawn = pawn;
			if (Current.ProgramState == ProgramState.Playing)
			{
				this.joinTick = Find.TickManager.TicksGame;
			}
			else
			{
				this.joinTick = 0;
			}
			this.Notify_FactionChanged();
		}

		public void ExposeData()
		{
			Scribe_Values.LookValue<int>(ref this.joinTick, "joinTick", 0, false);
			Scribe_Values.LookValue<bool>(ref this.animalsReleased, "animalsReleased", false, false);
			Scribe_Values.LookValue<MedicalCareCategory>(ref this.medCare, "medCare", MedicalCareCategory.NoCare, false);
			Scribe_References.LookReference<Area>(ref this.areaAllowedInt, "areaAllowed", false);
			Scribe_References.LookReference<Pawn>(ref this.master, "master", false);
			Scribe_Values.LookValue<bool>(ref this.followDrafted, "followDrafted", false, false);
			Scribe_Values.LookValue<bool>(ref this.followFieldwork, "followFieldwork", false, false);
			Scribe_Values.LookValue<HostilityResponseMode>(ref this.hostilityResponse, "hostilityResponse", HostilityResponseMode.Flee, false);
		}

		[DebuggerHidden]
		public IEnumerable<Gizmo> GetGizmos()
		{
			Pawn_PlayerSettings.<GetGizmos>c__IteratorE1 <GetGizmos>c__IteratorE = new Pawn_PlayerSettings.<GetGizmos>c__IteratorE1();
			<GetGizmos>c__IteratorE.<>f__this = this;
			Pawn_PlayerSettings.<GetGizmos>c__IteratorE1 expr_0E = <GetGizmos>c__IteratorE;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		public void Notify_FactionChanged()
		{
			this.medCare = MedicalCareCategory.HerbalOrWorse;
			if (this.pawn.IsColonist && !this.pawn.IsPrisoner && !this.pawn.RaceProps.Animal)
			{
				this.medCare = MedicalCareCategory.Best;
			}
		}

		public void Notify_MadePrisoner()
		{
			this.medCare = MedicalCareCategory.HerbalOrWorse;
		}

		public void Notify_AreaRemoved(Area area)
		{
			if (this.areaAllowedInt == area)
			{
				this.areaAllowedInt = null;
			}
		}
	}
}
