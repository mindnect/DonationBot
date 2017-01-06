using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RimWorld
{
	public class StorytellerComp_CategoryIndividualMTBByBiome : StorytellerComp
	{
		protected StorytellerCompProperties_CategoryIndividualMTBByBiome Props
		{
			get
			{
				return (StorytellerCompProperties_CategoryIndividualMTBByBiome)this.props;
			}
		}

		[DebuggerHidden]
		public override IEnumerable<FiringIncident> MakeIntervalIncidents(IIncidentTarget target)
		{
			StorytellerComp_CategoryIndividualMTBByBiome.<MakeIntervalIncidents>c__Iterator9F <MakeIntervalIncidents>c__Iterator9F = new StorytellerComp_CategoryIndividualMTBByBiome.<MakeIntervalIncidents>c__Iterator9F();
			<MakeIntervalIncidents>c__Iterator9F.target = target;
			<MakeIntervalIncidents>c__Iterator9F.<$>target = target;
			<MakeIntervalIncidents>c__Iterator9F.<>f__this = this;
			StorytellerComp_CategoryIndividualMTBByBiome.<MakeIntervalIncidents>c__Iterator9F expr_1C = <MakeIntervalIncidents>c__Iterator9F;
			expr_1C.$PC = -2;
			return expr_1C;
		}
	}
}
