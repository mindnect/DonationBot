using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Verse
{
	public class Editable
	{
		public virtual void ResolveReferences()
		{
		}

		public virtual void PostLoad()
		{
		}

		[DebuggerHidden]
		public virtual IEnumerable<string> ConfigErrors()
		{
			Editable.<ConfigErrors>c__Iterator84 <ConfigErrors>c__Iterator = new Editable.<ConfigErrors>c__Iterator84();
			Editable.<ConfigErrors>c__Iterator84 expr_07 = <ConfigErrors>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}
	}
}
