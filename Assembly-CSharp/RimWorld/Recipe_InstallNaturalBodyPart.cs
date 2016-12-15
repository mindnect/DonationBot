using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public class Recipe_InstallNaturalBodyPart : Recipe_Surgery
	{
		[DebuggerHidden]
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			Recipe_InstallNaturalBodyPart.<GetPartsToApplyOn>c__IteratorB9 <GetPartsToApplyOn>c__IteratorB = new Recipe_InstallNaturalBodyPart.<GetPartsToApplyOn>c__IteratorB9();
			<GetPartsToApplyOn>c__IteratorB.recipe = recipe;
			<GetPartsToApplyOn>c__IteratorB.pawn = pawn;
			<GetPartsToApplyOn>c__IteratorB.<$>recipe = recipe;
			<GetPartsToApplyOn>c__IteratorB.<$>pawn = pawn;
			Recipe_InstallNaturalBodyPart.<GetPartsToApplyOn>c__IteratorB9 expr_23 = <GetPartsToApplyOn>c__IteratorB;
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
				MedicalRecipesUtility.RestorePartAndSpawnAllPreviousParts(pawn, part, billDoer.Position, billDoer.Map);
			}
		}
	}
}
