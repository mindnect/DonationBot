using System;
using Verse;

namespace RimWorld
{
	public class StockGenerator_Clothes : StockGenerator_MiscItems
	{
		public override bool HandlesThingDef(ThingDef td)
		{
			return td != ThingDefOf.Apparel_PersonalShield && (base.HandlesThingDef(td) && td.IsApparel) && (td.GetStatValueAbstract(StatDefOf.ArmorRating_Blunt, null) < 0.15f || td.GetStatValueAbstract(StatDefOf.ArmorRating_Sharp, null) < 0.15f);
		}
	}
}
