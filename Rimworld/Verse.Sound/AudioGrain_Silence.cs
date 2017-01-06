using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Verse.Sound
{
	public class AudioGrain_Silence : AudioGrain
	{
		[EditSliderRange(0f, 5f)]
		public FloatRange durationRange = new FloatRange(1f, 2f);

		[DebuggerHidden]
		public override IEnumerable<ResolvedGrain> GetResolvedGrains()
		{
			AudioGrain_Silence.<GetResolvedGrains>c__Iterator1B9 <GetResolvedGrains>c__Iterator1B = new AudioGrain_Silence.<GetResolvedGrains>c__Iterator1B9();
			<GetResolvedGrains>c__Iterator1B.<>f__this = this;
			AudioGrain_Silence.<GetResolvedGrains>c__Iterator1B9 expr_0E = <GetResolvedGrains>c__Iterator1B;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		public override int GetHashCode()
		{
			return this.durationRange.GetHashCode();
		}
	}
}
