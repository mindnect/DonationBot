using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
	public class Trait : IExposable
	{
		public TraitDef def;

		private int degree;

		private bool scenForced;

		public int Degree
		{
			get
			{
				return this.degree;
			}
		}

		public TraitDegreeData CurrentData
		{
			get
			{
				return this.def.DataAtDegree(this.degree);
			}
		}

		public string Label
		{
			get
			{
				return this.CurrentData.label;
			}
		}

		public string LabelCap
		{
			get
			{
				return this.Label.CapitalizeFirst();
			}
		}

		public bool ScenForced
		{
			get
			{
				return this.scenForced;
			}
		}

		public Trait()
		{
		}

		public Trait(TraitDef def, int degree = 0, bool forced = false)
		{
			this.def = def;
			this.degree = degree;
			this.scenForced = forced;
		}

		public void ExposeData()
		{
			Scribe_Defs.LookDef<TraitDef>(ref this.def, "def");
			Scribe_Values.LookValue<int>(ref this.degree, "degree", 0, false);
			Scribe_Values.LookValue<bool>(ref this.scenForced, "scenForced", false, false);
			if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs && this.def == null)
			{
				this.def = DefDatabase<TraitDef>.GetRandom();
				this.degree = PawnGenerator.RandomTraitDegree(this.def);
			}
		}

		public float OffsetOfStat(StatDef stat)
		{
			float num = 0f;
			TraitDegreeData currentData = this.CurrentData;
			if (currentData.statOffsets != null)
			{
				for (int i = 0; i < currentData.statOffsets.Count; i++)
				{
					if (currentData.statOffsets[i].stat == stat)
					{
						num += currentData.statOffsets[i].value;
					}
				}
			}
			if (currentData.statFactors != null)
			{
				for (int j = 0; j < currentData.statFactors.Count; j++)
				{
					if (currentData.statFactors[j].stat == stat)
					{
						num *= currentData.statFactors[j].value;
					}
				}
			}
			return num;
		}

		public string TipString(Pawn pawn)
		{
			StringBuilder stringBuilder = new StringBuilder();
			TraitDegreeData currentData = this.CurrentData;
			stringBuilder.Append(currentData.description.AdjustedFor(pawn));
			int count = this.CurrentData.skillGains.Count;
			if (count > 0)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
			}
			int num = 0;
			foreach (KeyValuePair<SkillDef, int> current in this.CurrentData.skillGains)
			{
				if (current.Value != 0)
				{
					string value = "    " + current.Key.skillLabel + ":   " + current.Value.ToString("+##;-##");
					if (num < count - 1)
					{
						stringBuilder.AppendLine(value);
					}
					else
					{
						stringBuilder.Append(value);
					}
					num++;
				}
			}
			if (this.GetPermaThoughts().Any<ThoughtDef>())
			{
				stringBuilder.AppendLine();
				foreach (ThoughtDef current2 in this.GetPermaThoughts())
				{
					stringBuilder.AppendLine();
					stringBuilder.Append("    " + "PermanentMoodEffect".Translate() + " " + current2.stages[0].baseMoodEffect.ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Offset));
				}
			}
			if (currentData.statOffsets != null)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				for (int i = 0; i < currentData.statOffsets.Count; i++)
				{
					StatModifier statModifier = currentData.statOffsets[i];
					string toStringAsOffset = statModifier.ToStringAsOffset;
					string value2 = "    " + statModifier.stat.LabelCap + " " + toStringAsOffset;
					if (i < currentData.statOffsets.Count - 1)
					{
						stringBuilder.AppendLine(value2);
					}
					else
					{
						stringBuilder.Append(value2);
					}
				}
			}
			if (currentData.statFactors != null)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				for (int j = 0; j < currentData.statFactors.Count; j++)
				{
					StatModifier statModifier2 = currentData.statFactors[j];
					string toStringAsFactor = statModifier2.ToStringAsFactor;
					string value3 = "    " + statModifier2.stat.LabelCap + " " + toStringAsFactor;
					if (j < currentData.statFactors.Count - 1)
					{
						stringBuilder.AppendLine(value3);
					}
					else
					{
						stringBuilder.Append(value3);
					}
				}
			}
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Trait(",
				this.def.ToString(),
				"-",
				this.degree,
				")"
			});
		}

		[DebuggerHidden]
		private IEnumerable<ThoughtDef> GetPermaThoughts()
		{
			Trait.<GetPermaThoughts>c__IteratorE6 <GetPermaThoughts>c__IteratorE = new Trait.<GetPermaThoughts>c__IteratorE6();
			<GetPermaThoughts>c__IteratorE.<>f__this = this;
			Trait.<GetPermaThoughts>c__IteratorE6 expr_0E = <GetPermaThoughts>c__IteratorE;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		private bool AllowsWorkType(WorkTypeDef workDef)
		{
			return (this.def.disabledWorkTags & workDef.workTags) == WorkTags.None;
		}

		[DebuggerHidden]
		public IEnumerable<WorkTypeDef> GetDisabledWorkTypes()
		{
			Trait.<GetDisabledWorkTypes>c__IteratorE7 <GetDisabledWorkTypes>c__IteratorE = new Trait.<GetDisabledWorkTypes>c__IteratorE7();
			<GetDisabledWorkTypes>c__IteratorE.<>f__this = this;
			Trait.<GetDisabledWorkTypes>c__IteratorE7 expr_0E = <GetDisabledWorkTypes>c__IteratorE;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
