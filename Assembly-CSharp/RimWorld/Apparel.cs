using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public class Apparel : ThingWithComps
	{
		public Pawn wearer;

		private bool wornByCorpseInt;

		public bool WornByCorpse
		{
			get
			{
				return this.wornByCorpseInt;
			}
		}

		public void Notify_Stripped(Pawn pawn)
		{
			if (pawn.Dead && this.def.apparel.careIfWornByCorpse)
			{
				this.wornByCorpseInt = true;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<bool>(ref this.wornByCorpseInt, "wornByCorpse", false, false);
		}

		public virtual void DrawWornExtras()
		{
		}

		public virtual bool CheckPreAbsorbDamage(DamageInfo dinfo)
		{
			return false;
		}

		public virtual bool AllowVerbCast(IntVec3 root, TargetInfo targ)
		{
			return true;
		}

		[DebuggerHidden]
		public virtual IEnumerable<Gizmo> GetWornGizmos()
		{
			Apparel.<GetWornGizmos>c__Iterator144 <GetWornGizmos>c__Iterator = new Apparel.<GetWornGizmos>c__Iterator144();
			Apparel.<GetWornGizmos>c__Iterator144 expr_07 = <GetWornGizmos>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}

		public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
		{
			base.Destroy(mode);
			if (base.Destroyed && this.wearer != null)
			{
				this.wearer.apparel.Notify_WornApparelDestroyed(this);
			}
		}

		public override string GetInspectString()
		{
			string text = base.GetInspectString();
			if (this.WornByCorpse)
			{
				text += "WasWornByCorpse".Translate();
			}
			return text;
		}

		public virtual float GetSpecialApparelScoreOffset()
		{
			return 0f;
		}
	}
}
