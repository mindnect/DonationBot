using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public class StockGenerator_BuyWeirdOrganic : StockGenerator
	{
		[DebuggerHidden]
		public override IEnumerable<Thing> GenerateThings(Map forMap)
		{
			StockGenerator_BuyWeirdOrganic.<GenerateThings>c__Iterator15B <GenerateThings>c__Iterator15B = new StockGenerator_BuyWeirdOrganic.<GenerateThings>c__Iterator15B();
			StockGenerator_BuyWeirdOrganic.<GenerateThings>c__Iterator15B expr_07 = <GenerateThings>c__Iterator15B;
			expr_07.$PC = -2;
			return expr_07;
		}

		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return thingDef == ThingDefOf.InsectJelly;
		}
	}
}
