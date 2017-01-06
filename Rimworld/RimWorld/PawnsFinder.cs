using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public static class PawnsFinder
	{
		public static IEnumerable<Pawn> AllMapsAndWorld_AliveOrDead
		{
			get
			{
				PawnsFinder.<>c__IteratorC6 <>c__IteratorC = new PawnsFinder.<>c__IteratorC6();
				PawnsFinder.<>c__IteratorC6 expr_07 = <>c__IteratorC;
				expr_07.$PC = -2;
				return expr_07;
			}
		}

		public static IEnumerable<Pawn> AllMapsAndWorld_Alive
		{
			get
			{
				PawnsFinder.<>c__IteratorC7 <>c__IteratorC = new PawnsFinder.<>c__IteratorC7();
				PawnsFinder.<>c__IteratorC7 expr_07 = <>c__IteratorC;
				expr_07.$PC = -2;
				return expr_07;
			}
		}

		public static IEnumerable<Pawn> AllMaps
		{
			get
			{
				PawnsFinder.<>c__IteratorC8 <>c__IteratorC = new PawnsFinder.<>c__IteratorC8();
				PawnsFinder.<>c__IteratorC8 expr_07 = <>c__IteratorC;
				expr_07.$PC = -2;
				return expr_07;
			}
		}

		public static IEnumerable<Pawn> AllMaps_Spawned
		{
			get
			{
				PawnsFinder.<>c__IteratorC9 <>c__IteratorC = new PawnsFinder.<>c__IteratorC9();
				PawnsFinder.<>c__IteratorC9 expr_07 = <>c__IteratorC;
				expr_07.$PC = -2;
				return expr_07;
			}
		}

		public static IEnumerable<Pawn> AllMapsCaravansAndTravelingTransportPods
		{
			get
			{
				PawnsFinder.<>c__IteratorCA <>c__IteratorCA = new PawnsFinder.<>c__IteratorCA();
				PawnsFinder.<>c__IteratorCA expr_07 = <>c__IteratorCA;
				expr_07.$PC = -2;
				return expr_07;
			}
		}

		public static IEnumerable<Pawn> AllMapsCaravansAndTravelingTransportPods_Colonists
		{
			get
			{
				PawnsFinder.<>c__IteratorCB <>c__IteratorCB = new PawnsFinder.<>c__IteratorCB();
				PawnsFinder.<>c__IteratorCB expr_07 = <>c__IteratorCB;
				expr_07.$PC = -2;
				return expr_07;
			}
		}

		public static IEnumerable<Pawn> AllMapsCaravansAndTravelingTransportPods_FreeColonists
		{
			get
			{
				PawnsFinder.<>c__IteratorCC <>c__IteratorCC = new PawnsFinder.<>c__IteratorCC();
				PawnsFinder.<>c__IteratorCC expr_07 = <>c__IteratorCC;
				expr_07.$PC = -2;
				return expr_07;
			}
		}

		public static IEnumerable<Pawn> AllMapsCaravansAndTravelingTransportPods_PrisonersOfColony
		{
			get
			{
				PawnsFinder.<>c__IteratorCD <>c__IteratorCD = new PawnsFinder.<>c__IteratorCD();
				PawnsFinder.<>c__IteratorCD expr_07 = <>c__IteratorCD;
				expr_07.$PC = -2;
				return expr_07;
			}
		}

		public static IEnumerable<Pawn> AllMaps_PrisonersOfColonySpawned
		{
			get
			{
				PawnsFinder.<>c__IteratorCE <>c__IteratorCE = new PawnsFinder.<>c__IteratorCE();
				PawnsFinder.<>c__IteratorCE expr_07 = <>c__IteratorCE;
				expr_07.$PC = -2;
				return expr_07;
			}
		}

		public static IEnumerable<Pawn> AllMaps_PrisonersOfColony
		{
			get
			{
				PawnsFinder.<>c__IteratorCF <>c__IteratorCF = new PawnsFinder.<>c__IteratorCF();
				PawnsFinder.<>c__IteratorCF expr_07 = <>c__IteratorCF;
				expr_07.$PC = -2;
				return expr_07;
			}
		}

		public static IEnumerable<Pawn> AllMaps_FreeColonists
		{
			get
			{
				PawnsFinder.<>c__IteratorD0 <>c__IteratorD = new PawnsFinder.<>c__IteratorD0();
				PawnsFinder.<>c__IteratorD0 expr_07 = <>c__IteratorD;
				expr_07.$PC = -2;
				return expr_07;
			}
		}

		public static IEnumerable<Pawn> AllMaps_FreeColonistsSpawned
		{
			get
			{
				PawnsFinder.<>c__IteratorD1 <>c__IteratorD = new PawnsFinder.<>c__IteratorD1();
				PawnsFinder.<>c__IteratorD1 expr_07 = <>c__IteratorD;
				expr_07.$PC = -2;
				return expr_07;
			}
		}

		public static IEnumerable<Pawn> AllMaps_FreeColonistsAndPrisonersSpawned
		{
			get
			{
				PawnsFinder.<>c__IteratorD2 <>c__IteratorD = new PawnsFinder.<>c__IteratorD2();
				PawnsFinder.<>c__IteratorD2 expr_07 = <>c__IteratorD;
				expr_07.$PC = -2;
				return expr_07;
			}
		}

		public static IEnumerable<Pawn> AllMaps_FreeColonistsAndPrisoners
		{
			get
			{
				PawnsFinder.<>c__IteratorD3 <>c__IteratorD = new PawnsFinder.<>c__IteratorD3();
				PawnsFinder.<>c__IteratorD3 expr_07 = <>c__IteratorD;
				expr_07.$PC = -2;
				return expr_07;
			}
		}

		[DebuggerHidden]
		public static IEnumerable<Pawn> AllMaps_SpawnedPawnsInFaction(Faction faction)
		{
			PawnsFinder.<AllMaps_SpawnedPawnsInFaction>c__IteratorD4 <AllMaps_SpawnedPawnsInFaction>c__IteratorD = new PawnsFinder.<AllMaps_SpawnedPawnsInFaction>c__IteratorD4();
			<AllMaps_SpawnedPawnsInFaction>c__IteratorD.faction = faction;
			<AllMaps_SpawnedPawnsInFaction>c__IteratorD.<$>faction = faction;
			PawnsFinder.<AllMaps_SpawnedPawnsInFaction>c__IteratorD4 expr_15 = <AllMaps_SpawnedPawnsInFaction>c__IteratorD;
			expr_15.$PC = -2;
			return expr_15;
		}
	}
}
