using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class ScenPart_PermaMapCondition : ScenPart
	{
		public const string PermaMapConditionTag = "PermaMapCondition";

		private MapConditionDef mapCondition;

		public override string Label
		{
			get
			{
				return "Permanent".Translate().CapitalizeFirst() + ": " + this.mapCondition.label;
			}
		}

		public override void DoEditInterface(Listing_ScenEdit listing)
		{
			Rect scenPartRect = listing.GetScenPartRect(this, ScenPart.RowHeight);
			if (Widgets.ButtonText(scenPartRect, this.mapCondition.LabelCap, true, false, true))
			{
				FloatMenuUtility.MakeMenu<MapConditionDef>(this.AllowedMapConditions(), (MapConditionDef d) => d.LabelCap, (MapConditionDef d) => delegate
				{
					this.mapCondition = d;
				});
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.LookDef<MapConditionDef>(ref this.mapCondition, "mapCondition");
		}

		public override void Randomize()
		{
			this.mapCondition = this.AllowedMapConditions().RandomElement<MapConditionDef>();
		}

		private IEnumerable<MapConditionDef> AllowedMapConditions()
		{
			return from d in DefDatabase<MapConditionDef>.AllDefs
			where d.canBePermanent
			select d;
		}

		public override string Summary(Scenario scen)
		{
			return ScenSummaryList.SummaryWithList(scen, "PermaMapCondition", "ScenPart_PermaMapCondition".Translate());
		}

		[DebuggerHidden]
		public override IEnumerable<string> GetSummaryListEntries(string tag)
		{
			ScenPart_PermaMapCondition.<GetSummaryListEntries>c__IteratorFA <GetSummaryListEntries>c__IteratorFA = new ScenPart_PermaMapCondition.<GetSummaryListEntries>c__IteratorFA();
			<GetSummaryListEntries>c__IteratorFA.tag = tag;
			<GetSummaryListEntries>c__IteratorFA.<$>tag = tag;
			<GetSummaryListEntries>c__IteratorFA.<>f__this = this;
			ScenPart_PermaMapCondition.<GetSummaryListEntries>c__IteratorFA expr_1C = <GetSummaryListEntries>c__IteratorFA;
			expr_1C.$PC = -2;
			return expr_1C;
		}

		public override void GenerateIntoMap(Map map)
		{
			MapCondition cond = MapConditionMaker.MakeConditionPermanent(this.mapCondition);
			map.mapConditionManager.RegisterCondition(cond);
		}

		public override bool CanCoexistWith(ScenPart other)
		{
			if (this.mapCondition == null)
			{
				return true;
			}
			ScenPart_PermaMapCondition scenPart_PermaMapCondition = other as ScenPart_PermaMapCondition;
			return scenPart_PermaMapCondition == null || this.mapCondition.CanCoexistWith(scenPart_PermaMapCondition.mapCondition);
		}
	}
}
