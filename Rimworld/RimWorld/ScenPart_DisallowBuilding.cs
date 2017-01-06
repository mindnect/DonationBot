using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class ScenPart_DisallowBuilding : ScenPart_Rule
	{
		private const string DisallowBuildingTag = "DisallowBuilding";

		private ThingDef building;

		protected override void ApplyRule()
		{
			Current.Game.Rules.SetAllowBuilding(this.building, false);
		}

		public override string Summary(Scenario scen)
		{
			return ScenSummaryList.SummaryWithList(scen, "DisallowBuilding", "ScenPart_DisallowBuilding".Translate());
		}

		[DebuggerHidden]
		public override IEnumerable<string> GetSummaryListEntries(string tag)
		{
			ScenPart_DisallowBuilding.<GetSummaryListEntries>c__IteratorFD <GetSummaryListEntries>c__IteratorFD = new ScenPart_DisallowBuilding.<GetSummaryListEntries>c__IteratorFD();
			<GetSummaryListEntries>c__IteratorFD.tag = tag;
			<GetSummaryListEntries>c__IteratorFD.<$>tag = tag;
			<GetSummaryListEntries>c__IteratorFD.<>f__this = this;
			ScenPart_DisallowBuilding.<GetSummaryListEntries>c__IteratorFD expr_1C = <GetSummaryListEntries>c__IteratorFD;
			expr_1C.$PC = -2;
			return expr_1C;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.LookDef<ThingDef>(ref this.building, "building");
		}

		public override void Randomize()
		{
			this.building = this.RandomizableBuildingDefs().RandomElement<ThingDef>();
		}

		public override void DoEditInterface(Listing_ScenEdit listing)
		{
			Rect scenPartRect = listing.GetScenPartRect(this, ScenPart.RowHeight);
			if (Widgets.ButtonText(scenPartRect, this.building.LabelCap, true, false, true))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				foreach (ThingDef current in from t in this.PossibleBuildingDefs()
				orderby t.label
				select t)
				{
					ThingDef localTd = current;
					list.Add(new FloatMenuOption(localTd.LabelCap, delegate
					{
						this.building = localTd;
					}, MenuOptionPriority.Default, null, null, 0f, null, null));
				}
				Find.WindowStack.Add(new FloatMenu(list));
			}
		}

		public override bool TryMerge(ScenPart other)
		{
			ScenPart_DisallowBuilding scenPart_DisallowBuilding = other as ScenPart_DisallowBuilding;
			return scenPart_DisallowBuilding != null && scenPart_DisallowBuilding.building == this.building;
		}

		protected virtual IEnumerable<ThingDef> PossibleBuildingDefs()
		{
			return from d in DefDatabase<ThingDef>.AllDefs
			where d.category == ThingCategory.Building && d.designationCategory != null
			select d;
		}

		[DebuggerHidden]
		private IEnumerable<ThingDef> RandomizableBuildingDefs()
		{
			ScenPart_DisallowBuilding.<RandomizableBuildingDefs>c__IteratorFE <RandomizableBuildingDefs>c__IteratorFE = new ScenPart_DisallowBuilding.<RandomizableBuildingDefs>c__IteratorFE();
			ScenPart_DisallowBuilding.<RandomizableBuildingDefs>c__IteratorFE expr_07 = <RandomizableBuildingDefs>c__IteratorFE;
			expr_07.$PC = -2;
			return expr_07;
		}
	}
}
