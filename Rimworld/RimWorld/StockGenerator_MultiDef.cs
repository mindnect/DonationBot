using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public class StockGenerator_MultiDef : StockGenerator
	{
		private List<ThingDef> thingDefs = new List<ThingDef>();

		[DebuggerHidden]
		public override IEnumerable<Thing> GenerateThings(Map forMap)
		{
			StockGenerator_MultiDef.<GenerateThings>c__Iterator15E <GenerateThings>c__Iterator15E = new StockGenerator_MultiDef.<GenerateThings>c__Iterator15E();
			<GenerateThings>c__Iterator15E.<>f__this = this;
			StockGenerator_MultiDef.<GenerateThings>c__Iterator15E expr_0E = <GenerateThings>c__Iterator15E;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return this.thingDefs.Contains(thingDef);
		}
	}
}
