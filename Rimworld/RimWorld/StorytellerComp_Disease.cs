using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RimWorld
{
	public class StorytellerComp_Disease : StorytellerComp
	{
		[DebuggerHidden]
		public override IEnumerable<FiringIncident> MakeIntervalIncidents(IIncidentTarget target)
		{
			StorytellerComp_Disease.<MakeIntervalIncidents>c__IteratorA2 <MakeIntervalIncidents>c__IteratorA = new StorytellerComp_Disease.<MakeIntervalIncidents>c__IteratorA2();
			<MakeIntervalIncidents>c__IteratorA.target = target;
			<MakeIntervalIncidents>c__IteratorA.<$>target = target;
			<MakeIntervalIncidents>c__IteratorA.<>f__this = this;
			StorytellerComp_Disease.<MakeIntervalIncidents>c__IteratorA2 expr_1C = <MakeIntervalIncidents>c__IteratorA;
			expr_1C.$PC = -2;
			return expr_1C;
		}
	}
}
