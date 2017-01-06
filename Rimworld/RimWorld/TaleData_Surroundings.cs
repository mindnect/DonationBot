using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.Grammar;

namespace RimWorld
{
	public class TaleData_Surroundings : TaleData
	{
		public int tile;

		public float temperature;

		public float snowDepth;

		public WeatherDef weather;

		public RoomRoleDef roomRole;

		public float roomImpressiveness;

		public float roomBeauty;

		public float roomCleanliness;

		public bool Outdoors
		{
			get
			{
				return this.weather != null;
			}
		}

		public override void ExposeData()
		{
			Scribe_Values.LookValue<int>(ref this.tile, "tile", 0, false);
			Scribe_Values.LookValue<float>(ref this.temperature, "temperature", 0f, false);
			Scribe_Values.LookValue<float>(ref this.snowDepth, "snowDepth", 0f, false);
			Scribe_Defs.LookDef<WeatherDef>(ref this.weather, "weather");
			Scribe_Defs.LookDef<RoomRoleDef>(ref this.roomRole, "roomRole");
			Scribe_Values.LookValue<float>(ref this.roomImpressiveness, "roomImpressiveness", 0f, false);
			Scribe_Values.LookValue<float>(ref this.roomBeauty, "roomBeauty", 0f, false);
			Scribe_Values.LookValue<float>(ref this.roomCleanliness, "roomCleanliness", 0f, false);
		}

		[DebuggerHidden]
		public override IEnumerable<Rule> GetRules()
		{
			TaleData_Surroundings.<GetRules>c__Iterator113 <GetRules>c__Iterator = new TaleData_Surroundings.<GetRules>c__Iterator113();
			<GetRules>c__Iterator.<>f__this = this;
			TaleData_Surroundings.<GetRules>c__Iterator113 expr_0E = <GetRules>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		public static TaleData_Surroundings GenerateFrom(IntVec3 c, Map map)
		{
			TaleData_Surroundings taleData_Surroundings = new TaleData_Surroundings();
			taleData_Surroundings.tile = map.Tile;
			Room roomOrAdjacent = c.GetRoomOrAdjacent(map);
			if (roomOrAdjacent != null)
			{
				if (roomOrAdjacent.PsychologicallyOutdoors)
				{
					taleData_Surroundings.weather = map.weatherManager.curWeather;
				}
				taleData_Surroundings.roomRole = roomOrAdjacent.Role;
				taleData_Surroundings.roomImpressiveness = roomOrAdjacent.GetStat(RoomStatDefOf.Impressiveness);
				taleData_Surroundings.roomBeauty = roomOrAdjacent.GetStat(RoomStatDefOf.Beauty);
				taleData_Surroundings.roomCleanliness = roomOrAdjacent.GetStat(RoomStatDefOf.Cleanliness);
			}
			if (!GenTemperature.TryGetTemperatureForCell(c, map, out taleData_Surroundings.temperature))
			{
				taleData_Surroundings.temperature = 21f;
			}
			taleData_Surroundings.snowDepth = map.snowGrid.GetDepth(c);
			return taleData_Surroundings;
		}

		public static TaleData_Surroundings GenerateRandom(Map map)
		{
			return TaleData_Surroundings.GenerateFrom(CellFinder.RandomCell(map), map);
		}
	}
}
