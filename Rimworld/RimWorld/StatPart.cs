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
			StatPart.<ConfigErrors>c__Iterator18B <ConfigErrors>c__Iterator18B = new StatPart.<ConfigErrors>c__Iterator18B();
			StatPart.<ConfigErrors>c__Iterator18B expr_07 = <ConfigErrors>c__Iterator18B;
			expr_07.$PC = -2;
			return expr_07;
		}
	}
}
