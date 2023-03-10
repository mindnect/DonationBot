using System;

namespace Verse
{
	public class Projectile_Explosive : Projectile
	{
		private int ticksToDetonation;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<int>(ref this.ticksToDetonation, "ticksToDetonation", 0, false);
		}

		public override void Tick()
		{
			base.Tick();
			if (this.ticksToDetonation > 0)
			{
				this.ticksToDetonation--;
				if (this.ticksToDetonation <= 0)
				{
					this.Explode();
				}
			}
		}

		protected override void Impact(Thing hitThing)
		{
			if (this.def.projectile.explosionDelay == 0)
			{
				this.Explode();
				return;
			}
			this.landed = true;
			this.ticksToDetonation = this.def.projectile.explosionDelay;
			GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this, this.def.projectile.damageDef, this.launcher.Faction);
		}

		protected virtual void Explode()
		{
			Map map = base.Map;
			this.Destroy(DestroyMode.Vanish);
			ThingDef preExplosionSpawnThingDef = this.def.projectile.preExplosionSpawnThingDef;
			float explosionSpawnChance = this.def.projectile.explosionSpawnChance;
			GenExplosion.DoExplosion(base.Position, map, this.def.projectile.explosionRadius, this.def.projectile.damageDef, this.launcher, this.def.projectile.soundExplode, this.def, this.equipmentDef, this.def.projectile.postExplosionSpawnThingDef, this.def.projectile.explosionSpawnChance, 1, false, preExplosionSpawnThingDef, explosionSpawnChance, 1);
		}
	}
}
