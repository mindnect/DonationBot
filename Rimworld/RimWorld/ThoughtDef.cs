using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class ThoughtDef : Def
	{
		public Type thoughtClass;

		public Type workerClass;

		public List<ThoughtStage> stages = new List<ThoughtStage>();

		public int stackLimit = 1;

		public float stackedEffectMultiplier = 0.75f;

		public float durationDays;

		public bool invert;

		public bool validWhileDespawned;

		public ThoughtDef nextThought;

		public List<TraitDef> nullifyingTraits;

		public List<TaleDef> nullifyingOwnTales;

		public List<TraitDef> requiredTraits;

		public int requiredTraitsDegree = -2147483648;

		public StatDef effectMultiplyingStat;

		public HediffDef hediff;

		public bool nullifiedIfNotColonist;

		public ThoughtDef thoughtToMake;

		[NoTranslate]
		private string icon;

		public bool showBubble;

		public int stackLimitPerPawn = -1;

		public float lerpOpinionToZeroAfterDurationPct = 0.7f;

		public bool socialThoughtAffectingMood;

		public float maxCumulatedOpinionOffset = 3.40282347E+38f;

		public TaleDef taleDef;

		[Unsaved]
		private ThoughtWorker workerInt;

		private Texture2D iconInt;

		public string Label
		{
			get
			{
				if (!this.label.NullOrEmpty())
				{
					return this.label;
				}
				if (this.stages.NullOrEmpty<ThoughtStage>())
				{
					if (!this.stages[0].label.NullOrEmpty())
					{
						return this.stages[0].label;
					}
					if (!this.stages[0].labelSocial.NullOrEmpty())
					{
						return this.stages[0].labelSocial;
					}
				}
				Log.Error("Cannot get good label for ThoughtDef " + this.defName);
				return this.defName;
			}
		}

		public int DurationTicks
		{
			get
			{
				return (int)(this.durationDays * 60000f);
			}
		}

		public bool IsMemory
		{
			get
			{
				return this.durationDays > 0f || typeof(Thought_Memory).IsAssignableFrom(this.thoughtClass);
			}
		}

		public bool IsSituational
		{
			get
			{
				return this.Worker != null;
			}
		}

		public bool IsSocial
		{
			get
			{
				return typeof(ISocialThought).IsAssignableFrom(this.ThoughtClass);
			}
		}

		public bool RequiresSpecificTraitsDegree
		{
			get
			{
				return this.requiredTraitsDegree != -2147483648;
			}
		}

		public ThoughtWorker Worker
		{
			get
			{
				if (this.workerInt == null && this.workerClass != null)
				{
					this.workerInt = (ThoughtWorker)Activator.CreateInstance(this.workerClass);
					this.workerInt.def = this;
				}
				return this.workerInt;
			}
		}

		public Type ThoughtClass
		{
			get
			{
				if (this.thoughtClass != null)
				{
					return this.thoughtClass;
				}
				if (this.IsMemory)
				{
					return typeof(Thought_Memory);
				}
				return typeof(Thought_Situational);
			}
		}

		public Texture2D Icon
		{
			get
			{
				if (this.iconInt == null)
				{
					if (this.icon == null)
					{
						return null;
					}
					this.iconInt = ContentFinder<Texture2D>.Get(this.icon, true);
				}
				return this.iconInt;
			}
		}

		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			ThoughtDef.<ConfigErrors>c__Iterator94 <ConfigErrors>c__Iterator = new ThoughtDef.<ConfigErrors>c__Iterator94();
			<ConfigErrors>c__Iterator.<>f__this = this;
			ThoughtDef.<ConfigErrors>c__Iterator94 expr_0E = <ConfigErrors>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		public static ThoughtDef Named(string defName)
		{
			return DefDatabase<ThoughtDef>.GetNamed(defName, true);
		}
	}
}
