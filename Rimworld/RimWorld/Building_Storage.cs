using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;

namespace RimWorld
{
	public class Building_Storage : Building, IStoreSettingsParent, ISlotGroupParent
	{
		public SlotGroup slotGroup;

		public StorageSettings settings;

		private List<IntVec3> cachedOccupiedCells;

		public bool StorageTabVisible
		{
			get
			{
				return true;
			}
		}

		public SlotGroup GetSlotGroup()
		{
			return this.slotGroup;
		}

		public virtual void Notify_ReceivedThing(Thing newItem)
		{
			if (base.Faction == Faction.OfPlayer && newItem.def.storedConceptLearnOpportunity != null)
			{
				LessonAutoActivator.TeachOpportunity(newItem.def.storedConceptLearnOpportunity, OpportunityType.GoodToKnow);
			}
		}

		public virtual void Notify_LostThing(Thing newItem)
		{
		}

		[DebuggerHidden]
		public virtual IEnumerable<IntVec3> AllSlotCells()
		{
			Building_Storage.<AllSlotCells>c__Iterator130 <AllSlotCells>c__Iterator = new Building_Storage.<AllSlotCells>c__Iterator130();
			<AllSlotCells>c__Iterator.<>f__this = this;
			Building_Storage.<AllSlotCells>c__Iterator130 expr_0E = <AllSlotCells>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		public List<IntVec3> AllSlotCellsList()
		{
			if (this.cachedOccupiedCells == null)
			{
				this.cachedOccupiedCells = this.AllSlotCells().ToList<IntVec3>();
			}
			return this.cachedOccupiedCells;
		}

		public StorageSettings GetStoreSettings()
		{
			return this.settings;
		}

		public StorageSettings GetParentStoreSettings()
		{
			return this.def.building.fixedStorageSettings;
		}

		public string SlotYielderLabel()
		{
			return this.LabelCap;
		}

		public override void PostMake()
		{
			base.PostMake();
			this.settings = new StorageSettings(this);
			if (this.def.building.defaultStorageSettings != null)
			{
				this.settings.CopyFrom(this.def.building.defaultStorageSettings);
			}
		}

		public override void SpawnSetup(Map map)
		{
			base.SpawnSetup(map);
			this.cachedOccupiedCells = this.AllSlotCells().ToList<IntVec3>();
			this.slotGroup = new SlotGroup(this);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.LookDeep<StorageSettings>(ref this.settings, "settings", new object[]
			{
				this
			});
		}

		public override void DeSpawn()
		{
			if (this.slotGroup != null)
			{
				this.slotGroup.Notify_ParentDestroying();
			}
			base.DeSpawn();
		}

		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			Building_Storage.<GetGizmos>c__Iterator131 <GetGizmos>c__Iterator = new Building_Storage.<GetGizmos>c__Iterator131();
			<GetGizmos>c__Iterator.<>f__this = this;
			Building_Storage.<GetGizmos>c__Iterator131 expr_0E = <GetGizmos>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		virtual Map get_Map()
		{
			return base.Map;
		}
	}
}
