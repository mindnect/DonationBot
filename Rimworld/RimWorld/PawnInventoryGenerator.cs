using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorld
{
	public static class PawnInventoryGenerator
	{
		public static void GenerateInventoryFor(Pawn p, PawnGenerationRequest request)
		{
			p.inventory.DestroyAll(DestroyMode.Vanish);
			for (int i = 0; i < p.kindDef.fixedInventory.Count; i++)
			{
				ThingCountClass thingCountClass = p.kindDef.fixedInventory[i];
				Thing thing = ThingMaker.MakeThing(thingCountClass.thingDef, null);
				thing.stackCount = thingCountClass.count;
				p.inventory.innerContainer.TryAdd(thing, true);
			}
			if (p.kindDef.inventoryOptions != null)
			{
				foreach (Thing current in p.kindDef.inventoryOptions.GenerateThings())
				{
					p.inventory.innerContainer.TryAdd(current, true);
				}
			}
			if (request.AllowFood)
			{
				PawnInventoryGenerator.GiveRandomFood(p);
			}
			PawnInventoryGenerator.GiveDrugsIfAddicted(p);
			PawnInventoryGenerator.GiveCombatEnhancingDrugs(p);
		}

		public static void GiveRandomFood(Pawn p)
		{
			if (p.kindDef.invNutrition > 0.001f)
			{
				ThingDef thingDef;
				if (p.kindDef.invFoodDef != null)
				{
					thingDef = p.kindDef.invFoodDef;
				}
				else
				{
					float value = Rand.Value;
					if (value < 0.5f)
					{
						thingDef = ThingDefOf.MealSimple;
					}
					else if ((double)value < 0.75)
					{
						thingDef = ThingDefOf.MealFine;
					}
					else
					{
						thingDef = ThingDefOf.MealSurvivalPack;
					}
				}
				Thing thing = ThingMaker.MakeThing(thingDef, null);
				thing.stackCount = GenMath.RoundRandom(p.kindDef.invNutrition / thingDef.ingestible.nutrition);
				p.inventory.TryAddItemNotForSale(thing);
			}
		}

		private static void GiveDrugsIfAddicted(Pawn p)
		{
			PawnInventoryGenerator.<GiveDrugsIfAddicted>c__AnonStorey2D8 <GiveDrugsIfAddicted>c__AnonStorey2D = new PawnInventoryGenerator.<GiveDrugsIfAddicted>c__AnonStorey2D8();
			<GiveDrugsIfAddicted>c__AnonStorey2D.p = p;
			if (!<GiveDrugsIfAddicted>c__AnonStorey2D.p.RaceProps.Humanlike)
			{
				return;
			}
			IEnumerable<Hediff_Addiction> hediffs = <GiveDrugsIfAddicted>c__AnonStorey2D.p.health.hediffSet.GetHediffs<Hediff_Addiction>();
			foreach (Hediff_Addiction addiction in hediffs)
			{
				IEnumerable<ThingDef> source = DefDatabase<ThingDef>.AllDefsListForReading.Where(delegate(ThingDef x)
				{
					if (x.category != ThingCategory.Item)
					{
						return false;
					}
					if (<GiveDrugsIfAddicted>c__AnonStorey2D.p.Faction != null && x.techLevel > <GiveDrugsIfAddicted>c__AnonStorey2D.p.Faction.def.techLevel)
					{
						return false;
					}
					CompProperties_Drug compProperties = x.GetCompProperties<CompProperties_Drug>();
					return compProperties != null && compProperties.chemical != null && compProperties.chemical.addictionHediff == addiction.def;
				});
				ThingDef def;
				if (source.TryRandomElement(out def))
				{
					int stackCount = Rand.RangeInclusive(2, 5);
					Thing thing = ThingMaker.MakeThing(def, null);
					thing.stackCount = stackCount;
					<GiveDrugsIfAddicted>c__AnonStorey2D.p.inventory.TryAddItemNotForSale(thing);
				}
			}
		}

		private static void GiveCombatEnhancingDrugs(Pawn pawn)
		{
			if (Rand.Value >= pawn.kindDef.combatEnhancingDrugsChance)
			{
				return;
			}
			if (pawn.IsTeetotaler())
			{
				return;
			}
			for (int i = 0; i < pawn.inventory.innerContainer.Count; i++)
			{
				CompDrug compDrug = pawn.inventory.innerContainer[i].TryGetComp<CompDrug>();
				if (compDrug != null && compDrug.Props.isCombatEnhancingDrug)
				{
					return;
				}
			}
			int randomInRange = pawn.kindDef.combatEnhancingDrugsCount.RandomInRange;
			if (randomInRange <= 0)
			{
				return;
			}
			IEnumerable<ThingDef> source = DefDatabase<ThingDef>.AllDefsListForReading.Where(delegate(ThingDef x)
			{
				if (x.category != ThingCategory.Item)
				{
					return false;
				}
				if (pawn.Faction != null && x.techLevel > pawn.Faction.def.techLevel)
				{
					return false;
				}
				CompProperties_Drug compProperties = x.GetCompProperties<CompProperties_Drug>();
				return compProperties != null && compProperties.isCombatEnhancingDrug;
			});
			for (int j = 0; j < randomInRange; j++)
			{
				ThingDef def;
				if (!source.TryRandomElement(out def))
				{
					break;
				}
				pawn.inventory.innerContainer.TryAdd(ThingMaker.MakeThing(def, null), true);
			}
		}
	}
}
