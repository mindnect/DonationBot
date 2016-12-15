using RimWorld.Planet;
using System;
using Verse;

namespace RimWorld
{
	public class IncidentWorker_JourneyOffer : IncidentWorker
	{
		private const int MinTraversalDistance = 200;

		private const int MaxTraversalDistance = 400;

		protected override bool CanFireNowSub(IIncidentTarget target)
		{
			int num;
			return this.TryFindDestinationTile(target, out num);
		}

		public override bool TryExecute(IncidentParms parms)
		{
			int tile;
			if (!this.TryFindDestinationTile(parms.target, out tile))
			{
				return false;
			}
			WorldObject journeyDestination = WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.JourneyDestination);
			journeyDestination.Tile = tile;
			Find.WorldObjects.Add(journeyDestination);
			DiaNode diaNode = new DiaNode("JourneyOffer".Translate());
			DiaOption diaOption = new DiaOption("JumpToLocation".Translate());
			diaOption.action = delegate
			{
				JumpToTargetUtility.TryJumpAndSelect(journeyDestination);
			};
			diaOption.resolveTree = true;
			diaNode.options.Add(diaOption);
			DiaOption diaOption2 = new DiaOption("OK".Translate());
			diaOption2.resolveTree = true;
			diaNode.options.Add(diaOption2);
			Find.WindowStack.Add(new Dialog_NodeTree(diaNode, true, true));
			return true;
		}

		private bool TryFindDestinationTile(IIncidentTarget target, out int tile)
		{
			return TileFinder.TryFindPassableTileWithTraversalDistance(target.Tile, 200, 400, out tile, (int x) => !Find.WorldObjects.AnyWorldObjectAt(x), true);
		}
	}
}
