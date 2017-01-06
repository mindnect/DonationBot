using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse.AI;
using Verse.Sound;

namespace Verse
{
	public class Building : ThingWithComps
	{
		private Sustainer sustainerAmbient;

		public CompPower PowerComp
		{
			get
			{
				return base.GetComp<CompPower>();
			}
		}

		public virtual bool TransmitsPowerNow
		{
			get
			{
				CompPower powerComp = this.PowerComp;
				return powerComp != null && powerComp.Props.transmitsPower;
			}
		}

		public override int HitPoints
		{
			set
			{
				int hitPoints = this.HitPoints;
				base.HitPoints = value;
				BuildingsDamageSectionLayerUtility.Notify_BuildingHitPointsChanged(this, hitPoints);
			}
		}

		public override void SpawnSetup(Map map)
		{
			base.SpawnSetup(map);
			base.Map.listerBuildings.Add(this);
			if (this.def.coversFloor)
			{
				base.Map.mapDrawer.MapMeshDirty(base.Position, MapMeshFlag.Terrain, true, false);
			}
			CellRect cellRect = this.OccupiedRect();
			for (int i = cellRect.minZ; i <= cellRect.maxZ; i++)
			{
				for (int j = cellRect.minX; j <= cellRect.maxX; j++)
				{
					IntVec3 intVec = new IntVec3(j, 0, i);
					base.Map.mapDrawer.MapMeshDirty(intVec, MapMeshFlag.Buildings);
					base.Map.glowGrid.MarkGlowGridDirty(intVec);
					if (!SnowGrid.CanCoexistWithSnow(this.def))
					{
						base.Map.snowGrid.SetDepth(intVec, 0f);
					}
				}
			}
			if (this.def.IsEdifice())
			{
				base.Map.edificeGrid.Register(this);
			}
			if (base.Faction == Faction.OfPlayer && this.def.building != null && this.def.building.spawnedConceptLearnOpportunity != null)
			{
				LessonAutoActivator.TeachOpportunity(this.def.building.spawnedConceptLearnOpportunity, OpportunityType.GoodToKnow);
			}
			AutoHomeAreaMaker.Notify_BuildingSpawned(this);
			if (this.def.building != null && !this.def.building.soundAmbient.NullOrUndefined())
			{
				LongEventHandler.ExecuteWhenFinished(delegate
				{
					SoundInfo info = SoundInfo.InMap(this, MaintenanceType.None);
					this.sustainerAmbient = this.def.building.soundAmbient.TrySpawnSustainer(info);
				});
			}
			base.Map.listerBuildingsRepairable.Notify_BuildingSpawned(this);
			if (!this.CanBeSeenOver())
			{
				base.Map.exitMapGrid.Notify_LOSBlockerSpawned();
			}
		}

		public override void DeSpawn()
		{
			if (this.def.IsEdifice())
			{
				base.Map.edificeGrid.DeRegister(this);
			}
			Map map = base.Map;
			base.DeSpawn();
			if (this.def.MakeFog)
			{
				map.fogGrid.Notify_FogBlockerRemoved(base.Position);
			}
			if (this.def.holdsRoof)
			{
				RoofCollapseCellsFinder.Notify_RoofHolderDespawned(this, map);
			}
			if (this.sustainerAmbient != null)
			{
				this.sustainerAmbient.End();
			}
			CellRect cellRect = this.OccupiedRect();
			for (int i = cellRect.minZ; i <= cellRect.maxZ; i++)
			{
				for (int j = cellRect.minX; j <= cellRect.maxX; j++)
				{
					IntVec3 loc = new IntVec3(j, 0, i);
					MapMeshFlag mapMeshFlag = MapMeshFlag.Buildings;
					if (this.def.coversFloor)
					{
						mapMeshFlag |= MapMeshFlag.Terrain;
					}
					if (this.def.Fillage == FillCategory.Full)
					{
						mapMeshFlag |= MapMeshFlag.Roofs;
						mapMeshFlag |= MapMeshFlag.Snow;
					}
					map.mapDrawer.MapMeshDirty(loc, mapMeshFlag);
					map.glowGrid.MarkGlowGridDirty(loc);
				}
			}
			map.listerBuildings.Remove(this);
			map.listerBuildingsRepairable.Notify_BuildingDeSpawned(this);
			if (this.def.leaveTerrain != null && Current.ProgramState == ProgramState.Playing)
			{
				CellRect.CellRectIterator iterator = this.OccupiedRect().GetIterator();
				while (!iterator.Done())
				{
					map.terrainGrid.SetTerrain(iterator.Current, this.def.leaveTerrain);
					iterator.MoveNext();
				}
			}
			map.designationManager.Notify_BuildingDespawned(this);
			if (!this.CanBeSeenOver())
			{
				map.exitMapGrid.Notify_LOSBlockerDespawned();
			}
			if (this.def.building.hasFuelingPort)
			{
				IntVec3 fuelingPortCell = FuelingPortUtility.GetFuelingPortCell(base.Position, base.Rotation);
				CompLaunchable compLaunchable = FuelingPortUtility.LaunchableAt(fuelingPortCell, map);
				if (compLaunchable != null)
				{
					compLaunchable.Notify_FuelingPortSourceDeSpawned();
				}
			}
			if (this.def.building.ai_combatDangerous)
			{
				AvoidGridMaker.Notify_CombatDangerousBuildingDespawned(this, map);
			}
		}

		public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
		{
			Map map = base.Map;
			base.Destroy(mode);
			InstallBlueprintUtility.CancelBlueprintsFor(this);
			if (mode == DestroyMode.Deconstruct)
			{
				SoundDef.Named("BuildingDeconstructed").PlayOneShot(new TargetInfo(base.Position, map, false));
			}
		}

		public override void Draw()
		{
			if (this.def.drawerType == DrawerType.RealtimeOnly)
			{
				base.Draw();
			}
			base.Comps_PostDraw();
		}

		public override void SetFaction(Faction newFaction, Pawn recruiter = null)
		{
			base.SetFaction(newFaction, recruiter);
			if (base.Spawned)
			{
				base.Map.listerBuildingsRepairable.Notify_BuildingFactionChanged(this);
			}
		}

		public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
		{
			base.PostApplyDamage(dinfo, totalDamageDealt);
			if (base.Spawned)
			{
				base.Map.listerBuildingsRepairable.Notify_BuildingTookDamage(this);
			}
		}

		public override void DrawExtraSelectionOverlays()
		{
			base.DrawExtraSelectionOverlays();
			Blueprint_Install blueprint_Install = InstallBlueprintUtility.ExistingBlueprintFor(this);
			if (blueprint_Install != null)
			{
				GenDraw.DrawLineBetween(this.TrueCenter(), blueprint_Install.TrueCenter());
			}
		}

		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			Building.<GetGizmos>c__Iterator127 <GetGizmos>c__Iterator = new Building.<GetGizmos>c__Iterator127();
			<GetGizmos>c__Iterator.<>f__this = this;
			Building.<GetGizmos>c__Iterator127 expr_0E = <GetGizmos>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		public virtual bool ClaimableBy(Faction by)
		{
			if (this.def.building.isNaturalRock || !this.def.building.claimable)
			{
				return false;
			}
			if (base.Faction != null)
			{
				List<Pawn> list = base.Map.mapPawns.SpawnedPawnsInFaction(base.Faction);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].RaceProps.Humanlike && !PawnUtility.ThreatDisabledOrFleeing(list[i]))
					{
						return false;
					}
				}
			}
			return true;
		}

		public virtual ushort PathFindCostFor(Pawn p)
		{
			return 0;
		}

		public virtual ushort PathWalkCostFor(Pawn p)
		{
			return 0;
		}
	}
}
