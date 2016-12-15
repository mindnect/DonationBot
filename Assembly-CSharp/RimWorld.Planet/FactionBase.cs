using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;

namespace RimWorld.Planet
{
	[StaticConstructorOnStartup]
	public class FactionBase : MapParent, ITrader
	{
		private string nameInt;

		public FactionBase_TraderTracker trader;

		public bool namedByPlayer;

		private Material cachedMat;

		public string Name
		{
			get
			{
				return this.nameInt;
			}
			set
			{
				this.nameInt = value;
			}
		}

		public override Material Material
		{
			get
			{
				if (this.cachedMat == null)
				{
					this.cachedMat = MaterialPool.MatFrom(base.Faction.def.homeIconPath, ShaderDatabase.WorldOverlayTransparentLit, base.Faction.Color);
				}
				return this.cachedMat;
			}
		}

		public override Texture2D ExpandingIcon
		{
			get
			{
				return base.Faction.def.ExpandingIconTexture;
			}
		}

		public override string Label
		{
			get
			{
				return (this.nameInt == null) ? base.Label : this.nameInt;
			}
		}

		protected override bool UseGenericEnterMapFloatMenuOption
		{
			get
			{
				return base.Faction.IsPlayer;
			}
		}

		public TraderKindDef TraderKind
		{
			get
			{
				return this.trader.TraderKind;
			}
		}

		public IEnumerable<Thing> Goods
		{
			get
			{
				return this.trader.Stock;
			}
		}

		public int RandomPriceFactorSeed
		{
			get
			{
				return this.trader.RandomPriceFactorSeed;
			}
		}

		public string TraderName
		{
			get
			{
				return this.trader.TraderName;
			}
		}

		public bool CanTradeNow
		{
			get
			{
				return this.trader.CanTradeNow;
			}
		}

		public float TradePriceImprovementOffsetForPlayer
		{
			get
			{
				return this.trader.TradePriceImprovementOffsetForPlayer;
			}
		}

		public FactionBase()
		{
			this.trader = new FactionBase_TraderTracker(this);
		}

		public IEnumerable<Thing> ColonyThingsWillingToBuy(Pawn playerNegotiator)
		{
			return this.trader.ColonyThingsWillingToBuy(playerNegotiator);
		}

		public void AddToStock(Thing thing, Pawn playerNegotiator)
		{
			this.trader.AddToStock(thing, playerNegotiator);
		}

		public void GiveSoldThingToPlayer(Thing toGive, Thing originalThingFromStock, Pawn playerNegotiator)
		{
			this.trader.GiveSoldThingToPlayer(toGive, originalThingFromStock, playerNegotiator);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<string>(ref this.nameInt, "nameInt", null, false);
			Scribe_Deep.LookDeep<FactionBase_TraderTracker>(ref this.trader, "trader", new object[]
			{
				this
			});
			Scribe_Values.LookValue<bool>(ref this.namedByPlayer, "namedByPlayer", false, false);
		}

		public override void Tick()
		{
			base.Tick();
			this.trader.TraderTrackerTick();
			FactionBaseDefeatUtility.CheckDefeated(this);
		}

		public override bool ShouldRemoveMapNow(out bool alsoRemoveWorldObject)
		{
			alsoRemoveWorldObject = false;
			return !base.Map.IsPlayerHome && !base.Map.mapPawns.AnyColonistTameAnimalOrPrisonerOfColony;
		}

		public override void PostRemove()
		{
			base.PostRemove();
			this.trader.DestroyStock();
		}

		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			FactionBase.<GetGizmos>c__IteratorF1 <GetGizmos>c__IteratorF = new FactionBase.<GetGizmos>c__IteratorF1();
			<GetGizmos>c__IteratorF.<>f__this = this;
			FactionBase.<GetGizmos>c__IteratorF1 expr_0E = <GetGizmos>c__IteratorF;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		[DebuggerHidden]
		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
		{
			FactionBase.<GetFloatMenuOptions>c__IteratorF2 <GetFloatMenuOptions>c__IteratorF = new FactionBase.<GetFloatMenuOptions>c__IteratorF2();
			<GetFloatMenuOptions>c__IteratorF.caravan = caravan;
			<GetFloatMenuOptions>c__IteratorF.<$>caravan = caravan;
			<GetFloatMenuOptions>c__IteratorF.<>f__this = this;
			FactionBase.<GetFloatMenuOptions>c__IteratorF2 expr_1C = <GetFloatMenuOptions>c__IteratorF;
			expr_1C.$PC = -2;
			return expr_1C;
		}
	}
}
