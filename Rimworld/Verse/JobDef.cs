using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Verse
{
	public class JobDef : Def
	{
		public Type driverClass;

		[MustTranslate]
		public string reportString = "Doing something.";

		public bool playerInterruptible = true;

		public bool canCheckOverrideOnDamage = true;

		public bool alwaysShowWeapon;

		public bool neverShowWeapon;

		public bool suspendable = true;

		public bool casualInterruptible = true;

		public bool collideWithPawns;

		public bool makeTargetPrisoner;

		public int joyDuration = 4000;

		public int joyMaxParticipants = 1;

		public float joyGainRate = 1f;

		public SkillDef joySkill;

		public float joyXpPerTick;

		public JoyKindDef joyKind;

		public Rot4 faceDir = Rot4.Invalid;

		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			JobDef.<ConfigErrors>c__Iterator1AD <ConfigErrors>c__Iterator1AD = new JobDef.<ConfigErrors>c__Iterator1AD();
			<ConfigErrors>c__Iterator1AD.<>f__this = this;
			JobDef.<ConfigErrors>c__Iterator1AD expr_0E = <ConfigErrors>c__Iterator1AD;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
