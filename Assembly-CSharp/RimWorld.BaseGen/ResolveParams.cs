using System;
using Verse;
using Verse.AI.Group;

namespace RimWorld.BaseGen
{
	public struct ResolveParams
	{
		public CellRect rect;

		public Faction faction;

		public int? ancientTempleEntranceHeight;

		public PawnGroupMakerParms pawnGroupMakerParams;

		public PawnGroupKindDef pawnGroupKindDef;

		public float? chanceToSkipSandbag;

		public RoofDef roofDef;

		public bool? noRoof;

		public ThingDef singleThingDef;

		public int? singleThingStackCount;

		public Pawn singlePawnToSpawn;

		public PawnKindDef singlePawnKindDef;

		public bool? disableSinglePawn;

		public Lord singlePawnLord;

		public Predicate<IntVec3> singlePawnSpawnCellExtraPredicate;

		public int? mechanoidsCount;

		public int? hivesCount;

		public bool? disableHives;

		public Rot4? thingRot;

		public ThingDef wallStuff;

		public float? chanceToSkipWallBlock;

		public TerrainDef floorDef;

		public bool? clearEdificeOnly;

		public int? ancientCryptosleepCasketGroupID;

		public PodContentsType? podContentsType;

		public override string ToString()
		{
			object[] expr_07 = new object[48];
			expr_07[0] = "rect=";
			expr_07[1] = this.rect;
			expr_07[2] = ", faction=";
			expr_07[3] = ((this.faction == null) ? "null" : this.faction.ToString());
			expr_07[4] = ", pawnGroupMakerParams=";
			expr_07[5] = ((this.pawnGroupMakerParams == null) ? "null" : this.pawnGroupMakerParams.ToString());
			expr_07[6] = ", pawnGroupKindDef=";
			expr_07[7] = ((this.pawnGroupKindDef == null) ? "null" : this.pawnGroupKindDef.ToString());
			expr_07[8] = ", chanceToSkipSandbag=";
			int arg_D1_1 = 9;
			float? num = this.chanceToSkipSandbag;
			expr_07[arg_D1_1] = ((!num.HasValue) ? "null" : this.chanceToSkipSandbag.ToString());
			expr_07[10] = ", roofDef=";
			expr_07[11] = ((this.roofDef == null) ? "null" : this.roofDef.ToString());
			expr_07[12] = ", noRoof=";
			int arg_133_1 = 13;
			bool? flag = this.noRoof;
			expr_07[arg_133_1] = ((!flag.HasValue) ? "null" : this.noRoof.ToString());
			expr_07[14] = ", singleThingDef=";
			expr_07[15] = ((this.singleThingDef == null) ? "null" : this.singleThingDef.ToString());
			expr_07[16] = ", singleThingStackCount=";
			int arg_195_1 = 17;
			int? num2 = this.singleThingStackCount;
			expr_07[arg_195_1] = ((!num2.HasValue) ? "null" : this.singleThingStackCount.ToString());
			expr_07[18] = ", singlePawnToSpawn=";
			expr_07[19] = ((this.singlePawnToSpawn == null) ? "null" : this.singlePawnToSpawn.ToString());
			expr_07[20] = ", singlePawnKindDef=";
			expr_07[21] = ((this.singlePawnKindDef == null) ? "null" : this.singlePawnKindDef.ToString());
			expr_07[22] = ", disableSinglePawn=";
			int arg_224_1 = 23;
			bool? flag2 = this.disableSinglePawn;
			expr_07[arg_224_1] = ((!flag2.HasValue) ? "null" : this.disableSinglePawn.ToString());
			expr_07[24] = ", singlePawnLord=";
			expr_07[25] = ((this.singlePawnLord == null) ? "null" : this.singlePawnLord.ToString());
			expr_07[26] = ", singlePawnSpawnCellExtraPredicate=";
			expr_07[27] = ((this.singlePawnSpawnCellExtraPredicate == null) ? "null" : this.singlePawnSpawnCellExtraPredicate.ToString());
			expr_07[28] = ", mechanoidsCount=";
			int arg_2B4_1 = 29;
			int? num3 = this.mechanoidsCount;
			expr_07[arg_2B4_1] = ((!num3.HasValue) ? "null" : this.mechanoidsCount.ToString());
			expr_07[30] = ", hivesCount=";
			int arg_2EA_1 = 31;
			int? num4 = this.hivesCount;
			expr_07[arg_2EA_1] = ((!num4.HasValue) ? "null" : this.hivesCount.ToString());
			expr_07[32] = ", disableHives=";
			int arg_320_1 = 33;
			bool? flag3 = this.disableHives;
			expr_07[arg_320_1] = ((!flag3.HasValue) ? "null" : this.disableHives.ToString());
			expr_07[34] = ", thingRot=";
			int arg_356_1 = 35;
			Rot4? rot = this.thingRot;
			expr_07[arg_356_1] = ((!rot.HasValue) ? "null" : this.thingRot.ToString());
			expr_07[36] = ", wallStuff=";
			expr_07[37] = ((this.wallStuff == null) ? "null" : this.wallStuff.ToString());
			expr_07[38] = ", chanceToSkipWallBlock=";
			int arg_3B9_1 = 39;
			float? num5 = this.chanceToSkipWallBlock;
			expr_07[arg_3B9_1] = ((!num5.HasValue) ? "null" : this.chanceToSkipWallBlock.ToString());
			expr_07[40] = ", floorDef=";
			expr_07[41] = ((this.floorDef == null) ? "null" : this.floorDef.ToString());
			expr_07[42] = ", clearEdificeOnly=";
			int arg_41C_1 = 43;
			bool? flag4 = this.clearEdificeOnly;
			expr_07[arg_41C_1] = ((!flag4.HasValue) ? "null" : this.clearEdificeOnly.ToString());
			expr_07[44] = ", ancientCryptosleepCasketGroupID=";
			int arg_452_1 = 45;
			int? num6 = this.ancientCryptosleepCasketGroupID;
			expr_07[arg_452_1] = ((!num6.HasValue) ? "null" : this.ancientCryptosleepCasketGroupID.ToString());
			expr_07[46] = ", podContentsType=";
			int arg_488_1 = 47;
			PodContentsType? podContentsType = this.podContentsType;
			expr_07[arg_488_1] = ((!podContentsType.HasValue) ? "null" : this.podContentsType.ToString());
			return string.Concat(expr_07);
		}
	}
}
