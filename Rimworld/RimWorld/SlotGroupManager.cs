using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorld
{
	public sealed class SlotGroupManager
	{
		private Map map;

		private List<SlotGroup> allGroups = new List<SlotGroup>();

		private SlotGroup[,,] groupGrid;

		public IEnumerable<SlotGroup> AllGroups
		{
			get
			{
				return this.allGroups;
			}
		}

		public List<SlotGroup> AllGroupsListForReading
		{
			get
			{
				return this.allGroups;
			}
		}

		public List<SlotGroup> AllGroupsListInPriorityOrder
		{
			get
			{
				return this.allGroups;
			}
		}

		public IEnumerable<IntVec3> AllSlots
		{
			get
			{
				SlotGroupManager.<>c__Iterator134 <>c__Iterator = new SlotGroupManager.<>c__Iterator134();
				<>c__Iterator.<>f__this = this;
				SlotGroupManager.<>c__Iterator134 expr_0E = <>c__Iterator;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}

		public SlotGroupManager(Map map)
		{
			this.map = map;
			this.groupGrid = new SlotGroup[map.Size.x, map.Size.y, map.Size.z];
		}

		public void AddGroup(SlotGroup newGroup)
		{
			if (this.allGroups.Contains(newGroup))
			{
				Log.Error("Double-added slot group. SlotGroup parent is " + newGroup.parent);
				return;
			}
			if ((from g in this.allGroups
			where g.parent == newGroup.parent
			select g).Any<SlotGroup>())
			{
				Log.Error("Added SlotGroup with a parent matching an existing one. Parent is " + newGroup.parent);
				return;
			}
			this.allGroups.Add(newGroup);
			this.allGroups.InsertionSort(new Comparison<SlotGroup>(SlotGroupManager.CompareSlotGroupPrioritiesDescending));
			List<IntVec3> cellsList = newGroup.CellsList;
			for (int i = 0; i < cellsList.Count; i++)
			{
				this.SetCellFor(cellsList[i], newGroup);
			}
			this.map.listerHaulables.Notify_SlotGroupChanged(newGroup);
		}

		public void RemoveGroup(SlotGroup oldGroup)
		{
			if (!this.allGroups.Contains(oldGroup))
			{
				Log.Error("Removing SlotGroup that isn't registered.");
				return;
			}
			this.allGroups.Remove(oldGroup);
			List<IntVec3> cellsList = oldGroup.CellsList;
			for (int i = 0; i < cellsList.Count; i++)
			{
				IntVec3 intVec = cellsList[i];
				this.groupGrid[intVec.x, intVec.y, intVec.z] = null;
			}
			this.map.listerHaulables.Notify_SlotGroupChanged(oldGroup);
		}

		public void Notify_GroupChangedPriority()
		{
			this.allGroups.InsertionSort(new Comparison<SlotGroup>(SlotGroupManager.CompareSlotGroupPrioritiesDescending));
		}

		public SlotGroup SlotGroupAt(IntVec3 loc)
		{
			return this.groupGrid[loc.x, loc.y, loc.z];
		}

		public void SetCellFor(IntVec3 c, SlotGroup group)
		{
			if (this.SlotGroupAt(c) != null)
			{
				Log.Error(string.Concat(new object[]
				{
					group,
					" overwriting slot group square ",
					c,
					" of ",
					this.SlotGroupAt(c)
				}));
			}
			this.groupGrid[c.x, c.y, c.z] = group;
		}

		public void ClearCellFor(IntVec3 c, SlotGroup group)
		{
			if (this.SlotGroupAt(c) != group)
			{
				Log.Error(string.Concat(new object[]
				{
					group,
					" clearing group grid square ",
					c,
					" containing ",
					this.SlotGroupAt(c)
				}));
			}
			this.groupGrid[c.x, c.y, c.z] = null;
		}

		private static int CompareSlotGroupPrioritiesDescending(SlotGroup a, SlotGroup b)
		{
			return ((int)b.Settings.Priority).CompareTo((int)a.Settings.Priority);
		}
	}
}
