using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public class StockGenerator_Slaves : StockGenerator
	{
		[DebuggerHidden]
		public override IEnumerable<Thing> GenerateThings(Map forMap)
		{
			StockGenerator_Slaves.<GenerateThings>c__Iterator160 <GenerateThings>c__Iterator = new StockGenerator_Slaves.<GenerateThings>c__Iterator160();
			<GenerateThings>c__Iterator.forMap = forMap;
			<GenerateThings>c__Iterator.<$>forMap = forMap;
			<GenerateThings>c__Iterator.<>f__this = this;
			StockGenerator_Slaves.<GenerateThings>c__Iterator160 expr_1C = <GenerateThings>c__Iterator;
			expr_1C.$PC = -2;
			return expr_1C;
		}

		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return thingDef.category == ThingCategory.Pawn && thingDef.race.Humanlike;
		}
	}
}
