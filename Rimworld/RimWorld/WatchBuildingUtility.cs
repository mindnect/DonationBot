using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public static class WatchBuildingUtility
	{
		private static List<int> allowedDirections = new List<int>();

		[DebuggerHidden]
		public static IEnumerable<IntVec3> CalculateWatchCells(ThingDef def, IntVec3 center, Rot4 rot, Map map)
		{
			WatchBuildingUtility.<CalculateWatchCells>c__Iterator56 <CalculateWatchCells>c__Iterator = new WatchBuildingUtility.<CalculateWatchCells>c__Iterator56();
			<CalculateWatchCells>c__Iterator.def = def;
			<CalculateWatchCells>c__Iterator.rot = rot;
			<CalculateWatchCells>c__Iterator.center = center;
			<CalculateWatchCells>c__Iterator.map = map;
			<CalculateWatchCells>c__Iterator.<$>def = def;
			<CalculateWatchCells>c__Iterator.<$>rot = rot;
			<CalculateWatchCells>c__Iterator.<$>center = center;
			<CalculateWatchCells>c__Iterator.<$>map = map;
			WatchBuildingUtility.<CalculateWatchCells>c__Iterator56 expr_3F = <CalculateWatchCells>c__Iterator;
			expr_3F.$PC = -2;
			return expr_3F;
		}

		public static bool TryFindBestWatchCell(Thing toWatch, Pawn pawn, bool desireSit, out IntVec3 result, out Building chair)
		{
			List<int> list = WatchBuildingUtility.CalculateAllowedDirections(toWatch.def, toWatch.Rotation);
			IntVec3 intVec = IntVec3.Invalid;
			for (int i = 0; i < list.Count; i++)
			{
				CellRect watchCellRect = WatchBuildingUtility.GetWatchCellRect(toWatch.def, toWatch.Position, toWatch.Rotation, list[i]);
				IntVec3 centerCell = watchCellRect.CenterCell;
				int num = watchCellRect.Area * 4;
				for (int j = 0; j < num; j++)
				{
					IntVec3 intVec2 = centerCell + GenRadial.RadialPattern[j];
					if (watchCellRect.Contains(intVec2))
					{
						bool flag = false;
						Building building = null;
						if (WatchBuildingUtility.EverPossibleToWatchFrom(intVec2, toWatch.Position, toWatch.Map, false))
						{
							if (desireSit)
							{
								building = intVec2.GetEdifice(pawn.Map);
								if (building != null && building.def.building.isSittable && pawn.CanReserve(building, 1))
								{
									flag = true;
								}
							}
							else if (!intVec2.IsForbidden(pawn) && pawn.CanReserve(intVec2, 1) && !pawn.Map.pawnDestinationManager.DestinationIsReserved(intVec2, pawn))
							{
								flag = true;
							}
						}
						if (flag)
						{
							if (desireSit)
							{
								Rot4 arg_157_0 = building.Rotation;
								Rot4 rot = new Rot4(list[i]);
								if (arg_157_0 != rot.Opposite)
								{
									intVec = intVec2;
									goto IL_178;
								}
							}
							result = intVec2;
							chair = building;
							return true;
						}
					}
					IL_178:;
				}
			}
			if (intVec.IsValid)
			{
				result = intVec;
				chair = intVec.GetEdifice(pawn.Map);
				return true;
			}
			result = IntVec3.Invalid;
			chair = null;
			return false;
		}

		public static bool CanWatchFromBed(Pawn pawn, Building_Bed bed, Thing toWatch)
		{
			if (!WatchBuildingUtility.EverPossibleToWatchFrom(pawn.Position, toWatch.Position, pawn.Map, true))
			{
				return false;
			}
			if (toWatch.def.rotatable)
			{
				Rot4 rotation = bed.Rotation;
				CellRect cellRect = toWatch.OccupiedRect();
				if (rotation == Rot4.North && cellRect.maxZ < pawn.Position.z)
				{
					return false;
				}
				if (rotation == Rot4.South && cellRect.minZ > pawn.Position.z)
				{
					return false;
				}
				if (rotation == Rot4.East && cellRect.maxX < pawn.Position.x)
				{
					return false;
				}
				if (rotation == Rot4.West && cellRect.minX > pawn.Position.x)
				{
					return false;
				}
			}
			List<int> list = WatchBuildingUtility.CalculateAllowedDirections(toWatch.def, toWatch.Rotation);
			for (int i = 0; i < list.Count; i++)
			{
				if (WatchBuildingUtility.GetWatchCellRect(toWatch.def, toWatch.Position, toWatch.Rotation, list[i]).Contains(pawn.Position))
				{
					return true;
				}
			}
			return false;
		}

		private static CellRect GetWatchCellRect(ThingDef def, IntVec3 center, Rot4 rot, int watchRot)
		{
			Rot4 a = new Rot4(watchRot);
			CellRect result;
			if (a.IsHorizontal)
			{
				int num = center.x + GenAdj.CardinalDirections[watchRot].x * def.building.watchBuildingStandDistanceRange.min;
				int num2 = center.x + GenAdj.CardinalDirections[watchRot].x * def.building.watchBuildingStandDistanceRange.max;
				int num3 = center.z + def.building.watchBuildingStandRectWidth / 2;
				int num4 = center.z - def.building.watchBuildingStandRectWidth / 2;
				if (def.building.watchBuildingStandRectWidth % 2 == 0)
				{
					if (a == Rot4.West)
					{
						num4++;
					}
					else
					{
						num3--;
					}
				}
				result = new CellRect(Mathf.Min(num, num2), num4, Mathf.Abs(num - num2) + 1, num3 - num4 + 1);
			}
			else
			{
				int num5 = center.z + GenAdj.CardinalDirections[watchRot].z * def.building.watchBuildingStandDistanceRange.min;
				int num6 = center.z + GenAdj.CardinalDirections[watchRot].z * def.building.watchBuildingStandDistanceRange.max;
				int num7 = center.x + def.building.watchBuildingStandRectWidth / 2;
				int num8 = center.x - def.building.watchBuildingStandRectWidth / 2;
				if (def.building.watchBuildingStandRectWidth % 2 == 0)
				{
					if (a == Rot4.North)
					{
						num8++;
					}
					else
					{
						num7--;
					}
				}
				result = new CellRect(num8, Mathf.Min(num5, num6), num7 - num8 + 1, Mathf.Abs(num5 - num6) + 1);
			}
			return result;
		}

		private static bool EverPossibleToWatchFrom(IntVec3 watchCell, IntVec3 buildingCenter, Map map, bool bedAllowed)
		{
			return (watchCell.Standable(map) || (bedAllowed && watchCell.GetEdifice(map) is Building_Bed)) && GenSight.LineOfSight(buildingCenter, watchCell, map, true);
		}

		private static List<int> CalculateAllowedDirections(ThingDef toWatchDef, Rot4 toWatchRot)
		{
			WatchBuildingUtility.allowedDirections.Clear();
			if (toWatchDef.rotatable)
			{
				WatchBuildingUtility.allowedDirections.Add(toWatchRot.AsInt);
			}
			else
			{
				WatchBuildingUtility.allowedDirections.Add(0);
				WatchBuildingUtility.allowedDirections.Add(1);
				WatchBuildingUtility.allowedDirections.Add(2);
				WatchBuildingUtility.allowedDirections.Add(3);
			}
			return WatchBuildingUtility.allowedDirections;
		}
	}
}
