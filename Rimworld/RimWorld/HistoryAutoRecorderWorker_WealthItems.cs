using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	public class HistoryAutoRecorderWorker_WealthItems : HistoryAutoRecorderWorker
	{
		public override float PullRecord()
		{
			float num = 0f;
			List<Map> maps = Find.Maps;
			for (int i = 0; i < maps.Count; i++)
			{
				if (maps[i].IsPlayerHome)
				{
					num += maps[i].wealthWatcher.WealthItems;
				}
			}
			return num;
		}
	}
}
