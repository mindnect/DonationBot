using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Verse
{
	public static class GraphicDatabaseUtility
	{
		[DebuggerHidden]
		public static IEnumerable<string> GraphicNamesInFolder(string folderPath)
		{
			GraphicDatabaseUtility.<GraphicNamesInFolder>c__Iterator200 <GraphicNamesInFolder>c__Iterator = new GraphicDatabaseUtility.<GraphicNamesInFolder>c__Iterator200();
			<GraphicNamesInFolder>c__Iterator.folderPath = folderPath;
			<GraphicNamesInFolder>c__Iterator.<$>folderPath = folderPath;
			GraphicDatabaseUtility.<GraphicNamesInFolder>c__Iterator200 expr_15 = <GraphicNamesInFolder>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}
	}
}
