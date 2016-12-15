using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Verse;

namespace RimWorld
{
	public static class TraderStockGenerator
	{
		[DebuggerHidden]
		public static IEnumerable<Thing> GenerateTraderThings(TraderKindDef traderDef, Map forMap)
		{
			TraderStockGenerator.<GenerateTraderThings>c__Iterator163 <GenerateTraderThings>c__Iterator = new TraderStockGenerator.<GenerateTraderThings>c__Iterator163();
			<GenerateTraderThings>c__Iterator.traderDef = traderDef;
			<GenerateTraderThings>c__Iterator.forMap = forMap;
			<GenerateTraderThings>c__Iterator.<$>traderDef = traderDef;
			<GenerateTraderThings>c__Iterator.<$>forMap = forMap;
			TraderStockGenerator.<GenerateTraderThings>c__Iterator163 expr_23 = <GenerateTraderThings>c__Iterator;
			expr_23.$PC = -2;
			return expr_23;
		}

		internal static void LogGenerationData()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (TraderKindDef current in DefDatabase<TraderKindDef>.AllDefs)
			{
				stringBuilder.AppendLine("Generated stock for " + current.defName + ":");
				foreach (Thing current2 in TraderStockGenerator.GenerateTraderThings(current, null))
				{
					MinifiedThing minifiedThing = current2 as MinifiedThing;
					Thing thing;
					if (minifiedThing != null)
					{
						thing = minifiedThing.InnerThing;
					}
					else
					{
						thing = current2;
					}
					string text = thing.LabelCap;
					QualityCategory qualityCategory;
					if (thing.TryGetQuality(out qualityCategory))
					{
						text = text + " (" + qualityCategory.ToString() + ")";
					}
					stringBuilder.AppendLine(text);
				}
				stringBuilder.AppendLine();
			}
			Log.Message(stringBuilder.ToString());
		}
	}
}
