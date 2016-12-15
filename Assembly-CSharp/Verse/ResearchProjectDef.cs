using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Verse
{
	public class ResearchProjectDef : Def
	{
		public TechLevel techLevel;

		[MustTranslate]
		private string descriptionDiscovered;

		public float baseCost = 100f;

		public List<ResearchProjectDef> prerequisites;

		public List<ResearchProjectDef> requiredByThis;

		private List<ResearchMod> researchMods;

		public ThingDef requiredResearchBuilding;

		public List<ThingDef> requiredResearchFacilities;

		[NoTranslate]
		public List<string> tags;

		public float researchViewX = 1f;

		public float researchViewY = 1f;

		private float x = 1f;

		private float y = 1f;

		public float ResearchViewX
		{
			get
			{
				return this.x;
			}
		}

		public float ResearchViewY
		{
			get
			{
				return this.y;
			}
		}

		public float CostApparent
		{
			get
			{
				return this.baseCost * this.CostFactor(Faction.OfPlayer.def.techLevel);
			}
		}

		public float ProgressReal
		{
			get
			{
				return Find.ResearchManager.GetProgress(this);
			}
		}

		public float ProgressApparent
		{
			get
			{
				return this.ProgressReal * this.CostFactor(Faction.OfPlayer.def.techLevel);
			}
		}

		public float ProgressPercent
		{
			get
			{
				return Find.ResearchManager.GetProgress(this) / this.baseCost;
			}
		}

		public bool IsFinished
		{
			get
			{
				return this.ProgressReal >= this.baseCost;
			}
		}

		public bool CanStartNow
		{
			get
			{
				return !this.IsFinished && this.PrerequisitesCompleted && (this.requiredResearchBuilding == null || this.PlayerHasAnyAppropriateResearchBench);
			}
		}

		public bool PrerequisitesCompleted
		{
			get
			{
				if (this.prerequisites != null)
				{
					for (int i = 0; i < this.prerequisites.Count; i++)
					{
						if (!this.prerequisites[i].IsFinished)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		public string DescriptionDiscovered
		{
			get
			{
				if (this.descriptionDiscovered != null)
				{
					return this.descriptionDiscovered;
				}
				return this.description;
			}
		}

		private bool PlayerHasAnyAppropriateResearchBench
		{
			get
			{
				List<Map> maps = Find.Maps;
				for (int i = 0; i < maps.Count; i++)
				{
					List<Building> allBuildingsColonist = maps[i].listerBuildings.allBuildingsColonist;
					for (int j = 0; j < allBuildingsColonist.Count; j++)
					{
						Building_ResearchBench building_ResearchBench = allBuildingsColonist[j] as Building_ResearchBench;
						if (building_ResearchBench != null && this.CanBeResearchedAt(building_ResearchBench, true))
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		public float CostFactor(TechLevel researcherTechLevel)
		{
			if (researcherTechLevel >= this.techLevel)
			{
				return 1f;
			}
			int num = (int)(this.techLevel - researcherTechLevel);
			return 1f + (float)num;
		}

		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			ResearchProjectDef.<ConfigErrors>c__Iterator1B4 <ConfigErrors>c__Iterator1B = new ResearchProjectDef.<ConfigErrors>c__Iterator1B4();
			<ConfigErrors>c__Iterator1B.<>f__this = this;
			ResearchProjectDef.<ConfigErrors>c__Iterator1B4 expr_0E = <ConfigErrors>c__Iterator1B;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		public bool HasTag(string tag)
		{
			return this.tags != null && this.tags.Contains(tag);
		}

		public bool CanBeResearchedAt(Building_ResearchBench bench, bool ignoreResearchBenchPowerStatus)
		{
			if (this.requiredResearchBuilding != null && bench.def != this.requiredResearchBuilding)
			{
				return false;
			}
			if (!ignoreResearchBenchPowerStatus)
			{
				CompPowerTrader comp = bench.GetComp<CompPowerTrader>();
				if (comp != null && !comp.PowerOn)
				{
					return false;
				}
			}
			if (!this.requiredResearchFacilities.NullOrEmpty<ThingDef>())
			{
				ResearchProjectDef.<CanBeResearchedAt>c__AnonStorey47A <CanBeResearchedAt>c__AnonStorey47A = new ResearchProjectDef.<CanBeResearchedAt>c__AnonStorey47A();
				<CanBeResearchedAt>c__AnonStorey47A.<>f__this = this;
				<CanBeResearchedAt>c__AnonStorey47A.affectedByFacilities = bench.TryGetComp<CompAffectedByFacilities>();
				if (<CanBeResearchedAt>c__AnonStorey47A.affectedByFacilities == null)
				{
					return false;
				}
				List<Thing> linkedFacilitiesListForReading = <CanBeResearchedAt>c__AnonStorey47A.affectedByFacilities.LinkedFacilitiesListForReading;
				int i;
				for (i = 0; i < this.requiredResearchFacilities.Count; i++)
				{
					if (linkedFacilitiesListForReading.Find((Thing x) => x.def == this.requiredResearchFacilities[i] && <CanBeResearchedAt>c__AnonStorey47A.affectedByFacilities.IsFacilityActive(x)) == null)
					{
						return false;
					}
				}
			}
			return true;
		}

		public void ReapplyAllMods()
		{
			if (this.researchMods != null)
			{
				for (int i = 0; i < this.researchMods.Count; i++)
				{
					try
					{
						this.researchMods[i].Apply();
					}
					catch (Exception ex)
					{
						Log.Error(string.Concat(new object[]
						{
							"Exception applying research mod for project ",
							this,
							": ",
							ex.ToString()
						}));
					}
				}
			}
		}

		public static ResearchProjectDef Named(string defName)
		{
			return DefDatabase<ResearchProjectDef>.GetNamed(defName, true);
		}

		public static void GenerateNonOverlappingCoordinates()
		{
			foreach (ResearchProjectDef current in DefDatabase<ResearchProjectDef>.AllDefsListForReading)
			{
				current.x = current.researchViewX;
				current.y = current.researchViewY;
			}
			int num = 0;
			while (true)
			{
				bool flag = false;
				foreach (ResearchProjectDef current2 in DefDatabase<ResearchProjectDef>.AllDefsListForReading)
				{
					foreach (ResearchProjectDef current3 in DefDatabase<ResearchProjectDef>.AllDefsListForReading)
					{
						if (current2 != current3)
						{
							bool flag2 = Mathf.Abs(current2.x - current3.x) < 0.8f;
							bool flag3 = Mathf.Abs(current2.y - current3.y) < 0.45f;
							if (flag2 && flag3)
							{
								flag = true;
								if (current2.x <= current3.x)
								{
									current2.x -= 0.1f;
									current3.x += 0.1f;
								}
								else
								{
									current2.x += 0.1f;
									current3.x -= 0.1f;
								}
								if (current2.y <= current3.y)
								{
									current2.y -= 0.1f;
									current3.y += 0.1f;
								}
								else
								{
									current2.y += 0.1f;
									current3.y -= 0.1f;
								}
								current2.x += 0.001f;
								current2.y += 0.001f;
								current3.x -= 0.001f;
								current3.y -= 0.001f;
								ResearchProjectDef.ClampInCoordinateLimits(current2);
								ResearchProjectDef.ClampInCoordinateLimits(current3);
							}
						}
					}
				}
				if (!flag)
				{
					break;
				}
				num++;
				if (num > 200)
				{
					goto Block_4;
				}
			}
			return;
			Block_4:
			Log.Error("Couldn't relax research project coordinates apart after " + 200 + " passes.");
		}

		private static void ClampInCoordinateLimits(ResearchProjectDef rp)
		{
			if (rp.x < 0f)
			{
				rp.x = 0f;
			}
			if (rp.y < 0f)
			{
				rp.y = 0f;
			}
			if (rp.y > 6.5f)
			{
				rp.y = 6.5f;
			}
		}
	}
}
