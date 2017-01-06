using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Verse.Sound
{
	public class AudioGrain_Clip : AudioGrain
	{
		public string clipPath = string.Empty;

		[DebuggerHidden]
		public override IEnumerable<ResolvedGrain> GetResolvedGrains()
		{
			AudioGrain_Clip.<GetResolvedGrains>c__Iterator1B7 <GetResolvedGrains>c__Iterator1B = new AudioGrain_Clip.<GetResolvedGrains>c__Iterator1B7();
			<GetResolvedGrains>c__Iterator1B.<>f__this = this;
			AudioGrain_Clip.<GetResolvedGrains>c__Iterator1B7 expr_0E = <GetResolvedGrains>c__Iterator1B;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
