using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RimWorld
{
	public class ScenPart_DisableIncident : ScenPart_IncidentBase
	{
		protected override string IncidentTag
		{
			get
			{
				return "DisableIncident";
			}
		}

		[DebuggerHidden]
		protected override IEnumerable<IncidentDef> RandomizableIncidents()
		{
			ScenPart_DisableIncident.<RandomizableIncidents>c__Iterator100 <RandomizableIncidents>c__Iterator = new ScenPart_DisableIncident.<RandomizableIncidents>c__Iterator100();
			ScenPart_DisableIncident.<RandomizableIncidents>c__Iterator100 expr_07 = <RandomizableIncidents>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}
	}
}
