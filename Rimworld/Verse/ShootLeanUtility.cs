using System;
using System.Collections.Generic;

namespace Verse
{
	public static class ShootLeanUtility
	{
		private static Queue<bool[]> blockedArrays = new Queue<bool[]>();

		private static bool[] GetWorkingBlockedArray()
		{
			if (ShootLeanUtility.blockedArrays.Count > 0)
			{
				return ShootLeanUtility.blockedArrays.Dequeue();
			}
			return new bool[8];
		}

		private static void ReturnWorkingBlockedArray(bool[] ar)
		{
			ShootLeanUtility.blockedArrays.Enqueue(ar);
			if (ShootLeanUtility.blockedArrays.Count > 128)
			{
				Log.ErrorOnce("Too many blocked arrays to be feasible. >128", 388121);
			}
		}

		public static void LeanShootingSourcesFromTo(IntVec3 shooterLoc, IntVec3 targetPos, Map map, List<IntVec3> listToFill)
		{
			listToFill.Clear();
			float angleFlat = (targetPos - shooterLoc).AngleFlat;
			bool flag = angleFlat > 270f || angleFlat < 90f;
			bool flag2 = angleFlat > 90f && angleFlat < 270f;
			bool flag3 = angleFlat > 180f;
			bool flag4 = angleFlat < 180f;
			bool[] workingBlockedArray = ShootLeanUtility.GetWorkingBlockedArray();
			for (int i = 0; i < 8; i++)
			{
				workingBlockedArray[i] = !(shooterLoc + GenAdj.AdjacentCells[i]).CanBeSeenOver(map);
			}
			if (!workingBlockedArray[1] && ((workingBlockedArray[0] && !workingBlockedArray[5] && flag) || (workingBlockedArray[2] && !workingBlockedArray[4] && flag2)))
			{
				listToFill.Add(shooterLoc + new IntVec3(1, 0, 0));
			}
			if (!workingBlockedArray[3] && ((workingBlockedArray[0] && !workingBlockedArray[6] && flag) || (workingBlockedArray[2] && !workingBlockedArray[7] && flag2)))
			{
				listToFill.Add(shooterLoc + new IntVec3(-1, 0, 0));
			}
			if (!workingBlockedArray[2] && ((workingBlockedArray[3] && !workingBlockedArray[7] && flag3) || (workingBlockedArray[1] && !workingBlockedArray[4] && flag4)))
			{
				listToFill.Add(shooterLoc + new IntVec3(0, 0, -1));
			}
			if (!workingBlockedArray[0] && ((workingBlockedArray[3] && !workingBlockedArray[6] && flag3) || (workingBlockedArray[1] && !workingBlockedArray[5] && flag4)))
			{
				listToFill.Add(shooterLoc + new IntVec3(0, 0, 1));
			}
			for (int j = 0; j < 4; j++)
			{
				if (!workingBlockedArray[j])
				{
					if (j != 0 || flag)
					{
						if (j != 1 || flag4)
						{
							if (j != 2 || flag2)
							{
								if (j != 3 || flag3)
								{
									if ((shooterLoc + GenAdj.AdjacentCells[j]).GetCover(map) != null)
									{
										listToFill.Add(shooterLoc + GenAdj.AdjacentCells[j]);
									}
								}
							}
						}
					}
				}
			}
			ShootLeanUtility.ReturnWorkingBlockedArray(workingBlockedArray);
		}

		public static void CalcShootableCellsOf(List<IntVec3> outCells, Thing t)
		{
			outCells.Clear();
			if (t is Pawn)
			{
				outCells.Add(t.Position);
				for (int i = 0; i < 4; i++)
				{
					IntVec3 intVec = t.Position + GenAdj.CardinalDirections[i];
					if (intVec.CanBeSeenOver(t.Map))
					{
						outCells.Add(intVec);
					}
				}
			}
			else
			{
				CellRect.CellRectIterator iterator = t.OccupiedRect().GetIterator();
				while (!iterator.Done())
				{
					outCells.Add(iterator.Current);
					iterator.MoveNext();
				}
			}
		}
	}
}
