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
			StockGenerator_SingleDef.<GenerateThings>c__Iterator15C <GenerateThings>c__Iterator15C = new StockGenerator_SingleDef.<GenerateThings>c__Iterator15C();
			<GenerateThings>c__Iterator15C.<>f__this = this;
			StockGenerator_SingleDef.<GenerateThings>c__Iterator15C expr_0E = <GenerateThings>c__Iterator15C;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return thingDef == this.thingDef;
		}
	}
}
