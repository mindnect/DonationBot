using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public class Recipe_InstallImplant : Recipe_Surgery
	{
		[DebuggerHidden]
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			Recipe_InstallImplant.<GetPartsToApplyOn>c__IteratorB8 <GetPartsToApplyOn>c__IteratorB = new Recipe_InstallImplant.<GetPartsToApplyOn>c__IteratorB8();
			<GetPartsToApplyOn>c__IteratorB.recipe = recipe;
			<GetPartsToApplyOn>c__IteratorB.pawn = pawn;
			<GetPartsToApplyOn>c__IteratorB.<$>recipe = recipe;
			<GetPartsToApplyOn>c__IteratorB.<$>pawn = pawn;
			Recipe_InstallImplant.<GetPartsToApplyOn>c__IteratorB8 expr_23 = <GetPartsToApplyOn>c__IteratorB;
			expr_23.$PC = -2;
			return expr_23;
		}

		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients)
		{
			if (billDoer != null)
			{
				if (base.CheckSurgeryFail(billDoer, pawn, ingredients, part))
				{
					return;
				}
				TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[]
				{
					billDoer,
					pawn
				});
			}
			pawn.health.AddHediff(this.recipe.addsHediff, part, null);
		}
	}
}
