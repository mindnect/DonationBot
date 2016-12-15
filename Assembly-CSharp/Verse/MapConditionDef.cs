using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Verse
{
	public class MapConditionDef : Def
	{
		public Type conditionClass = typeof(MapCondition);

		private List<MapConditionDef> exclusiveConditions;

		[MustTranslate]
		public string endMessage;

		public bool canBePermanent;

		public PsychicDroneLevel droneLevel = PsychicDroneLevel.BadMedium;

		public bool preventRain;

		public bool CanCoexistWith(MapConditionDef other)
		{
			return this != other && (this.exclusiveConditions == null || !this.exclusiveConditions.Contains(other));
		}

		public static MapConditionDef Named(string defName)
		{
			return DefDatabase<MapConditionDef>.GetNamed(defName, true);
		}

		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			MapConditionDef.<ConfigErrors>c__Iterator1AD <ConfigErrors>c__Iterator1AD = new MapConditionDef.<ConfigErrors>c__Iterator1AD();
			<ConfigErrors>c__Iterator1AD.<>f__this = this;
			MapConditionDef.<ConfigErrors>c__Iterator1AD expr_0E = <ConfigErrors>c__Iterator1AD;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
