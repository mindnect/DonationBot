using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Verse
{
	public class HediffCompProperties
	{
		public Type compClass;

		[DebuggerHidden]
		public IEnumerable<string> ConfigErrors(HediffDef parentDef)
		{
			HediffCompProperties.<ConfigErrors>c__Iterator1A4 <ConfigErrors>c__Iterator1A = new HediffCompProperties.<ConfigErrors>c__Iterator1A4();
			<ConfigErrors>c__Iterator1A.parentDef = parentDef;
			<ConfigErrors>c__Iterator1A.<$>parentDef = parentDef;
			<ConfigErrors>c__Iterator1A.<>f__this = this;
			HediffCompProperties.<ConfigErrors>c__Iterator1A4 expr_1C = <ConfigErrors>c__Iterator1A;
			expr_1C.$PC = -2;
			return expr_1C;
		}
	}
}
