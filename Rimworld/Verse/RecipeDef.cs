using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Verse
{
	public class RecipeDef : Def
	{
		public Type workerClass = typeof(RecipeWorker);

		public Type workerCounterClass = typeof(RecipeWorkerCounter);

		[MustTranslate]
		public string jobString = "Doing an unknown recipe.";

		public WorkTypeDef requiredGiverWorkType;

		public float workAmount = -1f;

		public StatDef workSpeedStat;

		public StatDef efficiencyStat;

		public List<IngredientCount> ingredients = new List<IngredientCount>();

		public ThingFilter fixedIngredientFilter = new ThingFilter();

		public ThingFilter defaultIngredientFilter;

		public bool allowMixingIngredients;

		private Type ingredientValueGetterClass = typeof(IngredientValueGetter_Volume);

		public List<SpecialThingFilterDef> forceHiddenSpecialFilters;

		public List<ThingCountClass> products = new List<ThingCountClass>();

		public List<SpecialProductType> specialProducts;

		public bool productHasIngredientStuff;

		public int targetCountAdjustment = 1;

		public ThingDef unfinishedThingDef;

		public List<SkillRequirement> skillRequirements;

		public SkillDef workSkill;

		public float workSkillLearnFactor = 1f;

		public EffecterDef effectWorking;

		public SoundDef soundWorking;

		public List<ThingDef> recipeUsers;

		public List<BodyPartDef> appliedOnFixedBodyParts = new List<BodyPartDef>();

		public HediffDef addsHediff;

		public HediffDef removesHediff;

		public bool hideBodyPartNames;

		public bool isViolation;

		[MustTranslate]
		public string successfullyRemovedHediffMessage;

		public float surgeonSurgerySuccessChanceExponent = 1f;

		public float roomSurgerySuccessChanceFactorExponent = 1f;

		public float surgerySuccessChanceFactor = 1f;

		public float deathOnFailedSurgeryChance;

		public bool targetsBodyPart = true;

		public bool anesthesize = true;

		public ResearchProjectDef researchPrerequisite;

		public ConceptDef conceptLearned;

		[Unsaved]
		private RecipeWorker workerInt;

		[Unsaved]
		private RecipeWorkerCounter workerCounterInt;

		[Unsaved]
		private IngredientValueGetter ingredientValueGetterInt;

		public RecipeWorker Worker
		{
			get
			{
				if (this.workerInt == null)
				{
					this.workerInt = (RecipeWorker)Activator.CreateInstance(this.workerClass);
					this.workerInt.recipe = this;
				}
				return this.workerInt;
			}
		}

		public RecipeWorkerCounter WorkerCounter
		{
			get
			{
				if (this.workerCounterInt == null)
				{
					this.workerCounterInt = (RecipeWorkerCounter)Activator.CreateInstance(this.workerCounterClass);
					this.workerCounterInt.recipe = this;
				}
				return this.workerCounterInt;
			}
		}

		public IngredientValueGetter IngredientValueGetter
		{
			get
			{
				if (this.ingredientValueGetterInt == null)
				{
					this.ingredientValueGetterInt = (IngredientValueGetter)Activator.CreateInstance(this.ingredientValueGetterClass);
				}
				return this.ingredientValueGetterInt;
			}
		}

		public bool AvailableNow
		{
			get
			{
				return this.researchPrerequisite == null || this.researchPrerequisite.IsFinished;
			}
		}

		public string MinSkillString
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = false;
				if (this.skillRequirements != null)
				{
					for (int i = 0; i < this.skillRequirements.Count; i++)
					{
						SkillRequirement skillRequirement = this.skillRequirements[i];
						stringBuilder.AppendLine(string.Concat(new object[]
						{
							"   ",
							skillRequirement.skill.skillLabel,
							": ",
							skillRequirement.minLevel
						}));
						flag = true;
					}
				}
				if (!flag)
				{
					stringBuilder.AppendLine("   (" + "NoneLower".Translate() + ")");
				}
				return stringBuilder.ToString();
			}
		}

		public IEnumerable<ThingDef> AllRecipeUsers
		{
			get
			{
				RecipeDef.<>c__Iterator1B2 <>c__Iterator1B = new RecipeDef.<>c__Iterator1B2();
				<>c__Iterator1B.<>f__this = this;
				RecipeDef.<>c__Iterator1B2 expr_0E = <>c__Iterator1B;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}

		public bool UsesUnfinishedThing
		{
			get
			{
				return this.unfinishedThingDef != null;
			}
		}

		public float WorkAmountTotal(ThingDef stuffDef)
		{
			if (this.workAmount >= 0f)
			{
				return this.workAmount;
			}
			return this.products[0].thingDef.GetStatValueAbstract(StatDefOf.WorkToMake, stuffDef);
		}

		[DebuggerHidden]
		public IEnumerable<ThingDef> PotentiallyMissingIngredients(Pawn billDoer, Map map)
		{
			RecipeDef.<PotentiallyMissingIngredients>c__Iterator1B3 <PotentiallyMissingIngredients>c__Iterator1B = new RecipeDef.<PotentiallyMissingIngredients>c__Iterator1B3();
			<PotentiallyMissingIngredients>c__Iterator1B.map = map;
			<PotentiallyMissingIngredients>c__Iterator1B.billDoer = billDoer;
			<PotentiallyMissingIngredients>c__Iterator1B.<$>map = map;
			<PotentiallyMissingIngredients>c__Iterator1B.<$>billDoer = billDoer;
			<PotentiallyMissingIngredients>c__Iterator1B.<>f__this = this;
			RecipeDef.<PotentiallyMissingIngredients>c__Iterator1B3 expr_2A = <PotentiallyMissingIngredients>c__Iterator1B;
			expr_2A.$PC = -2;
			return expr_2A;
		}

		public bool IsIngredient(ThingDef th)
		{
			if (!this.fixedIngredientFilter.Allows(th))
			{
				return false;
			}
			for (int i = 0; i < this.ingredients.Count; i++)
			{
				if (this.ingredients[i].filter.Allows(th))
				{
					return true;
				}
			}
			return false;
		}

		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			RecipeDef.<ConfigErrors>c__Iterator1B4 <ConfigErrors>c__Iterator1B = new RecipeDef.<ConfigErrors>c__Iterator1B4();
			<ConfigErrors>c__Iterator1B.<>f__this = this;
			RecipeDef.<ConfigErrors>c__Iterator1B4 expr_0E = <ConfigErrors>c__Iterator1B;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		public override void ResolveReferences()
		{
			base.ResolveReferences();
			for (int i = 0; i < this.ingredients.Count; i++)
			{
				this.ingredients[i].ResolveReferences();
			}
			if (this.fixedIngredientFilter != null)
			{
				this.fixedIngredientFilter.ResolveReferences();
			}
			if (this.defaultIngredientFilter == null)
			{
				this.defaultIngredientFilter = new ThingFilter();
				if (this.fixedIngredientFilter != null)
				{
					this.defaultIngredientFilter.CopyAllowancesFrom(this.fixedIngredientFilter);
				}
			}
			this.defaultIngredientFilter.ResolveReferences();
		}

		public bool PawnSatisfiesSkillRequirements(Pawn pawn)
		{
			return this.skillRequirements == null || !this.skillRequirements.Any((SkillRequirement req) => !req.PawnSatisfies(pawn));
		}
	}
}
