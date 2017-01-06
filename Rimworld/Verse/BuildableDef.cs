using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Verse
{
	public abstract class BuildableDef : Def
	{
		public List<StatModifier> statBases;

		public Traversability passability;

		public int pathCost;

		public bool pathCostIgnoreRepeat = true;

		public float fertility = -1f;

		public List<ThingCountClass> costList;

		public int costStuffCount = -1;

		public List<StuffCategoryDef> stuffCategories;

		public TerrainAffordance terrainAffordanceNeeded = TerrainAffordance.Light;

		public List<ThingDef> buildingPrerequisites;

		public List<ResearchProjectDef> researchPrerequisites;

		public int placingDraggableDimensions;

		public EffecterDef repairEffect;

		public EffecterDef constructEffect;

		public Rot4 defaultPlacingRot = Rot4.North;

		[Unsaved]
		public ThingDef blueprintDef;

		[Unsaved]
		public ThingDef installBlueprintDef;

		[Unsaved]
		public ThingDef frameDef;

		public string uiIconPath;

		public AltitudeLayer altitudeLayer = AltitudeLayer.Item;

		[Unsaved]
		public Texture2D uiIcon = BaseContent.BadTex;

		[Unsaved]
		public Graphic graphic = BaseContent.BadGraphic;

		public bool menuHidden;

		public float specialDisplayRadius;

		public List<Type> placeWorkers;

		[NoTranslate]
		public DesignationCategoryDef designationCategory;

		public KeyBindingDef designationHotKey;

		[Unsaved]
		private List<PlaceWorker> placeWorkersInstantiatedInt;

		public virtual IntVec2 Size
		{
			get
			{
				return new IntVec2(1, 1);
			}
		}

		public bool MadeFromStuff
		{
			get
			{
				return !this.stuffCategories.NullOrEmpty<StuffCategoryDef>();
			}
		}

		public abstract Color IconDrawColor
		{
			get;
		}

		public Material DrawMatSingle
		{
			get
			{
				if (this.graphic == null)
				{
					return null;
				}
				return this.graphic.MatSingle;
			}
		}

		public float Altitude
		{
			get
			{
				return Altitudes.AltitudeFor(this.altitudeLayer);
			}
		}

		public List<PlaceWorker> PlaceWorkers
		{
			get
			{
				if (this.placeWorkers == null)
				{
					return null;
				}
				this.placeWorkersInstantiatedInt = new List<PlaceWorker>();
				foreach (Type current in this.placeWorkers)
				{
					this.placeWorkersInstantiatedInt.Add((PlaceWorker)Activator.CreateInstance(current));
				}
				return this.placeWorkersInstantiatedInt;
			}
		}

		public bool ForceAllowPlaceOver(BuildableDef other)
		{
			if (this.PlaceWorkers == null)
			{
				return false;
			}
			for (int i = 0; i < this.PlaceWorkers.Count; i++)
			{
				if (this.PlaceWorkers[i].ForceAllowPlaceOver(other))
				{
					return true;
				}
			}
			return false;
		}

		public override void PostLoad()
		{
			base.PostLoad();
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				if (!this.uiIconPath.NullOrEmpty())
				{
					this.uiIcon = ContentFinder<Texture2D>.Get(this.uiIconPath, true);
				}
				else if (this.DrawMatSingle != null && this.DrawMatSingle != BaseContent.BadMat)
				{
					this.uiIcon = (Texture2D)this.DrawMatSingle.mainTexture;
				}
			});
		}

		public override void ResolveReferences()
		{
			base.ResolveReferences();
		}

		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			BuildableDef.<ConfigErrors>c__Iterator1A3 <ConfigErrors>c__Iterator1A = new BuildableDef.<ConfigErrors>c__Iterator1A3();
			<ConfigErrors>c__Iterator1A.<>f__this = this;
			BuildableDef.<ConfigErrors>c__Iterator1A3 expr_0E = <ConfigErrors>c__Iterator1A;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		public override string ToString()
		{
			return this.defName;
		}

		public override int GetHashCode()
		{
			return this.defName.GetHashCode();
		}
	}
}
