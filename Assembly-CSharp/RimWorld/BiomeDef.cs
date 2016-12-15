using RimWorld.Planet;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class BiomeDef : Def
	{
		public Type workerClass;

		public bool implemented = true;

		public bool canBuildBase = true;

		public float animalDensity;

		public float plantDensity;

		public float diseaseMtbDays = 60f;

		public float factionBaseSelectionWeight = 1f;

		public bool impassable;

		public SimpleCurve pathCost;

		public List<WeatherCommonalityRecord> baseWeatherCommonalities = new List<WeatherCommonalityRecord>();

		public List<TerrainThreshold> terrainsByFertility = new List<TerrainThreshold>();

		public List<SoundDef> soundsAmbient = new List<SoundDef>();

		public List<TerrainPatchMaker> terrainPatchMakers = new List<TerrainPatchMaker>();

		private List<BiomePlantRecord> wildPlants = new List<BiomePlantRecord>();

		private List<BiomeAnimalRecord> wildAnimals = new List<BiomeAnimalRecord>();

		private List<BiomeDiseaseRecord> diseases = new List<BiomeDiseaseRecord>();

		private List<ThingDef> allowedPackAnimals = new List<ThingDef>();

		public bool hideTerrain;

		public string texture;

		[Unsaved]
		private Dictionary<PawnKindDef, float> cachedAnimalCommonalities;

		[Unsaved]
		private Dictionary<ThingDef, float> cachedPlantCommonalities;

		[Unsaved]
		private Dictionary<IncidentDef, float> cachedDiseaseCommonalities;

		[Unsaved]
		private Material cachedMat;

		[Unsaved]
		private BiomeWorker workerInt;

		public BiomeWorker Worker
		{
			get
			{
				if (this.workerInt == null)
				{
					this.workerInt = (BiomeWorker)Activator.CreateInstance(this.workerClass);
				}
				return this.workerInt;
			}
		}

		public Material DrawMaterial
		{
			get
			{
				if (this.cachedMat == null)
				{
					if (this.texture.NullOrEmpty())
					{
						return null;
					}
					if (this == BiomeDefOf.Ocean)
					{
						this.cachedMat = new Material(WorldMaterials.WorldOcean);
					}
					else
					{
						this.cachedMat = new Material(WorldMaterials.WorldTerrain);
					}
					this.cachedMat.mainTexture = ContentFinder<Texture2D>.Get(this.texture, true);
				}
				return this.cachedMat;
			}
		}

		public IEnumerable<ThingDef> AllWildPlants
		{
			get
			{
				BiomeDef.<>c__Iterator87 <>c__Iterator = new BiomeDef.<>c__Iterator87();
				<>c__Iterator.<>f__this = this;
				BiomeDef.<>c__Iterator87 expr_0E = <>c__Iterator;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}

		public IEnumerable<PawnKindDef> AllWildAnimals
		{
			get
			{
				BiomeDef.<>c__Iterator88 <>c__Iterator = new BiomeDef.<>c__Iterator88();
				<>c__Iterator.<>f__this = this;
				BiomeDef.<>c__Iterator88 expr_0E = <>c__Iterator;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}

		public float CommonalityOfAnimal(PawnKindDef animalDef)
		{
			if (this.cachedAnimalCommonalities == null)
			{
				this.cachedAnimalCommonalities = new Dictionary<PawnKindDef, float>();
				for (int i = 0; i < this.wildAnimals.Count; i++)
				{
					this.cachedAnimalCommonalities.Add(this.wildAnimals[i].animal, this.wildAnimals[i].commonality);
				}
				foreach (PawnKindDef current in DefDatabase<PawnKindDef>.AllDefs)
				{
					if (current.RaceProps.wildBiomes != null)
					{
						for (int j = 0; j < current.RaceProps.wildBiomes.Count; j++)
						{
							if (current.RaceProps.wildBiomes[j].biome == this)
							{
								this.cachedAnimalCommonalities.Add(current, current.RaceProps.wildBiomes[j].commonality);
							}
						}
					}
				}
			}
			float result;
			if (this.cachedAnimalCommonalities.TryGetValue(animalDef, out result))
			{
				return result;
			}
			return 0f;
		}

		public float CommonalityOfPlant(ThingDef plantDef)
		{
			if (this.cachedPlantCommonalities == null)
			{
				this.cachedPlantCommonalities = new Dictionary<ThingDef, float>();
				for (int i = 0; i < this.wildPlants.Count; i++)
				{
					this.cachedPlantCommonalities.Add(this.wildPlants[i].plant, this.wildPlants[i].commonality);
				}
				foreach (ThingDef current in DefDatabase<ThingDef>.AllDefs)
				{
					if (current.plant != null && current.plant.wildBiomes != null)
					{
						for (int j = 0; j < current.plant.wildBiomes.Count; j++)
						{
							if (current.plant.wildBiomes[j].biome == this)
							{
								this.cachedPlantCommonalities.Add(current, current.plant.wildBiomes[j].commonality);
							}
						}
					}
				}
			}
			float result;
			if (this.cachedPlantCommonalities.TryGetValue(plantDef, out result))
			{
				return result;
			}
			return 0f;
		}

		public float CommonalityOfDisease(IncidentDef diseaseInc)
		{
			if (this.cachedDiseaseCommonalities == null)
			{
				this.cachedDiseaseCommonalities = new Dictionary<IncidentDef, float>();
				for (int i = 0; i < this.diseases.Count; i++)
				{
					this.cachedDiseaseCommonalities.Add(this.diseases[i].diseaseInc, this.diseases[i].commonality);
				}
				foreach (IncidentDef current in DefDatabase<IncidentDef>.AllDefs)
				{
					if (current.diseaseBiomeRecords != null)
					{
						for (int j = 0; j < current.diseaseBiomeRecords.Count; j++)
						{
							if (current.diseaseBiomeRecords[j].biome == this)
							{
								this.cachedDiseaseCommonalities.Add(current.diseaseBiomeRecords[j].diseaseInc, current.diseaseBiomeRecords[j].commonality);
							}
						}
					}
				}
			}
			float result;
			if (this.cachedDiseaseCommonalities.TryGetValue(diseaseInc, out result))
			{
				return result;
			}
			return 0f;
		}

		public bool IsPackAnimalAllowed(ThingDef pawn)
		{
			return this.allowedPackAnimals.Contains(pawn);
		}

		public static BiomeDef Named(string defName)
		{
			return DefDatabase<BiomeDef>.GetNamed(defName, true);
		}
	}
}
