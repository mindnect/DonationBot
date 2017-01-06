using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RimWorld
{
	[StaticConstructorOnStartup]
	public class Building_TurretGun : Building_Turret
	{
		private Thing gunInt;

		protected TurretTop top;

		protected CompPowerTrader powerComp;

		protected CompMannable mannableComp;

		public bool loaded;

		private bool holdFire;

		protected LocalTargetInfo currentTargetInt = LocalTargetInfo.Invalid;

		protected int burstWarmupTicksLeft;

		protected int burstCooldownTicksLeft;

		public static Material ForcedTargetLineMat = MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, new Color(1f, 0.5f, 0.5f));

		public CompEquippable GunCompEq
		{
			get
			{
				return this.Gun.TryGetComp<CompEquippable>();
			}
		}

		public override LocalTargetInfo CurrentTarget
		{
			get
			{
				return this.currentTargetInt;
			}
		}

		private bool WarmingUp
		{
			get
			{
				return this.burstWarmupTicksLeft > 0;
			}
		}

		public Thing Gun
		{
			get
			{
				if (this.gunInt == null)
				{
					this.gunInt = ThingMaker.MakeThing(this.def.building.turretGunDef, null);
					for (int i = 0; i < this.GunCompEq.AllVerbs.Count; i++)
					{
						Verb verb = this.GunCompEq.AllVerbs[i];
						verb.caster = this;
						verb.castCompleteCallback = new Action(this.BurstComplete);
					}
				}
				return this.gunInt;
			}
		}

		public override Verb AttackVerb
		{
			get
			{
				return this.GunCompEq.verbTracker.PrimaryVerb;
			}
		}

		private bool MannedByColonist
		{
			get
			{
				return this.mannableComp != null && this.mannableComp.ManningPawn != null && this.mannableComp.ManningPawn.Faction == Faction.OfPlayer;
			}
		}

		private bool CanSetForcedTarget
		{
			get
			{
				return this.MannedByColonist;
			}
		}

		private bool CanToggleHoldFire
		{
			get
			{
				return base.Faction == Faction.OfPlayer || this.MannedByColonist;
			}
		}

		public Building_TurretGun()
		{
			this.top = new TurretTop(this);
		}

		public override void SpawnSetup(Map map)
		{
			base.SpawnSetup(map);
			this.powerComp = base.GetComp<CompPowerTrader>();
			this.mannableComp = base.GetComp<CompMannable>();
			this.currentTargetInt = LocalTargetInfo.Invalid;
			this.burstWarmupTicksLeft = 0;
			this.burstCooldownTicksLeft = 0;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<int>(ref this.burstCooldownTicksLeft, "burstCooldownTicksLeft", 0, false);
			Scribe_Values.LookValue<bool>(ref this.loaded, "loaded", false, false);
			Scribe_Values.LookValue<bool>(ref this.holdFire, "holdFire", false, false);
		}

		public override void OrderAttack(LocalTargetInfo targ)
		{
			if ((targ.Cell - base.Position).LengthHorizontal < this.GunCompEq.PrimaryVerb.verbProps.minRange)
			{
				Messages.Message("MessageTargetBelowMinimumRange".Translate(), this, MessageSound.RejectInput);
				return;
			}
			if ((targ.Cell - base.Position).LengthHorizontal > this.GunCompEq.PrimaryVerb.verbProps.range)
			{
				Messages.Message("MessageTargetBeyondMaximumRange".Translate(), this, MessageSound.RejectInput);
				return;
			}
			if (this.forcedTarget != targ)
			{
				this.forcedTarget = targ;
				if (this.burstCooldownTicksLeft <= 0)
				{
					this.TryStartShootSomething();
				}
			}
		}

		public override void Tick()
		{
			base.Tick();
			if (this.powerComp != null && !this.powerComp.PowerOn)
			{
				return;
			}
			if (this.mannableComp != null && !this.mannableComp.MannedNow)
			{
				return;
			}
			if (!this.CanSetForcedTarget && this.forcedTarget.IsValid)
			{
				this.ResetForcedTarget();
			}
			if (!this.CanToggleHoldFire)
			{
				this.holdFire = false;
			}
			this.GunCompEq.verbTracker.VerbsTick();
			if (this.stunner.Stunned)
			{
				return;
			}
			if (this.GunCompEq.PrimaryVerb.state == VerbState.Bursting)
			{
				return;
			}
			if (this.WarmingUp)
			{
				this.burstWarmupTicksLeft--;
				if (this.burstWarmupTicksLeft == 0)
				{
					this.BeginBurst();
				}
			}
			else
			{
				if (this.burstCooldownTicksLeft > 0)
				{
					this.burstCooldownTicksLeft--;
				}
				if (this.burstCooldownTicksLeft == 0)
				{
					this.TryStartShootSomething();
				}
			}
			this.top.TurretTopTick();
		}

		protected void TryStartShootSomething()
		{
			if (!base.Spawned)
			{
				return;
			}
			if (this.forcedTarget.ThingDestroyed)
			{
				this.forcedTarget = null;
			}
			if (this.holdFire && this.CanToggleHoldFire)
			{
				return;
			}
			if (this.GunCompEq.PrimaryVerb.verbProps.projectileDef.projectile.flyOverhead && base.Map.roofGrid.Roofed(base.Position))
			{
				return;
			}
			bool isValid = this.currentTargetInt.IsValid;
			if (this.forcedTarget.IsValid)
			{
				this.currentTargetInt = this.forcedTarget;
			}
			else
			{
				this.currentTargetInt = this.TryFindNewTarget();
			}
			if (!isValid && this.currentTargetInt.IsValid)
			{
				SoundDefOf.TurretAcquireTarget.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
			}
			if (this.currentTargetInt.IsValid)
			{
				if (this.def.building.turretBurstWarmupTime > 0f)
				{
					this.burstWarmupTicksLeft = this.def.building.turretBurstWarmupTime.SecondsToTicks();
				}
				else
				{
					this.BeginBurst();
				}
			}
		}

		protected LocalTargetInfo TryFindNewTarget()
		{
			Thing thing = this.TargSearcher();
			Faction faction = thing.Faction;
			float range = this.GunCompEq.PrimaryVerb.verbProps.range;
			float minRange = this.GunCompEq.PrimaryVerb.verbProps.minRange;
			Building t;
			if (Rand.Value < 0.5f && this.GunCompEq.PrimaryVerb.verbProps.projectileDef.projectile.flyOverhead && faction.HostileTo(Faction.OfPlayer) && base.Map.listerBuildings.allBuildingsColonist.Where(delegate(Building x)
			{
				float num = x.Position.DistanceToSquared(this.Position);
				return num > minRange * minRange && num < range * range;
			}).TryRandomElement(out t))
			{
				return t;
			}
			TargetScanFlags targetScanFlags = TargetScanFlags.NeedThreat;
			if (!this.GunCompEq.PrimaryVerb.verbProps.projectileDef.projectile.flyOverhead)
			{
				targetScanFlags |= TargetScanFlags.NeedLOSToAll;
			}
			if (this.GunCompEq.PrimaryVerb.verbProps.ai_IsIncendiary)
			{
				targetScanFlags |= TargetScanFlags.NeedNonBurning;
			}
			return AttackTargetFinder.BestShootTargetFromCurrentPosition(thing, new Predicate<Thing>(this.IsValidTarget), range, minRange, targetScanFlags);
		}

		private Thing TargSearcher()
		{
			if (this.mannableComp != null && this.mannableComp.MannedNow)
			{
				return this.mannableComp.ManningPawn;
			}
			return this;
		}

		private bool IsValidTarget(Thing t)
		{
			Pawn pawn = t as Pawn;
			if (pawn != null)
			{
				if (this.GunCompEq.PrimaryVerb.verbProps.projectileDef.projectile.flyOverhead)
				{
					RoofDef roofDef = base.Map.roofGrid.RoofAt(t.Position);
					if (roofDef != null && roofDef.isThickRoof)
					{
						return false;
					}
				}
				if (this.mannableComp == null)
				{
					return !GenAI.MachinesLike(base.Faction, pawn);
				}
				if (pawn.RaceProps.Animal && pawn.Faction == Faction.OfPlayer)
				{
					return false;
				}
			}
			return true;
		}

		protected void BeginBurst()
		{
			this.GunCompEq.PrimaryVerb.TryStartCastOn(this.CurrentTarget, false, true);
		}

		protected void BurstComplete()
		{
			if (this.def.building.turretBurstCooldownTime >= 0f)
			{
				this.burstCooldownTicksLeft = this.def.building.turretBurstCooldownTime.SecondsToTicks();
			}
			else
			{
				this.burstCooldownTicksLeft = this.GunCompEq.PrimaryVerb.verbProps.defaultCooldownTime.SecondsToTicks();
			}
			this.loaded = false;
		}

		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string inspectString = base.GetInspectString();
			if (!inspectString.NullOrEmpty())
			{
				stringBuilder.AppendLine(inspectString);
			}
			stringBuilder.AppendLine("GunInstalled".Translate() + ": " + this.Gun.Label);
			if (this.GunCompEq.PrimaryVerb.verbProps.minRange > 0f)
			{
				stringBuilder.AppendLine("MinimumRange".Translate() + ": " + this.GunCompEq.PrimaryVerb.verbProps.minRange.ToString("F0"));
			}
			if (this.burstCooldownTicksLeft > 0)
			{
				stringBuilder.AppendLine("CanFireIn".Translate() + ": " + this.burstCooldownTicksLeft.TickstoSecondsString());
			}
			if (this.def.building.turretShellDef != null)
			{
				if (this.loaded)
				{
					stringBuilder.AppendLine("ShellLoaded".Translate());
				}
				else
				{
					stringBuilder.AppendLine("ShellNotLoaded".Translate());
				}
			}
			return stringBuilder.ToString();
		}

		public override void Draw()
		{
			this.top.DrawTurret();
			base.Draw();
		}

		public override void DrawExtraSelectionOverlays()
		{
			float range = this.GunCompEq.PrimaryVerb.verbProps.range;
			if (range < 90f)
			{
				GenDraw.DrawRadiusRing(base.Position, range);
			}
			float minRange = this.GunCompEq.PrimaryVerb.verbProps.minRange;
			if (minRange < 90f && minRange > 0.1f)
			{
				GenDraw.DrawRadiusRing(base.Position, minRange);
			}
			if (this.burstWarmupTicksLeft > 0)
			{
				int degreesWide = (int)((float)this.burstWarmupTicksLeft * 0.5f);
				GenDraw.DrawAimPie(this, this.CurrentTarget, degreesWide, (float)this.def.size.x * 0.5f);
			}
			if (this.forcedTarget.IsValid && (!this.forcedTarget.HasThing || this.forcedTarget.Thing.Spawned))
			{
				Vector3 b;
				if (this.forcedTarget.HasThing)
				{
					b = this.forcedTarget.Thing.TrueCenter();
				}
				else
				{
					b = this.forcedTarget.Cell.ToVector3Shifted();
				}
				Vector3 a = this.TrueCenter();
				b.y = Altitudes.AltitudeFor(AltitudeLayer.MetaOverlays);
				a.y = b.y;
				GenDraw.DrawLineBetween(a, b, Building_TurretGun.ForcedTargetLineMat);
			}
		}

		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			Building_TurretGun.<GetGizmos>c__Iterator12E <GetGizmos>c__Iterator12E = new Building_TurretGun.<GetGizmos>c__Iterator12E();
			<GetGizmos>c__Iterator12E.<>f__this = this;
			Building_TurretGun.<GetGizmos>c__Iterator12E expr_0E = <GetGizmos>c__Iterator12E;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		private void ResetForcedTarget()
		{
			this.forcedTarget = LocalTargetInfo.Invalid;
			if (this.burstCooldownTicksLeft <= 0)
			{
				this.TryStartShootSomething();
			}
		}
	}
}
