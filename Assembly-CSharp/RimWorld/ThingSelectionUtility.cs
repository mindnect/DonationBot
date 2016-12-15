using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public static class ThingSelectionUtility
	{
		private static HashSet<Thing> yieldedThings = new HashSet<Thing>();

		private static HashSet<Zone> yieldedZones = new HashSet<Zone>();

		private static List<Pawn> tmpColonists = new List<Pawn>();

		public static bool SelectableByMapClick(Thing t)
		{
			if (!t.def.selectable)
			{
				return false;
			}
			if (!t.Spawned)
			{
				return false;
			}
			if (t.def.size.x == 1 && t.def.size.z == 1)
			{
				return !t.Position.Fogged(t.Map);
			}
			CellRect.CellRectIterator iterator = t.OccupiedRect().GetIterator();
			while (!iterator.Done())
			{
				if (!iterator.Current.Fogged(t.Map))
				{
					return true;
				}
				iterator.MoveNext();
			}
			return false;
		}

		public static bool SelectableByHotkey(Thing t)
		{
			if (!t.def.selectable)
			{
				return false;
			}
			Pawn pawn = t as Pawn;
			if (pawn != null)
			{
				if (pawn.Dead)
				{
					return false;
				}
				if (pawn.InContainerEnclosed)
				{
					return false;
				}
			}
			return true;
		}

		[DebuggerHidden]
		public static IEnumerable<Thing> MultiSelectableThingsInScreenRectDistinct(Rect rect)
		{
			ThingSelectionUtility.<MultiSelectableThingsInScreenRectDistinct>c__Iterator17E <MultiSelectableThingsInScreenRectDistinct>c__Iterator17E = new ThingSelectionUtility.<MultiSelectableThingsInScreenRectDistinct>c__Iterator17E();
			<MultiSelectableThingsInScreenRectDistinct>c__Iterator17E.rect = rect;
			<MultiSelectableThingsInScreenRectDistinct>c__Iterator17E.<$>rect = rect;
			ThingSelectionUtility.<MultiSelectableThingsInScreenRectDistinct>c__Iterator17E expr_15 = <MultiSelectableThingsInScreenRectDistinct>c__Iterator17E;
			expr_15.$PC = -2;
			return expr_15;
		}

		[DebuggerHidden]
		public static IEnumerable<Zone> MultiSelectableZonesInScreenRectDistinct(Rect rect)
		{
			ThingSelectionUtility.<MultiSelectableZonesInScreenRectDistinct>c__Iterator17F <MultiSelectableZonesInScreenRectDistinct>c__Iterator17F = new ThingSelectionUtility.<MultiSelectableZonesInScreenRectDistinct>c__Iterator17F();
			<MultiSelectableZonesInScreenRectDistinct>c__Iterator17F.rect = rect;
			<MultiSelectableZonesInScreenRectDistinct>c__Iterator17F.<$>rect = rect;
			ThingSelectionUtility.<MultiSelectableZonesInScreenRectDistinct>c__Iterator17F expr_15 = <MultiSelectableZonesInScreenRectDistinct>c__Iterator17F;
			expr_15.$PC = -2;
			return expr_15;
		}

		private static CellRect GetMapRect(Rect rect)
		{
			Vector2 screenLoc = new Vector2(rect.x, (float)UI.screenHeight - rect.y);
			Vector2 screenLoc2 = new Vector2(rect.x + rect.width, (float)UI.screenHeight - (rect.y + rect.height));
			Vector3 vector = UI.UIToMapPosition(screenLoc);
			Vector3 vector2 = UI.UIToMapPosition(screenLoc2);
			return new CellRect
			{
				minX = Mathf.FloorToInt(vector.x),
				minZ = Mathf.FloorToInt(vector2.z),
				maxX = Mathf.FloorToInt(vector2.x),
				maxZ = Mathf.FloorToInt(vector.z)
			};
		}

		public static void SelectNextColonist()
		{
			ThingSelectionUtility.tmpColonists.Clear();
			ThingSelectionUtility.tmpColonists.AddRange(Find.ColonistBar.GetColonistsInOrder().Where(new Func<Pawn, bool>(ThingSelectionUtility.SelectableByHotkey)));
			if (ThingSelectionUtility.tmpColonists.Count == 0)
			{
				return;
			}
			bool worldRenderedNow = WorldRendererUtility.WorldRenderedNow;
			int num = -1;
			for (int i = ThingSelectionUtility.tmpColonists.Count - 1; i >= 0; i--)
			{
				if ((!worldRenderedNow && Find.Selector.IsSelected(ThingSelectionUtility.tmpColonists[i])) || (worldRenderedNow && ThingSelectionUtility.tmpColonists[i].IsCaravanMember() && Find.WorldSelector.IsSelected(ThingSelectionUtility.tmpColonists[i].GetCaravan())))
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				JumpToTargetUtility.TryJumpAndSelect(ThingSelectionUtility.tmpColonists[0]);
			}
			else
			{
				JumpToTargetUtility.TryJumpAndSelect(ThingSelectionUtility.tmpColonists[(num + 1) % ThingSelectionUtility.tmpColonists.Count]);
			}
			ThingSelectionUtility.tmpColonists.Clear();
		}

		public static void SelectPreviousColonist()
		{
			ThingSelectionUtility.tmpColonists.Clear();
			ThingSelectionUtility.tmpColonists.AddRange(Find.ColonistBar.GetColonistsInOrder().Where(new Func<Pawn, bool>(ThingSelectionUtility.SelectableByHotkey)));
			if (ThingSelectionUtility.tmpColonists.Count == 0)
			{
				return;
			}
			bool worldRenderedNow = WorldRendererUtility.WorldRenderedNow;
			int num = -1;
			for (int i = 0; i < ThingSelectionUtility.tmpColonists.Count; i++)
			{
				if ((!worldRenderedNow && Find.Selector.IsSelected(ThingSelectionUtility.tmpColonists[i])) || (worldRenderedNow && ThingSelectionUtility.tmpColonists[i].IsCaravanMember() && Find.WorldSelector.IsSelected(ThingSelectionUtility.tmpColonists[i].GetCaravan())))
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				JumpToTargetUtility.TryJumpAndSelect(ThingSelectionUtility.tmpColonists[ThingSelectionUtility.tmpColonists.Count - 1]);
			}
			else
			{
				JumpToTargetUtility.TryJumpAndSelect(ThingSelectionUtility.tmpColonists[GenMath.PositiveMod(num - 1, ThingSelectionUtility.tmpColonists.Count)]);
			}
			ThingSelectionUtility.tmpColonists.Clear();
		}
	}
}
