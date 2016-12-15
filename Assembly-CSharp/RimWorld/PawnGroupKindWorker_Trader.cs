using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;

namespace RimWorld
{
	public class PawnGroupKindWorker_Trader : PawnGroupKindWorker
	{
		private const float GuardsPointsPerMarketValue = 0.015f;

		private const float MinGuardsPoints = 130f;

		private const float MaxGuardsPoints = 1700f;

		public override float MinPointsToGenerateAnything(PawnGroupMaker groupMaker)
		{
			return 0f;
		}

		public override bool CanGenerateFrom(PawnGroupMakerParms parms, PawnGroupMaker groupMaker)
		{
			return base.CanGenerateFrom(parms, groupMaker) && groupMaker.traders.Any<PawnGenOption>() && (parms.map == null || groupMaker.carriers.Any((PawnGenOption x) => parms.map.Biome.IsPackAnimalAllowed(x.kind.race)));
		}

		[DebuggerHidden]
		public override IEnumerable<Pawn> GeneratePawns(PawnGroupMakerParms parms, PawnGroupMaker groupMaker, bool errorOnZeroResults = true)
		{
			PawnGroupKindWorker_Trader.<GeneratePawns>c__IteratorC0 <GeneratePawns>c__IteratorC = new PawnGroupKindWorker_Trader.<GeneratePawns>c__IteratorC0();
			<GeneratePawns>c__IteratorC.parms = parms;
			<GeneratePawns>c__IteratorC.groupMaker = groupMaker;
			<GeneratePawns>c__IteratorC.errorOnZeroResults = errorOnZeroResults;
			<GeneratePawns>c__IteratorC.<$>parms = parms;
			<GeneratePawns>c__IteratorC.<$>groupMaker = groupMaker;
			<GeneratePawns>c__IteratorC.<$>errorOnZeroResults = errorOnZeroResults;
			<GeneratePawns>c__IteratorC.<>f__this = this;
			PawnGroupKindWorker_Trader.<GeneratePawns>c__IteratorC0 expr_38 = <GeneratePawns>c__IteratorC;
			expr_38.$PC = -2;
			return expr_38;
		}

		private Pawn GenerateTrader(PawnGroupMakerParms parms, PawnGroupMaker groupMaker, TraderKindDef traderKind)
		{
			Map map = parms.map;
			PawnGenerationRequest request = new PawnGenerationRequest(groupMaker.traders.RandomElementByWeight((PawnGenOption x) => (float)x.selectionWeight).kind, parms.faction, PawnGenerationContext.NonPlayer, map, false, false, false, false, true, false, 1f, false, true, true, null, null, null, null, null, null);
			Pawn pawn = PawnGenerator.GeneratePawn(request);
			pawn.mindState.wantsToTradeWithColony = true;
			PawnComponentsUtility.AddAndRemoveDynamicComponents(pawn, true);
			pawn.trader.traderKind = traderKind;
			this.PostGenerate(pawn);
			return pawn;
		}

		[DebuggerHidden]
		private IEnumerable<Pawn> GenerateCarriers(PawnGroupMakerParms parms, PawnGroupMaker groupMaker, Pawn trader, List<Thing> wares)
		{
			PawnGroupKindWorker_Trader.<GenerateCarriers>c__IteratorC1 <GenerateCarriers>c__IteratorC = new PawnGroupKindWorker_Trader.<GenerateCarriers>c__IteratorC1();
			<GenerateCarriers>c__IteratorC.wares = wares;
			<GenerateCarriers>c__IteratorC.groupMaker = groupMaker;
			<GenerateCarriers>c__IteratorC.parms = parms;
			<GenerateCarriers>c__IteratorC.<$>wares = wares;
			<GenerateCarriers>c__IteratorC.<$>groupMaker = groupMaker;
			<GenerateCarriers>c__IteratorC.<$>parms = parms;
			<GenerateCarriers>c__IteratorC.<>f__this = this;
			PawnGroupKindWorker_Trader.<GenerateCarriers>c__IteratorC1 expr_3A = <GenerateCarriers>c__IteratorC;
			expr_3A.$PC = -2;
			return expr_3A;
		}

		[DebuggerHidden]
		private IEnumerable<Pawn> GetSlavesAndAnimalsFromWares(PawnGroupMakerParms parms, Pawn trader, List<Thing> wares)
		{
			PawnGroupKindWorker_Trader.<GetSlavesAndAnimalsFromWares>c__IteratorC2 <GetSlavesAndAnimalsFromWares>c__IteratorC = new PawnGroupKindWorker_Trader.<GetSlavesAndAnimalsFromWares>c__IteratorC2();
			<GetSlavesAndAnimalsFromWares>c__IteratorC.wares = wares;
			<GetSlavesAndAnimalsFromWares>c__IteratorC.parms = parms;
			<GetSlavesAndAnimalsFromWares>c__IteratorC.<$>wares = wares;
			<GetSlavesAndAnimalsFromWares>c__IteratorC.<$>parms = parms;
			PawnGroupKindWorker_Trader.<GetSlavesAndAnimalsFromWares>c__IteratorC2 expr_23 = <GetSlavesAndAnimalsFromWares>c__IteratorC;
			expr_23.$PC = -2;
			return expr_23;
		}

		[DebuggerHidden]
		private IEnumerable<Pawn> GenerateGuards(PawnGroupMakerParms parms, PawnGroupMaker groupMaker, Pawn trader, List<Thing> wares)
		{
			PawnGroupKindWorker_Trader.<GenerateGuards>c__IteratorC3 <GenerateGuards>c__IteratorC = new PawnGroupKindWorker_Trader.<GenerateGuards>c__IteratorC3();
			<GenerateGuards>c__IteratorC.groupMaker = groupMaker;
			<GenerateGuards>c__IteratorC.wares = wares;
			<GenerateGuards>c__IteratorC.parms = parms;
			<GenerateGuards>c__IteratorC.<$>groupMaker = groupMaker;
			<GenerateGuards>c__IteratorC.<$>wares = wares;
			<GenerateGuards>c__IteratorC.<$>parms = parms;
			<GenerateGuards>c__IteratorC.<>f__this = this;
			PawnGroupKindWorker_Trader.<GenerateGuards>c__IteratorC3 expr_3A = <GenerateGuards>c__IteratorC;
			expr_3A.$PC = -2;
			return expr_3A;
		}

		private bool ValidateTradersAndCarriers(Faction faction, PawnGroupMaker groupMaker)
		{
			PawnGenOption pawnGenOption = groupMaker.traders.FirstOrDefault((PawnGenOption x) => !x.kind.trader);
			if (pawnGenOption != null)
			{
				Log.Error(string.Concat(new object[]
				{
					"Cannot generate arriving trader caravan for ",
					faction,
					" because there is a pawn kind (",
					pawnGenOption.kind.LabelCap,
					") who is not a trader but is in a traders list."
				}));
				return false;
			}
			PawnGenOption pawnGenOption2 = groupMaker.carriers.FirstOrDefault((PawnGenOption x) => !x.kind.RaceProps.packAnimal);
			if (pawnGenOption2 != null)
			{
				Log.Error(string.Concat(new object[]
				{
					"Cannot generate arriving trader caravan for ",
					faction,
					" because there is a pawn kind (",
					pawnGenOption2.kind.LabelCap,
					") who is not a carrier but is in a carriers list."
				}));
				return false;
			}
			return true;
		}
	}
}
