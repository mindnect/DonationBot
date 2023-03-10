using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public static class PowerConnectionMaker
	{
		private const int ConnectMaxDist = 6;

		public static void ConnectAllConnectorsToTransmitter(CompPower newTransmitter)
		{
			foreach (CompPower current in PowerConnectionMaker.PotentialConnectorsForTransmitter(newTransmitter))
			{
				if (current.connectParent == null)
				{
					current.ConnectToTransmitter(newTransmitter, false);
				}
			}
		}

		public static void DisconnectAllFromTransmitterAndSetWantConnect(CompPower deadPc, Map map)
		{
			if (deadPc.connectChildren == null)
			{
				return;
			}
			for (int i = 0; i < deadPc.connectChildren.Count; i++)
			{
				CompPower compPower = deadPc.connectChildren[i];
				compPower.connectParent = null;
				CompPowerTrader compPowerTrader = compPower as CompPowerTrader;
				if (compPowerTrader != null)
				{
					compPowerTrader.PowerOn = false;
				}
				map.powerNetManager.Notify_ConnectorWantsConnect(compPower);
			}
		}

		public static void TryConnectToAnyPowerNet(CompPower pc, List<PowerNet> disallowedNets = null)
		{
			if (pc.connectParent != null)
			{
				return;
			}
			if (!pc.parent.Spawned)
			{
				return;
			}
			CompPower compPower = PowerConnectionMaker.BestTransmitterForConnector(pc.parent.Position, pc.parent.Map, disallowedNets);
			if (compPower != null)
			{
				pc.ConnectToTransmitter(compPower, false);
			}
			else
			{
				pc.connectParent = null;
			}
		}

		public static void DisconnectFromPowerNet(CompPower pc)
		{
			if (pc.connectParent == null)
			{
				return;
			}
			if (pc.PowerNet != null)
			{
				pc.PowerNet.DeregisterConnector(pc);
			}
			if (pc.connectParent.connectChildren != null)
			{
				pc.connectParent.connectChildren.Remove(pc);
				if (pc.connectParent.connectChildren.Count == 0)
				{
					pc.connectParent.connectChildren = null;
				}
			}
			pc.connectParent = null;
		}

		[DebuggerHidden]
		private static IEnumerable<CompPower> PotentialConnectorsForTransmitter(CompPower b)
		{
			PowerConnectionMaker.<PotentialConnectorsForTransmitter>c__IteratorAD <PotentialConnectorsForTransmitter>c__IteratorAD = new PowerConnectionMaker.<PotentialConnectorsForTransmitter>c__IteratorAD();
			<PotentialConnectorsForTransmitter>c__IteratorAD.b = b;
			<PotentialConnectorsForTransmitter>c__IteratorAD.<$>b = b;
			PowerConnectionMaker.<PotentialConnectorsForTransmitter>c__IteratorAD expr_15 = <PotentialConnectorsForTransmitter>c__IteratorAD;
			expr_15.$PC = -2;
			return expr_15;
		}

		public static CompPower BestTransmitterForConnector(IntVec3 connectorPos, Map map, List<PowerNet> disallowedNets = null)
		{
			CellRect cellRect = CellRect.SingleCell(connectorPos).ExpandedBy(6).ClipInsideMap(map);
			cellRect.ClipInsideMap(map);
			float num = 999999f;
			CompPower result = null;
			for (int i = cellRect.minZ; i <= cellRect.maxZ; i++)
			{
				for (int j = cellRect.minX; j <= cellRect.maxX; j++)
				{
					IntVec3 c = new IntVec3(j, 0, i);
					Building transmitter = c.GetTransmitter(map);
					if (transmitter != null && !transmitter.Destroyed)
					{
						CompPower powerComp = transmitter.PowerComp;
						if (powerComp != null && powerComp.TransmitsPowerNow && (transmitter.def.building == null || transmitter.def.building.allowWireConnection))
						{
							if (disallowedNets == null || !disallowedNets.Contains(powerComp.transNet))
							{
								float lengthHorizontalSquared = (transmitter.Position - connectorPos).LengthHorizontalSquared;
								if (lengthHorizontalSquared < num)
								{
									num = lengthHorizontalSquared;
									result = powerComp;
								}
							}
						}
					}
				}
			}
			return result;
		}
	}
}
