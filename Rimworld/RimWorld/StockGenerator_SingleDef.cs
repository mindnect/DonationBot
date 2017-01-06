using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public class StockGenerator_SingleDef : StockGenerator
	{
		private ThingDef thingDef;

		[DebuggerHidden]
		public override IEnumerable<Thing> GenerateThings(Map forMap)
		{
			StockGenerator_SingleDef.<GenerateThings>c__Iterator15D <GenerateThings>c__Iterator15D = new StockGenerator_SingleDef.<GenerateThings>c__Iterator15D();
			<GenerateThings>c__Iterator15D.<>f__this = this;
			StockGenerator_SingleDef.<GenerateThings>c__Iterator15D expr_0E = <GenerateThings>c__Iterator15D;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return thingDef == this.thingDef;
		}
	}
}
