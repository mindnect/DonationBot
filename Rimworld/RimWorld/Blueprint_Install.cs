using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	public class Blueprint_Install : Blueprint
	{
		private MinifiedThing miniToInstall;

		private Building buildingToReinstall;

		public Thing MiniToInstallOrBuildingToReinstall
		{
			get
			{
				if (this.miniToInstall != null)
				{
					return this.miniToInstall;
				}
				if (this.buildingToReinstall != null)
				{
					return this.buildingToReinstall;
				}
				throw new InvalidOperationException("Nothing to install.");
			}
		}

		private Thing ThingToInstall
		{
			get
			{
				return this.MiniToInstallOrBuildingToReinstall.GetInnerIfMinified();
			}
		}

		public override Graphic Graphic
		{
			get
			{
				Graphic graphic = this.ThingToInstall.def.installBlueprintDef.graphic;
				return graphic.ExtractInnerGraphicFor(this.ThingToInstall);
			}
		}

		protected override float WorkTotal
		{
			get
			{
				return 150f;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.LookReference<MinifiedThing>(ref this.miniToInstall, "miniToInstall", false);
			Scribe_References.LookReference<Building>(ref this.buildingToReinstall, "buildingToReinstall", false);
		}

		public override ThingDef UIStuff()
		{
			return this.ThingToInstall.Stuff;
		}

		public override List<ThingCountClass> MaterialsNeeded()
		{
			Log.Error("Called MaterialsNeeded on a Blueprint_Install.");
			return new List<ThingCountClass>();
		}

		protected override Thing MakeSolidThing()
		{
			return this.ThingToInstall;
		}

		public override bool TryReplaceWithSolidThing(Pawn workerPawn, out Thing createdThing, out bool jobEnded)
		{
			Map map = base.Map;
			bool flag = base.TryReplaceWithSolidThing(workerPawn, out createdThing, out jobEnded);
			if (flag)
			{
				SoundDefOf.BuildingComplete.PlayOneShot(new TargetInfo(base.Position, map, false));
				workerPawn.records.Increment(RecordDefOf.ThingsInstalled);
			}
			return flag;
		}

		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			Blueprint_Install.<GetGizmos>c__Iterator12A <GetGizmos>c__Iterator12A = new Blueprint_Install.<GetGizmos>c__Iterator12A();
			<GetGizmos>c__Iterator12A.<>f__this = this;
			Blueprint_Install.<GetGizmos>c__Iterator12A expr_0E = <GetGizmos>c__Iterator12A;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		internal void SetThingToInstallFromMinified(MinifiedThing itemToInstall)
		{
			this.miniToInstall = itemToInstall;
			this.buildingToReinstall = null;
		}

		internal void SetBuildingToReinstall(Building buildingToReinstall)
		{
			if (!buildingToReinstall.def.Minifiable)
			{
				Log.Error("Tried to reinstall non-minifiable building.");
				return;
			}
			this.miniToInstall = null;
			this.buildingToReinstall = buildingToReinstall;
		}
	}
}
