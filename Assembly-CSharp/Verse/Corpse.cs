using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Verse
{
	public class Corpse : ThingWithComps, IBillGiver, IThoughtGiver, IStrippable, IThingContainerOwner
	{
		private const int VanishAfterTicks = 3000000;

		private ThingContainer innerContainer;

		private int timeOfDeath = -1000;

		private int vanishAfterTimestamp = -1000;

		private BillStack operationsBillStack;

		public bool everBuriedInSarcophagus;

		public Pawn InnerPawn
		{
			get
			{
				if (this.innerContainer.Count > 0)
				{
					return (Pawn)this.innerContainer[0];
				}
				return null;
			}
			set
			{
				if (this.innerContainer.Count > 0)
				{
					Log.Error("Setting InnerPawn in corpse that already has one.");
					this.innerContainer.Clear();
				}
				this.innerContainer.TryAdd(value, true);
			}
		}

		public int Age
		{
			get
			{
				return Find.TickManager.TicksGame - this.timeOfDeath;
			}
			set
			{
				this.timeOfDeath = Find.TickManager.TicksGame - value;
			}
		}

		public override string Label
		{
			get
			{
				return "DeadLabel".Translate(new object[]
				{
					this.InnerPawn.LabelCap
				});
			}
		}

		public override bool IngestibleNow
		{
			get
			{
				if (this.Bugged)
				{
					Log.Error("IngestibleNow on Corpse while Bugged.");
					return false;
				}
				return this.InnerPawn.RaceProps.IsFlesh && this.GetRotStage() != RotStage.Dessicated;
			}
		}

		public RotDrawMode CurRotDrawMode
		{
			get
			{
				CompRottable comp = base.GetComp<CompRottable>();
				if (comp != null)
				{
					if (comp.Stage == RotStage.Rotting)
					{
						return RotDrawMode.Rotting;
					}
					if (comp.Stage == RotStage.Dessicated)
					{
						return RotDrawMode.Dessicated;
					}
				}
				return RotDrawMode.Fresh;
			}
		}

		private bool ShouldVanish
		{
			get
			{
				return this.InnerPawn.RaceProps.Animal && this.vanishAfterTimestamp > 0 && this.Age >= this.vanishAfterTimestamp && this.holdingContainer == null && this.GetRoom().TouchesMapEdge && !base.Map.roofGrid.Roofed(base.Position);
			}
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats
		{
			get
			{
				Corpse.<>c__Iterator1FD <>c__Iterator1FD = new Corpse.<>c__Iterator1FD();
				<>c__Iterator1FD.<>f__this = this;
				Corpse.<>c__Iterator1FD expr_0E = <>c__Iterator1FD;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}

		public BillStack BillStack
		{
			get
			{
				return this.operationsBillStack;
			}
		}

		public IEnumerable<IntVec3> IngredientStackCells
		{
			get
			{
				Corpse.<>c__Iterator1FE <>c__Iterator1FE = new Corpse.<>c__Iterator1FE();
				<>c__Iterator1FE.<>f__this = this;
				Corpse.<>c__Iterator1FE expr_0E = <>c__Iterator1FE;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}

		public bool Bugged
		{
			get
			{
				return this.innerContainer.Count == 0;
			}
		}

		public Corpse()
		{
			this.operationsBillStack = new BillStack(this);
			this.innerContainer = new ThingContainer(this, true, LookMode.Reference);
		}

		public bool CurrentlyUsable()
		{
			return this.InteractionCell.IsValid;
		}

		public bool AnythingToStrip()
		{
			return this.InnerPawn.AnythingToStrip();
		}

		public ThingContainer GetInnerContainer()
		{
			return this.innerContainer;
		}

		public IntVec3 GetPosition()
		{
			return base.PositionHeld;
		}

		public Map GetMap()
		{
			return base.MapHeld;
		}

		public override void SpawnSetup(Map map)
		{
			if (this.Bugged)
			{
				Log.Error(this + " spawned in bugged state. Null innerPawn");
				return;
			}
			base.SpawnSetup(map);
			if (this.timeOfDeath < 0)
			{
				this.timeOfDeath = Find.TickManager.TicksGame;
			}
			this.InnerPawn.Rotation = Rot4.South;
			this.NotifyColonistBar();
		}

		public override void DeSpawn()
		{
			base.DeSpawn();
			if (!this.Bugged)
			{
				this.NotifyColonistBar();
			}
		}

		public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
		{
			Pawn pawn = null;
			if (!this.Bugged)
			{
				pawn = this.InnerPawn;
				this.NotifyColonistBar();
				if (pawn.ownership != null)
				{
					pawn.ownership.UnclaimAll();
				}
				if (pawn.equipment != null)
				{
					pawn.equipment.DestroyAllEquipment(DestroyMode.Vanish);
				}
				pawn.inventory.DestroyAll(DestroyMode.Vanish);
				if (pawn.apparel != null)
				{
					pawn.apparel.DestroyAll(DestroyMode.Vanish);
				}
				this.innerContainer.Clear();
			}
			base.Destroy(mode);
			if (pawn != null)
			{
				Find.WorldPawns.DiscardIfUnimportant(pawn);
			}
		}

		public override void TickRare()
		{
			base.TickRare();
			if (this.Bugged)
			{
				Log.Error(this + " has null innerPawn. Destroying.");
				this.Destroy(DestroyMode.Vanish);
				return;
			}
			this.InnerPawn.TickRare();
			if (this.vanishAfterTimestamp < 0 || this.GetRotStage() != RotStage.Dessicated)
			{
				this.vanishAfterTimestamp = this.Age + 3000000;
			}
			if (this.ShouldVanish)
			{
				this.Destroy(DestroyMode.Vanish);
			}
		}

		protected override void IngestedCalculateAmounts(Pawn ingester, float nutritionWanted, out int numTaken, out float nutritionIngested)
		{
			BodyPartRecord bodyPartRecord = this.GetBestBodyPartToEat(ingester, nutritionWanted);
			if (bodyPartRecord == null)
			{
				Log.Error(string.Concat(new object[]
				{
					ingester,
					" ate ",
					this,
					" but no body part was found. Replacing with core part."
				}));
				bodyPartRecord = this.InnerPawn.RaceProps.body.corePart;
			}
			float bodyPartNutrition = FoodUtility.GetBodyPartNutrition(this.InnerPawn, bodyPartRecord);
			if (bodyPartRecord == this.InnerPawn.RaceProps.body.corePart)
			{
				if (PawnUtility.ShouldSendNotificationAbout(this.InnerPawn) && this.InnerPawn.RaceProps.Humanlike)
				{
					Messages.Message("MessageEatenByPredator".Translate(new object[]
					{
						this.InnerPawn.LabelShort,
						ingester.LabelIndefinite()
					}).CapitalizeFirst(), ingester, MessageSound.Negative);
				}
				numTaken = 1;
			}
			else
			{
				Hediff_MissingPart hediff_MissingPart = (Hediff_MissingPart)HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, this.InnerPawn, bodyPartRecord);
				hediff_MissingPart.lastInjury = HediffDefOf.Bite;
				hediff_MissingPart.IsFresh = true;
				this.InnerPawn.health.AddHediff(hediff_MissingPart, null, null);
				numTaken = 0;
			}
			if (ingester.RaceProps.Humanlike && Rand.Value < 0.05f)
			{
				FoodUtility.AddFoodPoisoningHediff(ingester, this);
			}
			nutritionIngested = bodyPartNutrition;
		}

		[DebuggerHidden]
		public override IEnumerable<Thing> ButcherProducts(Pawn butcher, float efficiency)
		{
			Corpse.<ButcherProducts>c__Iterator1FF <ButcherProducts>c__Iterator1FF = new Corpse.<ButcherProducts>c__Iterator1FF();
			<ButcherProducts>c__Iterator1FF.butcher = butcher;
			<ButcherProducts>c__Iterator1FF.efficiency = efficiency;
			<ButcherProducts>c__Iterator1FF.<$>butcher = butcher;
			<ButcherProducts>c__Iterator1FF.<$>efficiency = efficiency;
			<ButcherProducts>c__Iterator1FF.<>f__this = this;
			Corpse.<ButcherProducts>c__Iterator1FF expr_2A = <ButcherProducts>c__Iterator1FF;
			expr_2A.$PC = -2;
			return expr_2A;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<int>(ref this.timeOfDeath, "timeOfDeath", 0, false);
			Scribe_Values.LookValue<int>(ref this.vanishAfterTimestamp, "vanishAfterTimestamp", 0, false);
			Scribe_Values.LookValue<bool>(ref this.everBuriedInSarcophagus, "everBuriedInSarcophagus", false, false);
			Scribe_Deep.LookDeep<BillStack>(ref this.operationsBillStack, "operationsBillStack", new object[]
			{
				this
			});
			Scribe_Deep.LookDeep<ThingContainer>(ref this.innerContainer, "innerContainer", new object[]
			{
				this
			});
		}

		public void Strip()
		{
			if (this.InnerPawn.equipment != null)
			{
				this.InnerPawn.equipment.DropAllEquipment(base.PositionHeld, false);
			}
			if (this.InnerPawn.apparel != null)
			{
				this.InnerPawn.apparel.DropAll(base.PositionHeld, false);
			}
			if (this.InnerPawn.inventory != null)
			{
				this.InnerPawn.inventory.DropAllNearPawn(base.PositionHeld, false, false);
			}
		}

		public override void DrawAt(Vector3 drawLoc)
		{
			Building building = this.StoringBuilding();
			if (building != null && building.def == ThingDefOf.Grave)
			{
				return;
			}
			this.InnerPawn.Drawer.renderer.RenderPawnAt(drawLoc, this.CurRotDrawMode);
		}

		public Thought_Memory GiveObservedThought()
		{
			if (!this.InnerPawn.RaceProps.Humanlike)
			{
				return null;
			}
			if (this.StoringBuilding() == null)
			{
				Thought_MemoryObservation thought_MemoryObservation;
				if (this.IsNotFresh())
				{
					thought_MemoryObservation = (Thought_MemoryObservation)ThoughtMaker.MakeThought(ThoughtDefOf.ObservedLayingRottingCorpse);
				}
				else
				{
					thought_MemoryObservation = (Thought_MemoryObservation)ThoughtMaker.MakeThought(ThoughtDefOf.ObservedLayingCorpse);
				}
				thought_MemoryObservation.Target = this;
				return thought_MemoryObservation;
			}
			return null;
		}

		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.InnerPawn.Faction != null)
			{
				stringBuilder.AppendLine("Faction".Translate() + ": " + this.InnerPawn.Faction);
			}
			stringBuilder.AppendLine("DeadTime".Translate(new object[]
			{
				this.Age.ToStringTicksToPeriod(false)
			}));
			float num = 1f - this.InnerPawn.health.hediffSet.GetCoverageOfNotMissingNaturalParts(this.InnerPawn.RaceProps.body.corePart);
			if (num != 0f)
			{
				stringBuilder.AppendLine("CorpsePercentMissing".Translate() + ": " + num.ToStringPercent());
			}
			stringBuilder.AppendLine(base.GetInspectString());
			return stringBuilder.ToString();
		}

		public void RotStageChanged()
		{
			PortraitsCache.SetDirty(this.InnerPawn);
			this.NotifyColonistBar();
		}

		private BodyPartRecord GetBestBodyPartToEat(Pawn ingester, float nutritionWanted)
		{
			IEnumerable<BodyPartRecord> source = from x in this.InnerPawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined)
			where x.depth == BodyPartDepth.Outside && FoodUtility.GetBodyPartNutrition(this.InnerPawn, x) > 0.001f
			select x;
			if (!source.Any<BodyPartRecord>())
			{
				return null;
			}
			return source.MinBy((BodyPartRecord x) => Mathf.Abs(FoodUtility.GetBodyPartNutrition(this.InnerPawn, x) - nutritionWanted));
		}

		private void NotifyColonistBar()
		{
			if (this.InnerPawn.Faction == Faction.OfPlayer && Current.ProgramState == ProgramState.Playing)
			{
				Find.ColonistBar.MarkColonistsDirty();
			}
		}

		virtual bool get_Spawned()
		{
			return base.Spawned;
		}

		virtual Map get_Map()
		{
			return base.Map;
		}
	}
}
