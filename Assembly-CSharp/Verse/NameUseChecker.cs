using System;
using System.Collections.Generic;

namespace Verse
{
	public static class NameUseChecker
	{
		public static IEnumerable<Name> AllPawnsNamesEverUsed
		{
			get
			{
				NameUseChecker.<>c__Iterator1D7 <>c__Iterator1D = new NameUseChecker.<>c__Iterator1D7();
				NameUseChecker.<>c__Iterator1D7 expr_07 = <>c__Iterator1D;
				expr_07.$PC = -2;
				return expr_07;
			}
		}

		public static bool NameWordIsUsed(string singleName)
		{
			foreach (Name current in NameUseChecker.AllPawnsNamesEverUsed)
			{
				NameTriple nameTriple = current as NameTriple;
				if (nameTriple != null && (singleName == nameTriple.First || singleName == nameTriple.Nick || singleName == nameTriple.Last))
				{
					bool result = true;
					return result;
				}
				NameSingle nameSingle = current as NameSingle;
				if (nameSingle != null && nameSingle.Name == singleName)
				{
					bool result = true;
					return result;
				}
			}
			return false;
		}

		public static bool NameSingleIsUsedOnAnyMap(string candidate)
		{
			List<Map> maps = Find.Maps;
			for (int i = 0; i < maps.Count; i++)
			{
				foreach (Pawn current in maps[i].mapPawns.AllPawns)
				{
					NameSingle nameSingle = current.Name as NameSingle;
					if (nameSingle != null && nameSingle.Name == candidate)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
