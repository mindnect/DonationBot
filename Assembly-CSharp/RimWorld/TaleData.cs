using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.Grammar;

namespace RimWorld
{
	public abstract class TaleData : IExposable
	{
		public abstract void ExposeData();

		[DebuggerHidden]
		public virtual IEnumerable<Rule> GetRules(string prefix)
		{
			TaleData.<GetRules>c__Iterator10F <GetRules>c__Iterator10F = new TaleData.<GetRules>c__Iterator10F();
			<GetRules>c__Iterator10F.<>f__this = this;
			TaleData.<GetRules>c__Iterator10F expr_0E = <GetRules>c__Iterator10F;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		[DebuggerHidden]
		public virtual IEnumerable<Rule> GetRules()
		{
			TaleData.<GetRules>c__Iterator110 <GetRules>c__Iterator = new TaleData.<GetRules>c__Iterator110();
			<GetRules>c__Iterator.<>f__this = this;
			TaleData.<GetRules>c__Iterator110 expr_0E = <GetRules>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
