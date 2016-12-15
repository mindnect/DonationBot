using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Verse
{
	public class CompProperties
	{
		public Type compClass;

		public CompProperties()
		{
		}

		public CompProperties(Type compClass)
		{
			this.compClass = compClass;
		}

		[DebuggerHidden]
		public virtual IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			CompProperties.<ConfigErrors>c__Iterator75 <ConfigErrors>c__Iterator = new CompProperties.<ConfigErrors>c__Iterator75();
			<ConfigErrors>c__Iterator.parentDef = parentDef;
			<ConfigErrors>c__Iterator.<$>parentDef = parentDef;
			<ConfigErrors>c__Iterator.<>f__this = this;
			CompProperties.<ConfigErrors>c__Iterator75 expr_1C = <ConfigErrors>c__Iterator;
			expr_1C.$PC = -2;
			return expr_1C;
		}

		public virtual void ResolveReferences(ThingDef parentDef)
		{
		}

		[DebuggerHidden]
		public virtual IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			CompProperties.<SpecialDisplayStats>c__Iterator76 <SpecialDisplayStats>c__Iterator = new CompProperties.<SpecialDisplayStats>c__Iterator76();
			CompProperties.<SpecialDisplayStats>c__Iterator76 expr_07 = <SpecialDisplayStats>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}
	}
}
