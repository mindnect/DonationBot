using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	public class TraitSet : IExposable
	{
		protected Pawn pawn;

		public List<Trait> allTraits = new List<Trait>();

		public IEnumerable<MentalBreakDef> AllowedMentalBreaks
		{
			get
			{
				TraitSet.<>c__IteratorE8 <>c__IteratorE = new TraitSet.<>c__IteratorE8();
				<>c__IteratorE.<>f__this = this;
				TraitSet.<>c__IteratorE8 expr_0E = <>c__IteratorE;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}

		public TraitSet(Pawn pawn)
		{
			this.pawn = pawn;
		}

		public void ExposeData()
		{
			Scribe_Collections.LookList<Trait>(ref this.allTraits, "allTraits", LookMode.Deep, new object[0]);
		}

		public void GainTrait(Trait trait)
		{
			if (this.HasTrait(trait.def))
			{
				Log.Warning(this.pawn + " already has trait " + trait.def);
				return;
			}
			this.allTraits.Add(trait);
			if (this.pawn.workSettings != null)
			{
				this.pawn.workSettings.Notify_GainedTrait();
			}
			this.pawn.story.Notify_TraitChanged();
			if (this.pawn.skills != null)
			{
				this.pawn.skills.Notify_SkillDisablesChanged();
			}
			if (!this.pawn.Dead && this.pawn.RaceProps.Humanlike)
			{
				this.pawn.needs.mood.thoughts.situational.Notify_SituationalThoughtsDirty();
			}
		}

		public bool HasTrait(TraitDef tDef)
		{
			for (int i = 0; i < this.allTraits.Count; i++)
			{
				if (this.allTraits[i].def == tDef)
				{
					return true;
				}
			}
			return false;
		}

		public Trait GetTrait(TraitDef tDef)
		{
			for (int i = 0; i < this.allTraits.Count; i++)
			{
				if (this.allTraits[i].def == tDef)
				{
					return this.allTraits[i];
				}
			}
			return null;
		}

		public int DegreeOfTrait(TraitDef tDef)
		{
			for (int i = 0; i < this.allTraits.Count; i++)
			{
				if (this.allTraits[i].def == tDef)
				{
					return this.allTraits[i].Degree;
				}
			}
			return 0;
		}
	}
}
