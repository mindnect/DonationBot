using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	internal class Recipe_Kill : RecipeWorker
	{
		[DebuggerHidden]
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			Recipe_Kill.<GetPartsToApplyOn>c__IteratorBA <GetPartsToApplyOn>c__IteratorBA = new Recipe_Kill.<GetPartsToApplyOn>c__IteratorBA();
			<GetPartsToApplyOn>c__IteratorBA.pawn = pawn;
			<GetPartsToApplyOn>c__IteratorBA.<$>pawn = pawn;
			Recipe_Kill.<GetPartsToApplyOn>c__IteratorBA expr_15 = <GetPartsToApplyOn>c__IteratorBA;
			expr_15.$PC = -2;
			return expr_15;
		}

		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients)
		{
			pawn.health.AddHediff(this.recipe.addsHediff, part, null);
			ThoughtUtility.GiveThoughtsForPawnExecuted(pawn, PawnExecutionKind.GenericHumane);
		}
	}
}
