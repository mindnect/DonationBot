using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public class CompProperties_Drug : CompProperties
	{
		public ChemicalDef chemical;

		public float addictiveness;

		public float minToleranceToAddict;

		public float existingAddictionSeverityOffset = 0.1f;

		public float needLevelOffset = 1f;

		public FloatRange overdoseSeverityOffset = FloatRange.Zero;

		public float largeOverdoseChance;

		public bool isCombatEnhancingDrug;

		public float listOrder;

		public bool Addictive
		{
			get
			{
				return this.addictiveness > 0f;
			}
		}

		public bool CanCauseOverdose
		{
			get
			{
				return this.overdoseSeverityOffset.TrueMax > 0f;
			}
		}

		public CompProperties_Drug()
		{
			this.compClass = typeof(CompDrug);
		}

		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			CompProperties_Drug.<ConfigErrors>c__Iterator78 <ConfigErrors>c__Iterator = new CompProperties_Drug.<ConfigErrors>c__Iterator78();
			<ConfigErrors>c__Iterator.parentDef = parentDef;
			<ConfigErrors>c__Iterator.<$>parentDef = parentDef;
			<ConfigErrors>c__Iterator.<>f__this = this;
			CompProperties_Drug.<ConfigErrors>c__Iterator78 expr_1C = <ConfigErrors>c__Iterator;
			expr_1C.$PC = -2;
			return expr_1C;
		}

		[DebuggerHidden]
		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			CompProperties_Drug.<SpecialDisplayStats>c__Iterator79 <SpecialDisplayStats>c__Iterator = new CompProperties_Drug.<SpecialDisplayStats>c__Iterator79();
			<SpecialDisplayStats>c__Iterator.<>f__this = this;
			CompProperties_Drug.<SpecialDisplayStats>c__Iterator79 expr_0E = <SpecialDisplayStats>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
