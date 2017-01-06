using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	public class CompFlickable : ThingComp
	{
		public const string FlickedOnSignal = "FlickedOn";

		public const string FlickedOffSignal = "FlickedOff";

		private bool switchOnInt = true;

		private bool wantSwitchOn = true;

		public bool SwitchIsOn
		{
			get
			{
				return this.switchOnInt;
			}
			set
			{
				if (this.switchOnInt == value)
				{
					return;
				}
				this.switchOnInt = value;
				if (this.switchOnInt)
				{
					this.parent.BroadcastCompSignal("FlickedOn");
				}
				else
				{
					this.parent.BroadcastCompSignal("FlickedOff");
				}
			}
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.LookValue<bool>(ref this.switchOnInt, "switchOn", true, false);
			Scribe_Values.LookValue<bool>(ref this.wantSwitchOn, "wantSwitchOn", true, false);
		}

		public bool WantsFlick()
		{
			return this.wantSwitchOn != this.switchOnInt;
		}

		public void DoFlick()
		{
			this.SwitchIsOn = !this.SwitchIsOn;
			SoundDefOf.FlickSwitch.PlayOneShot(new TargetInfo(this.parent.Position, this.parent.Map, false));
		}

		public void ResetToOn()
		{
			this.switchOnInt = true;
			this.wantSwitchOn = true;
		}

		[DebuggerHidden]
		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			CompFlickable.<CompGetGizmosExtra>c__IteratorAB <CompGetGizmosExtra>c__IteratorAB = new CompFlickable.<CompGetGizmosExtra>c__IteratorAB();
			<CompGetGizmosExtra>c__IteratorAB.<>f__this = this;
			CompFlickable.<CompGetGizmosExtra>c__IteratorAB expr_0E = <CompGetGizmosExtra>c__IteratorAB;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
