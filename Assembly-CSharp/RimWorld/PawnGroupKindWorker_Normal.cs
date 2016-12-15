using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;

namespace RimWorld
{
	public class PawnGroupKindWorker_Normal : PawnGroupKindWorker
	{
		public override float MinPointsToGenerateAnything(PawnGroupMaker groupMaker)
		{
			return groupMaker.options.Min((PawnGenOption g) => g.Cost);
		}

		public override bool CanGenerateFrom(PawnGroupMakerParms parms, PawnGroupMaker groupMaker)
		{
			return base.CanGenerateFrom(parms, groupMaker) && PawnGroupMakerUtility.ChoosePawnGenOptionsByPoints(parms.points, groupMaker.options, parms).Any<PawnGenOption>();
		}

		[DebuggerHidden]
		public override IEnumerable<Pawn> GeneratePawns(PawnGroupMakerParms parms, PawnGroupMaker groupMaker, bool errorOnZeroResults = true)
		{
			PawnGroupKindWorker_Normal.<GeneratePawns>c__IteratorBF <GeneratePawns>c__IteratorBF = new PawnGroupKindWorker_Normal.<GeneratePawns>c__IteratorBF();
			<GeneratePawns>c__IteratorBF.parms = parms;
			<GeneratePawns>c__IteratorBF.groupMaker = groupMaker;
			<GeneratePawns>c__IteratorBF.errorOnZeroResults = errorOnZeroResults;
			<GeneratePawns>c__IteratorBF.<$>parms = parms;
			<GeneratePawns>c__IteratorBF.<$>groupMaker = groupMaker;
			<GeneratePawns>c__IteratorBF.<$>errorOnZeroResults = errorOnZeroResults;
			<GeneratePawns>c__IteratorBF.<>f__this = this;
			PawnGroupKindWorker_Normal.<GeneratePawns>c__IteratorBF expr_38 = <GeneratePawns>c__IteratorBF;
			expr_38.$PC = -2;
			return expr_38;
		}
	}
}
