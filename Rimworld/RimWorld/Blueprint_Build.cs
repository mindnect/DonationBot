using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Verse;

namespace RimWorld
{
	public class Blueprint_Build : Blueprint
	{
		public ThingDef stuffToUse;

		public override string Label
		{
			get
			{
				string label = base.Label;
				if (this.stuffToUse != null)
				{
					return "ThingMadeOfStuffLabel".Translate(new object[]
					{
						this.stuffToUse.LabelAsStuff,
						label
					});
				}
				return label;
			}
		}

		protected override float WorkTotal
		{
			get
			{
				return this.def.entityDefToBuild.GetStatValueAbstract(StatDefOf.WorkToBuild, this.stuffToUse);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.LookDef<ThingDef>(ref this.stuffToUse, "stuffToUse");
		}

		public override ThingDef UIStuff()
		{
			return this.stuffToUse;
		}

		public override List<ThingCountClass> MaterialsNeeded()
		{
			return this.def.entityDefToBuild.CostListAdjusted(this.stuffToUse, true);
		}

		protected override Thing MakeSolidThing()
		{
			return ThingMaker.MakeThing(this.def.entityDefToBuild.frameDef, this.stuffToUse);
		}

		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			Blueprint_Build.<GetGizmos>c__Iterator129 <GetGizmos>c__Iterator = new Blueprint_Build.<GetGizmos>c__Iterator129();
			<GetGizmos>c__Iterator.<>f__this = this;
			Blueprint_Build.<GetGizmos>c__Iterator129 expr_0E = <GetGizmos>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(base.GetInspectString());
			stringBuilder.AppendLine("ContainedResources".Translate() + ":");
			foreach (ThingCountClass current in this.MaterialsNeeded())
			{
				stringBuilder.AppendLine(current.thingDef.LabelCap + ": 0 / " + current.count);
			}
			return stringBuilder.ToString();
		}
	}
}
