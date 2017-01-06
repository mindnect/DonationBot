using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Verse
{
	public static class HediffStatsUtility
	{
		[DebuggerHidden]
		public static IEnumerable<StatDrawEntry> SpecialDisplayStats(HediffStage stage, Hediff instance)
		{
			HediffStatsUtility.<SpecialDisplayStats>c__Iterator1AC <SpecialDisplayStats>c__Iterator1AC = new HediffStatsUtility.<SpecialDisplayStats>c__Iterator1AC();
			<SpecialDisplayStats>c__Iterator1AC.instance = instance;
			<SpecialDisplayStats>c__Iterator1AC.stage = stage;
			<SpecialDisplayStats>c__Iterator1AC.<$>instance = instance;
			<SpecialDisplayStats>c__Iterator1AC.<$>stage = stage;
			HediffStatsUtility.<SpecialDisplayStats>c__Iterator1AC expr_23 = <SpecialDisplayStats>c__Iterator1AC;
			expr_23.$PC = -2;
			return expr_23;
		}
	}
}
