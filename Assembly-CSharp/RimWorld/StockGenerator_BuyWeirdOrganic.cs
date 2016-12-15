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
			StockGenerator_BuyWeirdOrganic.<GenerateThings>c__Iterator15A <GenerateThings>c__Iterator15A = new StockGenerator_BuyWeirdOrganic.<GenerateThings>c__Iterator15A();
			StockGenerator_BuyWeirdOrganic.<GenerateThings>c__Iterator15A expr_07 = <GenerateThings>c__Iterator15A;
			expr_07.$PC = -2;
			return expr_07;
		}

		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return thingDef == ThingDefOf.InsectJelly;
		}
	}
}
