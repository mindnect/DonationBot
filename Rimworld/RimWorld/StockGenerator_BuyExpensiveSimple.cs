using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public class StockGenerator_BuyExpensiveSimple : StockGenerator
	{
		public float minValuePerUnit = 15f;

		[DebuggerHidden]
		public override IEnumerable<Thing> GenerateThings(Map forMap)
		{
			StockGenerator_BuyExpensiveSimple.<GenerateThings>c__Iterator15C <GenerateThings>c__Iterator15C = new StockGenerator_BuyExpensiveSimple.<GenerateThings>c__Iterator15C();
			StockGenerator_BuyExpensiveSimple.<GenerateThings>c__Iterator15C expr_07 = <GenerateThings>c__Iterator15C;
			expr_07.$PC = -2;
			return expr_07;
		}

		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return thingDef.category == ThingCategory.Item && !thingDef.IsApparel && !thingDef.IsWeapon && !thingDef.IsMedicine && !thingDef.IsDrug && thingDef.BaseMarketValue / thingDef.VolumePerUnit > this.minValuePerUnit;
		}
	}
}
