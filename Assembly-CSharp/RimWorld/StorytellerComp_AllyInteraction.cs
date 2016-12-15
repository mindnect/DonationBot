using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class StorytellerComp_AllyInteraction : StorytellerComp
	{
		private const int ForceChooseTraderAfterTicks = 780000;

		private StorytellerCompProperties_AllyInteraction Props
		{
			get
			{
				return (StorytellerCompProperties_AllyInteraction)this.props;
			}
		}

		private float IncidentMTBDays
		{
			get
			{
				List<Faction> allFactionsListForReading = Find.FactionManager.AllFactionsListForReading;
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < allFactionsListForReading.Count; i++)
				{
					if (!allFactionsListForReading[i].def.hidden && !allFactionsListForReading[i].IsPlayer)
					{
						if (allFactionsListForReading[i].def.CanEverBeNonHostile)
						{
							num2++;
						}
						if (!allFactionsListForReading[i].HostileTo(Faction.OfPlayer))
						{
							num++;
						}
					}
				}
				if (num == 0)
				{
					return -1f;
				}
				float num3 = (float)num / Mathf.Max((float)num2, 1f);
				return this.Props.baseMtb / num3;
			}
		}

		[DebuggerHidden]
		public override IEnumerable<FiringIncident> MakeIntervalIncidents(IIncidentTarget target)
		{
			StorytellerComp_AllyInteraction.<MakeIntervalIncidents>c__Iterator9E <MakeIntervalIncidents>c__Iterator9E = new StorytellerComp_AllyInteraction.<MakeIntervalIncidents>c__Iterator9E();
			<MakeIntervalIncidents>c__Iterator9E.target = target;
			<MakeIntervalIncidents>c__Iterator9E.<$>target = target;
			<MakeIntervalIncidents>c__Iterator9E.<>f__this = this;
			StorytellerComp_AllyInteraction.<MakeIntervalIncidents>c__Iterator9E expr_1C = <MakeIntervalIncidents>c__Iterator9E;
			expr_1C.$PC = -2;
			return expr_1C;
		}

		private bool TryChooseIncident(IIncidentTarget target, out IncidentDef result)
		{
			if (IncidentDefOf.TraderCaravanArrival.TargetAllowed(target))
			{
				int num = 0;
				if (!Find.StoryWatcher.storyState.lastFireTicks.TryGetValue(IncidentDefOf.TraderCaravanArrival, out num))
				{
					num = (int)(this.props.minDaysPassed * 60000f);
				}
				if (Find.TickManager.TicksGame > num + 780000)
				{
					result = IncidentDefOf.TraderCaravanArrival;
					return true;
				}
			}
			return (from d in DefDatabase<IncidentDef>.AllDefs
			where d.category == IncidentCategory.AllyArrival && d.TargetAllowed(target) && d.Worker.CanFireNow(target)
			select d).TryRandomElementByWeight((IncidentDef d) => d.baseChance, out result);
		}
	}
}
