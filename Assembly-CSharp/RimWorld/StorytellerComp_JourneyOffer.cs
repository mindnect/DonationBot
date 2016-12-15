using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public class StorytellerComp_JourneyOffer : StorytellerComp
	{
		private const int StartOnDay = 14;

		private int IntervalsPassed
		{
			get
			{
				return Find.TickManager.TicksGame / 1000;
			}
		}

		[DebuggerHidden]
		public override IEnumerable<FiringIncident> MakeIntervalIncidents(IIncidentTarget target)
		{
			StorytellerComp_JourneyOffer.<MakeIntervalIncidents>c__IteratorA3 <MakeIntervalIncidents>c__IteratorA = new StorytellerComp_JourneyOffer.<MakeIntervalIncidents>c__IteratorA3();
			<MakeIntervalIncidents>c__IteratorA.target = target;
			<MakeIntervalIncidents>c__IteratorA.<$>target = target;
			<MakeIntervalIncidents>c__IteratorA.<>f__this = this;
			StorytellerComp_JourneyOffer.<MakeIntervalIncidents>c__IteratorA3 expr_1C = <MakeIntervalIncidents>c__IteratorA;
			expr_1C.$PC = -2;
			return expr_1C;
		}
	}
}
