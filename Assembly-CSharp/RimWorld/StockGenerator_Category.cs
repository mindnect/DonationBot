using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;

namespace RimWorld
{
	public class StockGenerator_Category : StockGenerator
	{
		private ThingCategoryDef categoryDef;

		private IntRange thingDefCountRange = IntRange.one;

		private List<ThingDef> excludedThingDefs;

		[DebuggerHidden]
		public override IEnumerable<Thing> GenerateThings(Map forMap)
		{
			StockGenerator_Category.<GenerateThings>c__Iterator15E <GenerateThings>c__Iterator15E = new StockGenerator_Category.<GenerateThings>c__Iterator15E();
			<GenerateThings>c__Iterator15E.<>f__this = this;
			StockGenerator_Category.<GenerateThings>c__Iterator15E expr_0E = <GenerateThings>c__Iterator15E;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return this.categoryDef.DescendantThingDefs.Contains(thingDef) && thingDef.techLevel <= this.maxTechLevelBuy && (this.excludedThingDefs == null || !this.excludedThingDefs.Contains(thingDef));
		}
	}
}
