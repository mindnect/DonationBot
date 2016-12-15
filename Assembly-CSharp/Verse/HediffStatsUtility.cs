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
			HediffStatsUtility.<SpecialDisplayStats>c__Iterator1AB <SpecialDisplayStats>c__Iterator1AB = new HediffStatsUtility.<SpecialDisplayStats>c__Iterator1AB();
			<SpecialDisplayStats>c__Iterator1AB.instance = instance;
			<SpecialDisplayStats>c__Iterator1AB.stage = stage;
			<SpecialDisplayStats>c__Iterator1AB.<$>instance = instance;
			<SpecialDisplayStats>c__Iterator1AB.<$>stage = stage;
			HediffStatsUtility.<SpecialDisplayStats>c__Iterator1AB expr_23 = <SpecialDisplayStats>c__Iterator1AB;
			expr_23.$PC = -2;
			return expr_23;
		}
	}
}
