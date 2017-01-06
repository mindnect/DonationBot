using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public static class WindTurbineUtility
	{
		[DebuggerHidden]
		public static IEnumerable<IntVec3> CalculateWindCells(IntVec3 center, Rot4 rot, IntVec2 size)
		{
			WindTurbineUtility.<CalculateWindCells>c__IteratorAC <CalculateWindCells>c__IteratorAC = new WindTurbineUtility.<CalculateWindCells>c__IteratorAC();
			<CalculateWindCells>c__IteratorAC.rot = rot;
			<CalculateWindCells>c__IteratorAC.center = center;
			<CalculateWindCells>c__IteratorAC.<$>rot = rot;
			<CalculateWindCells>c__IteratorAC.<$>center = center;
			WindTurbineUtility.<CalculateWindCells>c__IteratorAC expr_23 = <CalculateWindCells>c__IteratorAC;
			expr_23.$PC = -2;
			return expr_23;
		}
	}
}
