using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Verse
{
	public static class GenRecipe
	{
		[DebuggerHidden]
		public static IEnumerable<Thing> MakeRecipeProducts(RecipeDef recipeDef, Pawn worker, List<Thing> ingredients, Thing dominantIngredient)
		{
			GenRecipe.<MakeRecipeProducts>c__Iterator21D <MakeRecipeProducts>c__Iterator21D = new GenRecipe.<MakeRecipeProducts>c__Iterator21D();
			<MakeRecipeProducts>c__Iterator21D.recipeDef = recipeDef;
			<MakeRecipeProducts>c__Iterator21D.worker = worker;
			<MakeRecipeProducts>c__Iterator21D.dominantIngredient = dominantIngredient;
			<MakeRecipeProducts>c__Iterator21D.ingredients = ingredients;
			<MakeRecipeProducts>c__Iterator21D.<$>recipeDef = recipeDef;
			<MakeRecipeProducts>c__Iterator21D.<$>worker = worker;
			<MakeRecipeProducts>c__Iterator21D.<$>dominantIngredient = dominantIngredient;
			<MakeRecipeProducts>c__Iterator21D.<$>ingredients = ingredients;
			GenRecipe.<MakeRecipeProducts>c__Iterator21D expr_3F = <MakeRecipeProducts>c__Iterator21D;
			expr_3F.$PC = -2;
			return expr_3F;
		}

		private static Thing PostProcessProduct(Thing product, RecipeDef recipeDef, Pawn worker)
		{
			CompQuality compQuality = product.TryGetComp<CompQuality>();
			if (compQuality != null)
			{
				if (recipeDef.workSkill == null)
				{
					Log.Error(recipeDef + " needs workSkill because it creates a product with a quality.");
				}
				int level = worker.skills.GetSkill(recipeDef.workSkill).Level;
				compQuality.SetQuality(QualityUtility.RandomCreationQuality(level), ArtGenerationContext.Colony);
			}
			CompArt compArt = product.TryGetComp<CompArt>();
			if (compArt != null)
			{
				compArt.JustCreatedBy(worker);
				if (compQuality.Quality >= QualityCategory.Excellent)
				{
					TaleRecorder.RecordTale(TaleDefOf.CraftedArt, new object[]
					{
						worker,
						product
					});
				}
			}
			if (product.def.Minifiable)
			{
				product = product.MakeMinified();
			}
			return product;
		}
	}
}
