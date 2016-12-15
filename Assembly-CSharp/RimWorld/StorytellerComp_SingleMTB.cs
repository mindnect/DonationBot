using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RimWorld
{
	public class StorytellerComp_SingleMTB : StorytellerComp
	{
		private StorytellerCompProperties_SingleMTB Props
		{
			get
			{
				return (StorytellerCompProperties_SingleMTB)this.props;
			}
		}

		[DebuggerHidden]
		public override IEnumerable<FiringIncident> MakeIntervalIncidents(IIncidentTarget target)
		{
			StorytellerComp_SingleMTB.<MakeIntervalIncidents>c__IteratorA6 <MakeIntervalIncidents>c__IteratorA = new StorytellerComp_SingleMTB.<MakeIntervalIncidents>c__IteratorA6();
			<MakeIntervalIncidents>c__IteratorA.target = target;
			<MakeIntervalIncidents>c__IteratorA.<$>target = target;
			<MakeIntervalIncidents>c__IteratorA.<>f__this = this;
			StorytellerComp_SingleMTB.<MakeIntervalIncidents>c__IteratorA6 expr_1C = <MakeIntervalIncidents>c__IteratorA;
			expr_1C.$PC = -2;
			return expr_1C;
		}
	}
}
