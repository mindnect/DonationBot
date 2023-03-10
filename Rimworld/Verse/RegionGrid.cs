using System;
using System.Collections.Generic;

namespace Verse
{
	public sealed class RegionGrid
	{
		private const int CleanSquaresPerFrame = 16;

		private Map map;

		private Region[] regionGrid;

		private int curCleanIndex;

		public List<Room> allRooms = new List<Room>();

		public static HashSet<Region> allRegionsYielded = new HashSet<Region>();

		public HashSet<Region> drawnRegions = new HashSet<Region>();

		public IEnumerable<Region> AllRegions
		{
			get
			{
				RegionGrid.<>c__Iterator1DE <>c__Iterator1DE = new RegionGrid.<>c__Iterator1DE();
				<>c__Iterator1DE.<>f__this = this;
				RegionGrid.<>c__Iterator1DE expr_0E = <>c__Iterator1DE;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}

		public RegionGrid(Map map)
		{
			this.map = map;
			this.regionGrid = new Region[map.cellIndices.NumGridCells];
		}

		public Region GetValidRegionAt(IntVec3 c)
		{
			if (!c.InBounds(this.map))
			{
				Log.Error("Tried to get valid region out of bounds at " + c);
				return null;
			}
			this.map.regionAndRoomUpdater.RebuildDirtyRegionsAndRooms();
			Region region = this.regionGrid[this.map.cellIndices.CellToIndex(c)];
			if (region != null && region.valid)
			{
				return region;
			}
			return null;
		}

		public Region GetValidRegionAt_NoRebuild(IntVec3 c)
		{
			if (!c.InBounds(this.map))
			{
				Log.Error("Tried to get valid region out of bounds at " + c);
				return null;
			}
			Region region = this.regionGrid[this.map.cellIndices.CellToIndex(c)];
			if (region != null && region.valid)
			{
				return region;
			}
			return null;
		}

		public Region GetRegionAt_InvalidAllowed(IntVec3 c)
		{
			return this.regionGrid[this.map.cellIndices.CellToIndex(c)];
		}

		public void SetRegionAt(IntVec3 c, Region reg)
		{
			this.regionGrid[this.map.cellIndices.CellToIndex(c)] = reg;
		}

		public void UpdateClean()
		{
			for (int i = 0; i < 16; i++)
			{
				if (this.curCleanIndex >= this.regionGrid.Length)
				{
					this.curCleanIndex = 0;
				}
				Region region = this.regionGrid[this.curCleanIndex];
				if (region != null && !region.valid)
				{
					this.regionGrid[this.curCleanIndex] = null;
				}
				this.curCleanIndex++;
			}
		}

		public void DebugDraw()
		{
			if (this.map != Find.VisibleMap)
			{
				return;
			}
			if (DebugViewSettings.drawRegionTraversal)
			{
				CellRect currentViewRect = Find.CameraDriver.CurrentViewRect;
				currentViewRect.ClipInsideMap(this.map);
				foreach (IntVec3 current in currentViewRect)
				{
					Region validRegionAt = this.GetValidRegionAt(current);
					if (validRegionAt != null && !this.drawnRegions.Contains(validRegionAt))
					{
						validRegionAt.DebugDraw();
						this.drawnRegions.Add(validRegionAt);
					}
				}
				this.drawnRegions.Clear();
			}
			IntVec3 c = UI.MouseCell();
			if (c.InBounds(this.map))
			{
				if (DebugViewSettings.drawRooms)
				{
					Room room = RoomQuery.RoomAt(c, this.map);
					if (room != null)
					{
						room.DebugDraw();
					}
				}
				if (DebugViewSettings.drawRegions || DebugViewSettings.drawRegionLinks)
				{
					Region regionAt_InvalidAllowed = this.GetRegionAt_InvalidAllowed(c);
					if (regionAt_InvalidAllowed != null)
					{
						regionAt_InvalidAllowed.DebugDrawMouseover();
					}
				}
			}
		}
	}
}
