using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public static class PawnNameDatabaseSolid
	{
		private const float PreferredNameChance = 0.5f;

		private static Dictionary<GenderPossibility, List<NameTriple>> solidNames;

		static PawnNameDatabaseSolid()
		{
			PawnNameDatabaseSolid.solidNames = new Dictionary<GenderPossibility, List<NameTriple>>();
			using (IEnumerator enumerator = Enum.GetValues(typeof(GenderPossibility)).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					GenderPossibility key = (GenderPossibility)((byte)enumerator.Current);
					PawnNameDatabaseSolid.solidNames.Add(key, new List<NameTriple>());
				}
			}
		}

		public static void AddPlayerContentName(NameTriple newName, GenderPossibility genderPos)
		{
			PawnNameDatabaseSolid.solidNames[genderPos].Add(newName);
		}

		public static List<NameTriple> GetListForGender(GenderPossibility gp)
		{
			return PawnNameDatabaseSolid.solidNames[gp];
		}

		[DebuggerHidden]
		public static IEnumerable<NameTriple> AllNames()
		{
			PawnNameDatabaseSolid.<AllNames>c__IteratorDB <AllNames>c__IteratorDB = new PawnNameDatabaseSolid.<AllNames>c__IteratorDB();
			PawnNameDatabaseSolid.<AllNames>c__IteratorDB expr_07 = <AllNames>c__IteratorDB;
			expr_07.$PC = -2;
			return expr_07;
		}
	}
}
