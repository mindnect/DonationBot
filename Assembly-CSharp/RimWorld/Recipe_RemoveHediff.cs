using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public class Recipe_RemoveHediff : Recipe_Surgery
	{
		[DebuggerHidden]
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			Recipe_RemoveHediff.<GetPartsToApplyOn>c__IteratorBC <GetPartsToApplyOn>c__IteratorBC = new Recipe_RemoveHediff.<GetPartsToApplyOn>c__IteratorBC();
			<GetPartsToApplyOn>c__IteratorBC.pawn = pawn;
			<GetPartsToApplyOn>c__IteratorBC.recipe = recipe;
			<GetPartsToApplyOn>c__IteratorBC.<$>pawn = pawn;
			<GetPartsToApplyOn>c__IteratorBC.<$>recipe = recipe;
			Recipe_RemoveHediff.<GetPartsToApplyOn>c__IteratorBC expr_23 = <GetPartsToApplyOn>c__IteratorBC;
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
				if (PawnUtility.ShouldSendNotificationAbout(pawn) || PawnUtility.ShouldSendNotificationAbout(billDoer))
				{
					string text;
					if (!this.recipe.successfullyRemovedHediffMessage.NullOrEmpty())
					{
						text = string.Format(this.recipe.successfullyRemovedHediffMessage, billDoer.LabelShort, pawn.LabelShort);
					}
					else
					{
						text = "MessageSuccessfullyRemovedHediff".Translate(new object[]
						{
							billDoer.LabelShort,
							pawn.LabelShort,
							this.recipe.removesHediff.label
						});
					}
					Messages.Message(text, pawn, MessageSound.Benefit);
				}
			}
			Hediff hediff = pawn.health.hediffSet.hediffs.Find((Hediff x) => x.def == this.recipe.removesHediff && x.Part == part);
			pawn.health.RemoveHediff(hediff);
		}
	}
}
