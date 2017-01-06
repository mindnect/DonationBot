using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse.Sound;

namespace Verse
{
	public abstract class Zone : IExposable, ISelectable
	{
		private const int StaticFireCheckInterval = 1000;

		public ZoneManager zoneManager;

		public string label;

		public List<IntVec3> cells = new List<IntVec3>();

		private bool cellsShuffled;

		public Color color = Color.white;

		private Material materialInt;

		public bool hidden;

		private int lastStaticFireCheckTick = -9999;

		private bool lastStaticFireCheckResult;

		private static BoolGrid extantGrid;

		private static BoolGrid foundGrid;

		public Map Map
		{
			get
			{
				return this.zoneManager.map;
			}
		}

		public Material Material
		{
			get
			{
				if (this.materialInt == null)
				{
					this.materialInt = SolidColorMaterials.SimpleSolidColorMaterial(this.color);
					this.materialInt.renderQueue = 3600;
				}
				return this.materialInt;
			}
		}

		public List<IntVec3> Cells
		{
			get
			{
				if (!this.cellsShuffled)
				{
					this.cells.Shuffle<IntVec3>();
					this.cellsShuffled = true;
				}
				return this.cells;
			}
		}

		public IEnumerable<Thing> AllContainedThings
		{
			get
			{
				Zone.<>c__IteratorAF <>c__IteratorAF = new Zone.<>c__IteratorAF();
				<>c__IteratorAF.<>f__this = this;
				Zone.<>c__IteratorAF expr_0E = <>c__IteratorAF;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}

		public bool ContainsStaticFire
		{
			get
			{
				if (Find.TickManager.TicksGame > this.lastStaticFireCheckTick + 1000)
				{
					this.lastStaticFireCheckResult = false;
					for (int i = 0; i < this.cells.Count; i++)
					{
						if (this.cells[i].ContainsStaticFire(this.Map))
						{
							this.lastStaticFireCheckResult = true;
							break;
						}
					}
				}
				return this.lastStaticFireCheckResult;
			}
		}

		public virtual bool IsMultiselectable
		{
			get
			{
				return false;
			}
		}

		protected abstract Color NextZoneColor
		{
			get;
		}

		public Zone()
		{
		}

		public Zone(string baseName, ZoneManager zoneManager)
		{
			this.label = zoneManager.NewZoneName(baseName);
			this.zoneManager = zoneManager;
			this.color = this.NextZoneColor;
			zoneManager.RegisterZone(this);
		}

		[DebuggerHidden]
		public IEnumerator<IntVec3> GetEnumerator()
		{
			Zone.<GetEnumerator>c__IteratorAE <GetEnumerator>c__IteratorAE = new Zone.<GetEnumerator>c__IteratorAE();
			<GetEnumerator>c__IteratorAE.<>f__this = this;
			return <GetEnumerator>c__IteratorAE;
		}

		public virtual void ExposeData()
		{
			Scribe_Values.LookValue<string>(ref this.label, "label", null, false);
			Scribe_Values.LookValue<Color>(ref this.color, "color", default(Color), false);
			Scribe_Values.LookValue<bool>(ref this.hidden, "hidden", false, false);
			Scribe_Collections.LookList<IntVec3>(ref this.cells, "cells", LookMode.Undefined, new object[0]);
		}

		public virtual void AddCell(IntVec3 c)
		{
			if (this.cells.Contains(c))
			{
				Log.Error(string.Concat(new object[]
				{
					"Adding cell to zone which already has it. c=",
					c,
					", zone=",
					this
				}));
				return;
			}
			List<Thing> list = this.Map.thingGrid.ThingsListAt(c);
			for (int i = 0; i < list.Count; i++)
			{
				Thing thing = list[i];
				if (!thing.def.CanOverlapZones)
				{
					Log.Error("Added zone over zone-incompatible thing " + thing);
					return;
				}
			}
			this.cells.Add(c);
			this.zoneManager.AddZoneGridCell(this, c);
			this.Map.mapDrawer.MapMeshDirty(c, MapMeshFlag.Zone);
			AutoHomeAreaMaker.Notify_ZoneCellAdded(c, this);
			this.cellsShuffled = false;
		}

		public virtual void RemoveCell(IntVec3 c)
		{
			if (!this.cells.Contains(c))
			{
				Log.Error(string.Concat(new object[]
				{
					"Cannot remove cell from zone which doesn't have it. c=",
					c,
					", zone=",
					this
				}));
				return;
			}
			this.cells.Remove(c);
			this.zoneManager.ClearZoneGridCell(c);
			this.Map.mapDrawer.MapMeshDirty(c, MapMeshFlag.Zone);
			this.cellsShuffled = false;
			if (this.cells.Count == 0)
			{
				this.Deregister();
			}
		}

		public virtual void Delete()
		{
			if (this.cells.Count == 0)
			{
				this.Deregister();
			}
			else
			{
				while (this.cells.Count > 0)
				{
					this.RemoveCell(this.cells[this.cells.Count - 1]);
				}
			}
			Find.Selector.Deselect(this);
			SoundDefOf.DesignateZoneDelete.PlayOneShotOnCamera();
		}

		public virtual void Deregister()
		{
			this.zoneManager.DeregisterZone(this);
		}

		public bool ContainsCell(IntVec3 c)
		{
			for (int i = 0; i < this.cells.Count; i++)
			{
				if (this.cells[i] == c)
				{
					return true;
				}
			}
			return false;
		}

		public virtual string GetInspectString()
		{
			return string.Empty;
		}

		[DebuggerHidden]
		public virtual IEnumerable<InspectTabBase> GetInspectTabs()
		{
			Zone.<GetInspectTabs>c__IteratorB0 <GetInspectTabs>c__IteratorB = new Zone.<GetInspectTabs>c__IteratorB0();
			Zone.<GetInspectTabs>c__IteratorB0 expr_07 = <GetInspectTabs>c__IteratorB;
			expr_07.$PC = -2;
			return expr_07;
		}

		[DebuggerHidden]
		public virtual IEnumerable<Gizmo> GetGizmos()
		{
			Zone.<GetGizmos>c__IteratorB1 <GetGizmos>c__IteratorB = new Zone.<GetGizmos>c__IteratorB1();
			<GetGizmos>c__IteratorB.<>f__this = this;
			Zone.<GetGizmos>c__IteratorB1 expr_0E = <GetGizmos>c__IteratorB;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		public void CheckContiguous()
		{
			if (this.cells.Count == 0)
			{
				return;
			}
			if (Zone.extantGrid == null)
			{
				Zone.extantGrid = new BoolGrid(this.Map);
			}
			else
			{
				Zone.extantGrid.ClearAndResizeTo(this.Map);
			}
			if (Zone.foundGrid == null)
			{
				Zone.foundGrid = new BoolGrid(this.Map);
			}
			else
			{
				Zone.foundGrid.ClearAndResizeTo(this.Map);
			}
			for (int i = 0; i < this.cells.Count; i++)
			{
				Zone.extantGrid.Set(this.cells[i], true);
			}
			Predicate<IntVec3> passCheck = (IntVec3 c) => Zone.extantGrid[c] && !Zone.foundGrid[c];
			int numFound = 0;
			Action<IntVec3> processor = delegate(IntVec3 c)
			{
				Zone.foundGrid.Set(c, true);
				numFound++;
			};
			this.Map.floodFiller.FloodFill(this.cells[0], passCheck, processor);
			if (numFound < this.cells.Count)
			{
				foreach (IntVec3 current in this.Map.AllCells)
				{
					if (Zone.extantGrid[current] && !Zone.foundGrid[current])
					{
						this.RemoveCell(current);
					}
				}
			}
		}

		public override string ToString()
		{
			return this.label;
		}
	}
}
