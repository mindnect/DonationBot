using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;

namespace RimWorld.Planet
{
	public class Caravan_TraderTracker : IExposable
	{
		private Caravan caravan;

		private List<Pawn> soldPrisoners = new List<Pawn>();

		public TraderKindDef TraderKind
		{
			get
			{
				List<Pawn> pawnsListForReading = this.caravan.PawnsListForReading;
				for (int i = 0; i < pawnsListForReading.Count; i++)
				{
					Pawn pawn = pawnsListForReading[i];
					if (this.caravan.IsOwner(pawn) && pawn.TraderKind != null)
					{
						return pawn.TraderKind;
					}
				}
				return null;
			}
		}

		public IEnumerable<Thing> Goods
		{
			get
			{
				Caravan_TraderTracker.<>c__IteratorEC <>c__IteratorEC = new Caravan_TraderTracker.<>c__IteratorEC();
				<>c__IteratorEC.<>f__this = this;
				Caravan_TraderTracker.<>c__IteratorEC expr_0E = <>c__IteratorEC;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}

		public int RandomPriceFactorSeed
		{
			get
			{
				return Gen.HashCombineInt(this.caravan.ID, 1048142365);
			}
		}

		public string TraderName
		{
			get
			{
				return this.caravan.LabelCap;
			}
		}

		public bool CanTradeNow
		{
			get
			{
				return this.TraderKind != null && !this.caravan.AllOwnersDowned && this.caravan.Faction != Faction.OfPlayer && this.Goods.Any((Thing x) => this.TraderKind.WillTrade(x.def));
			}
		}

		public Caravan_TraderTracker(Caravan caravan)
		{
			this.caravan = caravan;
		}

		public void ExposeData()
		{
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				this.soldPrisoners.RemoveAll((Pawn x) => x.Destroyed);
			}
			Scribe_Collections.LookList<Pawn>(ref this.soldPrisoners, "soldPrisoners", LookMode.Reference, new object[0]);
		}

		[DebuggerHidden]
		public IEnumerable<Thing> ColonyThingsWillingToBuy(Pawn playerNegotiator)
		{
			Caravan_TraderTracker.<ColonyThingsWillingToBuy>c__IteratorED <ColonyThingsWillingToBuy>c__IteratorED = new Caravan_TraderTracker.<ColonyThingsWillingToBuy>c__IteratorED();
			<ColonyThingsWillingToBuy>c__IteratorED.playerNegotiator = playerNegotiator;
			<ColonyThingsWillingToBuy>c__IteratorED.<$>playerNegotiator = playerNegotiator;
			Caravan_TraderTracker.<ColonyThingsWillingToBuy>c__IteratorED expr_15 = <ColonyThingsWillingToBuy>c__IteratorED;
			expr_15.$PC = -2;
			return expr_15;
		}

		public void AddToStock(Thing thing, Pawn playerNegotiator)
		{
			if (this.Goods.Contains(thing))
			{
				Log.Error("Tried to add " + thing + " to stock (pawn's trader tracker), but it's already here.");
				return;
			}
			Caravan caravan = playerNegotiator.GetCaravan();
			Pawn pawn = thing as Pawn;
			if (pawn != null)
			{
				CaravanInventoryUtility.MoveAllInventoryToSomeoneElse(pawn, caravan.PawnsListForReading, null);
				caravan.RemovePawn(pawn);
				this.caravan.AddPawn(pawn, false);
				Find.WorldPawns.RemovePawn(pawn);
				if (pawn.RaceProps.Humanlike)
				{
					this.soldPrisoners.Add(pawn);
				}
			}
			else
			{
				Pawn ownerOf = CaravanInventoryUtility.GetOwnerOf(caravan, thing);
				Pawn pawn2 = CaravanInventoryUtility.FindPawnToMoveInventoryTo(thing, this.caravan.PawnsListForReading, null, null);
				if (pawn2 == null)
				{
					Log.Error("Could not find pawn to move sold thing to (sold by player). thing=" + thing);
				}
				else if (ownerOf != null)
				{
					ownerOf.inventory.innerContainer.TransferToContainer(thing, pawn2.inventory.innerContainer, thing.stackCount);
				}
				else if (!pawn2.inventory.innerContainer.TryAdd(thing, true))
				{
					Log.Error("Could not add item to inventory.");
				}
			}
		}

		public void GiveSoldThingToPlayer(Thing toGive, Thing originalThingFromStock, Pawn playerNegotiator)
		{
			Caravan caravan = playerNegotiator.GetCaravan();
			Pawn pawn = toGive as Pawn;
			if (pawn != null)
			{
				CaravanInventoryUtility.MoveAllInventoryToSomeoneElse(pawn, this.caravan.PawnsListForReading, null);
				this.caravan.RemovePawn(pawn);
				caravan.AddPawn(pawn, true);
				Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
			}
			else
			{
				Pawn ownerOf = CaravanInventoryUtility.GetOwnerOf(this.caravan, toGive);
				Pawn pawn2 = CaravanInventoryUtility.FindPawnToMoveInventoryTo(toGive, caravan.PawnsListForReading, null, null);
				if (pawn2 == null)
				{
					Log.Error("Could not find pawn to move bought thing to (bought by player). thing=" + toGive);
				}
				else if (ownerOf != null)
				{
					ownerOf.inventory.innerContainer.TransferToContainer(toGive, pawn2.inventory.innerContainer, toGive.stackCount);
				}
				else if (!pawn2.inventory.innerContainer.TryAdd(toGive, true))
				{
					Log.Error("Could not add item to inventory.");
				}
			}
		}
	}
}
