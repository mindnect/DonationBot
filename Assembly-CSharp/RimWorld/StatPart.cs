using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public abstract class StatPart
	{
		[Unsaved]
		public StatDef parentStat;

		public abstract void TransformValue(StatRequest req, ref float val);

		public abstract string ExplanationPart(StatRequest req);

		[DebuggerHidden]
		public virtual IEnumerable<string> ConfigErrors()
		{
			StatPart.<ConfigErrors>c__Iterator18A <ConfigErrors>c__Iterator18A = new StatPart.<ConfigErrors>c__Iterator18A();
			StatPart.<ConfigErrors>c__Iterator18A expr_07 = <ConfigErrors>c__Iterator18A;
			expr_07.$PC = -2;
			return expr_07;
		}
	}
}
