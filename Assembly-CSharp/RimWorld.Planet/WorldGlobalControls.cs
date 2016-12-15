using System;
using UnityEngine;
using Verse;

namespace RimWorld.Planet
{
	public class WorldGlobalControls
	{
		public const float Width = 150f;

		private const int VisibilityControlsPerRow = 5;

		private WidgetRow rowVisibility = new WidgetRow();

		public void WorldGlobalControlsOnGUI()
		{
			if (Event.current.type == EventType.Layout)
			{
				return;
			}
			float leftX = (float)UI.screenWidth - 150f;
			float num = (float)UI.screenHeight - 4f;
			if (Current.ProgramState == ProgramState.Playing)
			{
				num -= 35f;
			}
			Profiler.BeginSample("World play settings");
			GlobalControlsUtility.DoPlaySettings(this.rowVisibility, true, ref num);
			Profiler.EndSample();
			if (Current.ProgramState == ProgramState.Playing)
			{
				num -= 4f;
				Profiler.BeginSample("Timespeed controls");
				GlobalControlsUtility.DoTimespeedControls(leftX, 150f, ref num);
				Profiler.EndSample();
				if (Find.VisibleMap != null || Find.WorldSelector.AnyObjectOrTileSelected)
				{
					num -= 4f;
					Profiler.BeginSample("Date");
					GlobalControlsUtility.DoDate(leftX, 150f, ref num);
					Profiler.EndSample();
				}
			}
			if (Prefs.ShowRealtimeClock)
			{
				Profiler.BeginSample("RealtimeClock");
				GlobalControlsUtility.DoRealtimeClock(leftX, 150f, ref num);
				Profiler.EndSample();
			}
			if (!Find.PlaySettings.lockNorthUp)
			{
				Profiler.BeginSample("Compass");
				CompassWidget.CompassOnGUI(ref num);
				Profiler.EndSample();
			}
			if (Current.ProgramState == ProgramState.Playing)
			{
				Profiler.BeginSample("Letters");
				num -= 10f;
				Find.LetterStack.LettersOnGUI(num);
				Profiler.EndSample();
			}
		}
	}
}
