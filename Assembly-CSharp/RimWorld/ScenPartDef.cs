using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public class ScenPartDef : Def
	{
		public ScenPartCategory category;

		public Type scenPartClass;

		public float summaryPriority = -1f;

		public float selectionWeight = 1f;

		public int maxUses = 999999;

		public Type pageClass;

		public MapConditionDef mapCondition;

		public FloatRange durationRandomRange = new FloatRange(30f, 100f);

		public Type designatorType;

		public bool PlayerAddRemovable
		{
			get
			{
				return this.category != ScenPartCategory.Fixed;
			}
		}

		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			ScenPartDef.<ConfigErrors>c__Iterator8E <ConfigErrors>c__Iterator8E = new ScenPartDef.<ConfigErrors>c__Iterator8E();
			<ConfigErrors>c__Iterator8E.<>f__this = this;
			ScenPartDef.<ConfigErrors>c__Iterator8E expr_0E = <ConfigErrors>c__Iterator8E;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
