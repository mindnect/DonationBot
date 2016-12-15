using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public static class ThingDefGenerator_Corpses
	{
		private const float DaysToStartRot = 2.5f;

		private const float DaysToDessicate = 5f;

		private const float RotDamagePerDay = 2.5f;

		private const float DessicatedDamagePerDay = 0.75f;

		[DebuggerHidden]
		public static IEnumerable<ThingDef> ImpliedCorpseDefs()
		{
			ThingDefGenerator_Corpses.<ImpliedCorpseDefs>c__Iterator70 <ImpliedCorpseDefs>c__Iterator = new ThingDefGenerator_Corpses.<ImpliedCorpseDefs>c__Iterator70();
			ThingDefGenerator_Corpses.<ImpliedCorpseDefs>c__Iterator70 expr_07 = <ImpliedCorpseDefs>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}
	}
}
