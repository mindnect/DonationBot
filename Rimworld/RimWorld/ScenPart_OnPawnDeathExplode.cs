using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class ScenPart_OnPawnDeathExplode : ScenPart
	{
		private float radius = 5.9f;

		private DamageDef damage;

		private string radiusBuf;

		public override void Randomize()
		{
			this.radius = (float)Rand.RangeInclusive(3, 8) - 0.1f;
			this.damage = this.PossibleDamageDefs().RandomElement<DamageDef>();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<float>(ref this.radius, "radius", 0f, false);
			Scribe_Defs.LookDef<DamageDef>(ref this.damage, "damage");
		}

		public override string Summary(Scenario scen)
		{
			return "ScenPart_OnPawnDeathExplode".Translate(new object[]
			{
				this.damage.label,
				this.radius.ToString()
			});
		}

		public override void DoEditInterface(Listing_ScenEdit listing)
		{
			Rect scenPartRect = listing.GetScenPartRect(this, ScenPart.RowHeight * 2f);
			Widgets.TextFieldNumericLabeled<float>(scenPartRect.TopHalf(), "radius".Translate(), ref this.radius, ref this.radiusBuf, 0f, 1E+09f);
			if (Widgets.ButtonText(scenPartRect.BottomHalf(), this.damage.LabelCap, true, false, true))
			{
				FloatMenuUtility.MakeMenu<DamageDef>(this.PossibleDamageDefs(), (DamageDef d) => d.LabelCap, (DamageDef d) => delegate
				{
					this.damage = d;
				});
			}
		}

		public override void Notify_PawnDied(Corpse corpse)
		{
			if (corpse.Spawned)
			{
				GenExplosion.DoExplosion(corpse.Position, corpse.Map, this.radius, this.damage, null, null, null, null, null, 0f, 1, false, null, 0f, 1);
			}
		}

		[DebuggerHidden]
		private IEnumerable<DamageDef> PossibleDamageDefs()
		{
			ScenPart_OnPawnDeathExplode.<PossibleDamageDefs>c__IteratorFC <PossibleDamageDefs>c__IteratorFC = new ScenPart_OnPawnDeathExplode.<PossibleDamageDefs>c__IteratorFC();
			ScenPart_OnPawnDeathExplode.<PossibleDamageDefs>c__IteratorFC expr_07 = <PossibleDamageDefs>c__IteratorFC;
			expr_07.$PC = -2;
			return expr_07;
		}
	}
}
