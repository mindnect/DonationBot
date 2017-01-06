using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Verse
{
	public class HediffSet : IExposable
	{
		public Pawn pawn;

		public List<Hediff> hediffs = new List<Hediff>();

		private List<Hediff_MissingPart> cachedMissingPartsCommonAncestors;

		private float cachedPain = -1f;

		private float cachedBleedRate = -1f;

		private Stack<BodyPartRecord> coveragePartsStack = new Stack<BodyPartRecord>();

		private HashSet<BodyPartRecord> coverageRejectedPartsSet = new HashSet<BodyPartRecord>();

		private Queue<BodyPartRecord> missingPartsCommonAncestorsQueue = new Queue<BodyPartRecord>();

		public float PainTotal
		{
			get
			{
				if (this.cachedPain < 0f)
				{
					this.cachedPain = this.CalculatePain();
				}
				return this.cachedPain;
			}
		}

		public float BleedRateTotal
		{
			get
			{
				if (this.cachedBleedRate < 0f)
				{
					this.cachedBleedRate = this.CalculateBleedRate();
				}
				return this.cachedBleedRate;
			}
		}

		public bool AnyHediffMakesSickThought
		{
			get
			{
				for (int i = 0; i < this.hediffs.Count; i++)
				{
					if (this.hediffs[i].def.makesSickThought)
					{
						if (this.hediffs[i].Visible)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		public HediffSet(Pawn newPawn)
		{
			this.pawn = newPawn;
		}

		public void ExposeData()
		{
			Scribe_Collections.LookList<Hediff>(ref this.hediffs, "hediffs", LookMode.Deep, new object[0]);
			if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
			{
				for (int i = 0; i < this.hediffs.Count; i++)
				{
					this.hediffs[i].pawn = this.pawn;
				}
				this.DirtyCache();
			}
		}

		public void AddDirect(Hediff hediff, DamageInfo? dinfo = null)
		{
			if (hediff.def == null)
			{
				Log.Error("Tried to add health diff with null def. Canceling.");
				return;
			}
			if (hediff.Part != null && !this.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined).Contains(hediff.Part))
			{
				Log.Error("Tried to add health diff to missing part " + hediff.Part);
				return;
			}
			ProfilerThreadCheck.BeginSample("HediffSet.AddHediff()");
			hediff.ageTicks = 0;
			hediff.pawn = this.pawn;
			ProfilerThreadCheck.BeginSample("Attempt merge or add new hediff");
			bool flag = false;
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (this.hediffs[i].TryMergeWith(hediff))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				this.hediffs.Add(hediff);
				hediff.PostAdd(dinfo);
				if (this.pawn.needs != null && this.pawn.needs.mood != null)
				{
					this.pawn.needs.mood.thoughts.situational.Notify_SituationalThoughtsDirty();
				}
			}
			ProfilerThreadCheck.EndSample();
			bool flag2 = hediff is Hediff_MissingPart;
			if (!(hediff is Hediff_MissingPart) && hediff.Part != null && hediff.Part != this.pawn.RaceProps.body.corePart && this.GetPartHealth(hediff.Part) == 0f)
			{
				ProfilerThreadCheck.BeginSample("Handle missing body part");
				if (hediff.Part != this.pawn.RaceProps.body.corePart)
				{
					bool flag3 = this.HasDirectlyAddedPartFor(hediff.Part);
					Hediff_MissingPart hediff_MissingPart = (Hediff_MissingPart)HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, this.pawn, null);
					hediff_MissingPart.IsFresh = !flag3;
					hediff_MissingPart.lastInjury = hediff.def;
					this.pawn.health.AddHediff(hediff_MissingPart, hediff.Part, dinfo);
					if (flag3)
					{
						if (dinfo.HasValue)
						{
							hediff_MissingPart.lastInjury = HealthUtility.GetHediffDefFromDamage(dinfo.Value.Def, this.pawn, hediff.Part);
						}
						else
						{
							hediff_MissingPart.lastInjury = null;
						}
					}
					flag2 = true;
				}
				ProfilerThreadCheck.EndSample();
			}
			ProfilerThreadCheck.BeginSample("Dirty cache");
			this.DirtyCache();
			ProfilerThreadCheck.EndSample();
			if (flag2 && this.pawn.apparel != null)
			{
				this.pawn.apparel.Notify_LostBodyPart();
			}
			if (this.pawn.meleeVerbs != null)
			{
				this.pawn.meleeVerbs.Notify_HediffAddedOrRemoved();
			}
			if (hediff.def.causesNeed != null && !this.pawn.Dead)
			{
				this.pawn.needs.AddOrRemoveNeedsAsAppropriate();
			}
			ProfilerThreadCheck.EndSample();
		}

		public void DirtyCache()
		{
			this.CacheMissingPartsCommonAncestors();
			this.cachedPain = -1f;
			this.cachedBleedRate = -1f;
			this.pawn.health.capacities.Notify_CapacityEfficienciesDirty();
			this.pawn.health.summaryHealth.Notify_HealthChanged();
		}

		[DebuggerHidden]
		public IEnumerable<T> GetHediffs<T>() where T : Hediff
		{
			HediffSet.<GetHediffs>c__Iterator1F0<T> <GetHediffs>c__Iterator1F = new HediffSet.<GetHediffs>c__Iterator1F0<T>();
			<GetHediffs>c__Iterator1F.<>f__this = this;
			HediffSet.<GetHediffs>c__Iterator1F0<T> expr_0E = <GetHediffs>c__Iterator1F;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		public Hediff GetFirstHediffOfDef(HediffDef def)
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (this.hediffs[i].def == def)
				{
					return this.hediffs[i];
				}
			}
			return null;
		}

		public bool PartIsMissing(BodyPartRecord part)
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (this.hediffs[i].Part == part && this.hediffs[i] is Hediff_MissingPart)
				{
					return true;
				}
			}
			return false;
		}

		public float GetPartHealth(BodyPartRecord part)
		{
			if (part == null)
			{
				return 0f;
			}
			float num = part.def.GetMaxHealth(this.pawn);
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (this.hediffs[i] is Hediff_MissingPart && this.hediffs[i].Part == part)
				{
					return 0f;
				}
				if (this.hediffs[i].Part == part)
				{
					Hediff_Injury hediff_Injury = this.hediffs[i] as Hediff_Injury;
					if (hediff_Injury != null)
					{
						num -= (float)Mathf.RoundToInt(hediff_Injury.Severity);
					}
				}
			}
			if (num < 0f)
			{
				num = 0f;
			}
			return num;
		}

		public BodyPartRecord GetBrain()
		{
			foreach (BodyPartRecord current in this.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined))
			{
				for (int i = 0; i < current.def.Activities.Count; i++)
				{
					if (current.def.Activities[i].First == PawnCapacityDefOf.Consciousness)
					{
						return current;
					}
				}
			}
			return null;
		}

		public bool HasHediff(HediffDef def)
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (this.hediffs[i].def == def)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasHediff(HediffDef def, BodyPartRecord bodyPart)
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (this.hediffs[i].def == def && this.hediffs[i].Part == bodyPart)
				{
					return true;
				}
			}
			return false;
		}

		[DebuggerHidden]
		public IEnumerable<Verb> GetHediffsVerbs()
		{
			HediffSet.<GetHediffsVerbs>c__Iterator1F1 <GetHediffsVerbs>c__Iterator1F = new HediffSet.<GetHediffsVerbs>c__Iterator1F1();
			<GetHediffsVerbs>c__Iterator1F.<>f__this = this;
			HediffSet.<GetHediffsVerbs>c__Iterator1F1 expr_0E = <GetHediffsVerbs>c__Iterator1F;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		[DebuggerHidden]
		public IEnumerable<Hediff_Injury> GetInjuriesTendable()
		{
			HediffSet.<GetInjuriesTendable>c__Iterator1F2 <GetInjuriesTendable>c__Iterator1F = new HediffSet.<GetInjuriesTendable>c__Iterator1F2();
			<GetInjuriesTendable>c__Iterator1F.<>f__this = this;
			HediffSet.<GetInjuriesTendable>c__Iterator1F2 expr_0E = <GetInjuriesTendable>c__Iterator1F;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		public bool HasTendableInjury()
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				Hediff_Injury hediff_Injury = this.hediffs[i] as Hediff_Injury;
				if (hediff_Injury != null && hediff_Injury.TendableNow)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasNaturallyHealingInjury()
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				Hediff_Injury hediff_Injury = this.hediffs[i] as Hediff_Injury;
				if (hediff_Injury != null && hediff_Injury.CanHealNaturally())
				{
					return true;
				}
			}
			return false;
		}

		public bool HasTendedAndHealingInjury()
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				Hediff_Injury hediff_Injury = this.hediffs[i] as Hediff_Injury;
				if (hediff_Injury != null && hediff_Injury.CanHealFromTending() && hediff_Injury.Severity > 0f)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasTemperatureInjury(TemperatureInjuryStage minStage)
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if ((this.hediffs[i].def == HediffDefOf.Hypothermia || this.hediffs[i].def == HediffDefOf.Heatstroke) && this.hediffs[i].CurStageIndex >= (int)minStage)
				{
					return true;
				}
			}
			return false;
		}

		public IEnumerable<BodyPartRecord> GetInjuredParts()
		{
			return (from x in this.hediffs
			where x is Hediff_Injury
			select x.Part).Distinct<BodyPartRecord>();
		}

		[DebuggerHidden]
		public IEnumerable<BodyPartRecord> GetNaturallyHealingInjuredParts()
		{
			HediffSet.<GetNaturallyHealingInjuredParts>c__Iterator1F3 <GetNaturallyHealingInjuredParts>c__Iterator1F = new HediffSet.<GetNaturallyHealingInjuredParts>c__Iterator1F3();
			<GetNaturallyHealingInjuredParts>c__Iterator1F.<>f__this = this;
			HediffSet.<GetNaturallyHealingInjuredParts>c__Iterator1F3 expr_0E = <GetNaturallyHealingInjuredParts>c__Iterator1F;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		public List<Hediff_MissingPart> GetMissingPartsCommonAncestors()
		{
			if (this.cachedMissingPartsCommonAncestors == null)
			{
				this.CacheMissingPartsCommonAncestors();
			}
			return this.cachedMissingPartsCommonAncestors;
		}

		[DebuggerHidden]
		public IEnumerable<BodyPartRecord> GetNotMissingParts(BodyPartHeight height = BodyPartHeight.Undefined, BodyPartDepth depth = BodyPartDepth.Undefined)
		{
			HediffSet.<GetNotMissingParts>c__Iterator1F4 <GetNotMissingParts>c__Iterator1F = new HediffSet.<GetNotMissingParts>c__Iterator1F4();
			<GetNotMissingParts>c__Iterator1F.height = height;
			<GetNotMissingParts>c__Iterator1F.depth = depth;
			<GetNotMissingParts>c__Iterator1F.<$>height = height;
			<GetNotMissingParts>c__Iterator1F.<$>depth = depth;
			<GetNotMissingParts>c__Iterator1F.<>f__this = this;
			HediffSet.<GetNotMissingParts>c__Iterator1F4 expr_2A = <GetNotMissingParts>c__Iterator1F;
			expr_2A.$PC = -2;
			return expr_2A;
		}

		public BodyPartRecord GetRandomNotMissingPart(DamageDef damDef, BodyPartHeight height = BodyPartHeight.Undefined, BodyPartDepth depth = BodyPartDepth.Undefined)
		{
			IEnumerable<BodyPartRecord> notMissingParts;
			if (this.GetNotMissingParts(height, depth).Any<BodyPartRecord>())
			{
				notMissingParts = this.GetNotMissingParts(height, depth);
			}
			else
			{
				if (!this.GetNotMissingParts(BodyPartHeight.Undefined, depth).Any<BodyPartRecord>())
				{
					return null;
				}
				notMissingParts = this.GetNotMissingParts(BodyPartHeight.Undefined, depth);
			}
			BodyPartRecord result;
			if (notMissingParts.TryRandomElementByWeight((BodyPartRecord x) => x.absoluteFleshCoverage * x.def.GetHitChanceFactorFor(damDef), out result))
			{
				return result;
			}
			if (notMissingParts.TryRandomElementByWeight((BodyPartRecord x) => x.absoluteFleshCoverage, out result))
			{
				return result;
			}
			throw new InvalidOperationException();
		}

		public bool HasFreshMissingPartsCommonAncestor()
		{
			if (this.cachedMissingPartsCommonAncestors == null)
			{
				this.CacheMissingPartsCommonAncestors();
			}
			for (int i = 0; i < this.cachedMissingPartsCommonAncestors.Count; i++)
			{
				if (this.cachedMissingPartsCommonAncestors[i].IsFresh)
				{
					return true;
				}
			}
			return false;
		}

		public float GetCoverageOfNotMissingNaturalParts(BodyPartRecord part)
		{
			if (this.PartIsMissing(part))
			{
				return 0f;
			}
			if (this.PartOrAnyAncestorHasDirectlyAddedParts(part))
			{
				return 0f;
			}
			this.coverageRejectedPartsSet.Clear();
			List<Hediff_MissingPart> missingPartsCommonAncestors = this.GetMissingPartsCommonAncestors();
			for (int i = 0; i < missingPartsCommonAncestors.Count; i++)
			{
				this.coverageRejectedPartsSet.Add(missingPartsCommonAncestors[i].Part);
			}
			for (int j = 0; j < this.hediffs.Count; j++)
			{
				if (this.hediffs[j] is Hediff_AddedPart)
				{
					this.coverageRejectedPartsSet.Add(this.hediffs[j].Part);
				}
			}
			float num = 0f;
			this.coveragePartsStack.Clear();
			this.coveragePartsStack.Push(part);
			while (this.coveragePartsStack.Any<BodyPartRecord>())
			{
				BodyPartRecord bodyPartRecord = this.coveragePartsStack.Pop();
				num += bodyPartRecord.absoluteFleshCoverage;
				for (int k = 0; k < bodyPartRecord.parts.Count; k++)
				{
					if (!this.coverageRejectedPartsSet.Contains(bodyPartRecord.parts[k]))
					{
						this.coveragePartsStack.Push(bodyPartRecord.parts[k]);
					}
				}
			}
			this.coveragePartsStack.Clear();
			this.coverageRejectedPartsSet.Clear();
			return num;
		}

		private void CacheMissingPartsCommonAncestors()
		{
			if (this.cachedMissingPartsCommonAncestors == null)
			{
				this.cachedMissingPartsCommonAncestors = new List<Hediff_MissingPart>();
			}
			else
			{
				this.cachedMissingPartsCommonAncestors.Clear();
			}
			this.missingPartsCommonAncestorsQueue.Clear();
			this.missingPartsCommonAncestorsQueue.Enqueue(this.pawn.def.race.body.corePart);
			while (this.missingPartsCommonAncestorsQueue.Count != 0)
			{
				BodyPartRecord node = this.missingPartsCommonAncestorsQueue.Dequeue();
				if (!this.PartOrAnyAncestorHasDirectlyAddedParts(node))
				{
					Hediff_MissingPart hediff_MissingPart = (from x in this.GetHediffs<Hediff_MissingPart>()
					where x.Part == node
					select x).FirstOrDefault<Hediff_MissingPart>();
					if (hediff_MissingPart != null)
					{
						this.cachedMissingPartsCommonAncestors.Add(hediff_MissingPart);
					}
					else
					{
						for (int i = 0; i < node.parts.Count; i++)
						{
							this.missingPartsCommonAncestorsQueue.Enqueue(node.parts[i]);
						}
					}
				}
			}
		}

		public bool HasDirectlyAddedPartFor(BodyPartRecord part)
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (this.hediffs[i].Part == part && this.hediffs[i] is Hediff_AddedPart)
				{
					return true;
				}
			}
			return false;
		}

		public bool PartOrAnyAncestorHasDirectlyAddedParts(BodyPartRecord part)
		{
			return this.HasDirectlyAddedPartFor(part) || (part.parent != null && this.PartOrAnyAncestorHasDirectlyAddedParts(part.parent));
		}

		[DebuggerHidden]
		public IEnumerable<Hediff> GetTendableNonInjuryNonMissingPartHediffs()
		{
			HediffSet.<GetTendableNonInjuryNonMissingPartHediffs>c__Iterator1F5 <GetTendableNonInjuryNonMissingPartHediffs>c__Iterator1F = new HediffSet.<GetTendableNonInjuryNonMissingPartHediffs>c__Iterator1F5();
			<GetTendableNonInjuryNonMissingPartHediffs>c__Iterator1F.<>f__this = this;
			HediffSet.<GetTendableNonInjuryNonMissingPartHediffs>c__Iterator1F5 expr_0E = <GetTendableNonInjuryNonMissingPartHediffs>c__Iterator1F;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		public bool HasTendableNonInjuryNonMissingPartHediff(bool forAlert = false)
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (!forAlert || this.hediffs[i].def.makesAlert)
				{
					if (!(this.hediffs[i] is Hediff_Injury))
					{
						if (!(this.hediffs[i] is Hediff_MissingPart))
						{
							if (this.hediffs[i].TendableNow)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		private float CalculateBleedRate()
		{
			if (!this.pawn.RaceProps.IsFlesh || this.pawn.health.Dead)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				num += this.hediffs[i].BleedRate;
			}
			return num / this.pawn.HealthScale;
		}

		private float CalculatePain()
		{
			if (!this.pawn.RaceProps.IsFlesh || this.pawn.Dead)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				num += this.hediffs[i].PainOffset;
			}
			float num2 = num / this.pawn.HealthScale;
			for (int j = 0; j < this.hediffs.Count; j++)
			{
				num2 *= this.hediffs[j].PainFactor;
			}
			return Mathf.Clamp(num2, 0f, 1f);
		}

		public void Clear()
		{
			this.hediffs.Clear();
			this.DirtyCache();
		}
	}
}
