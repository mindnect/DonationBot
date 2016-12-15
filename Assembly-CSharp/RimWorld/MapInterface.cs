using RimWorld.Planet;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class MapInterface
	{
		public ThingOverlays thingOverlays = new ThingOverlays();

		public Selector selector = new Selector();

		public Targeter targeter = new Targeter();

		public DesignatorManager designatorManager = new DesignatorManager();

		public ReverseDesignatorDatabase reverseDesignatorDatabase = new ReverseDesignatorDatabase();

		private MouseoverReadout mouseoverReadout = new MouseoverReadout();

		public GlobalControls globalControls = new GlobalControls();

		protected ResourceReadout resourceReadout = new ResourceReadout();

		public ColonistBar colonistBar = new ColonistBar();

		public void MapInterfaceOnGUI_BeforeMainTabs()
		{
			if (Find.VisibleMap == null)
			{
				return;
			}
			if (!WorldRendererUtility.WorldRenderedNow)
			{
				ScreenshotModeHandler screenshotMode = Find.UIRoot.screenshotMode;
				Profiler.BeginSample("ThingOverlaysOnGUI()");
				this.thingOverlays.ThingOverlaysOnGUI();
				Profiler.EndSample();
				Profiler.BeginSample("MapComponentOnGUI()");
				for (int i = 0; i < Find.VisibleMap.components.Count; i++)
				{
					Find.VisibleMap.components[i].MapComponentOnGUI();
				}
				Profiler.EndSample();
				if (!screenshotMode.FiltersCurrentEvent)
				{
					Profiler.BeginSample("ColonistBarOnGUI()");
					this.colonistBar.ColonistBarOnGUI();
					Profiler.EndSample();
				}
				Profiler.BeginSample("DragBoxOnGUI()");
				this.selector.dragBox.DragBoxOnGUI();
				Profiler.EndSample();
				Profiler.BeginSample("DesignationManagerOnGUI()");
				this.designatorManager.DesignationManagerOnGUI();
				Profiler.EndSample();
				Profiler.BeginSample("TargeterOnGUI()");
				this.targeter.TargeterOnGUI();
				Profiler.EndSample();
				Profiler.BeginSample("DispenseAllThingTooltips()");
				Find.VisibleMap.tooltipGiverList.DispenseAllThingTooltips();
				Profiler.EndSample();
				if (DebugViewSettings.drawFoodSearchFromMouse)
				{
					Profiler.BeginSample("FoodUtility.DebugFoodSearchFromMouse_OnGUI()");
					FoodUtility.DebugFoodSearchFromMouse_OnGUI();
					Profiler.EndSample();
				}
				if (!screenshotMode.FiltersCurrentEvent)
				{
					Profiler.BeginSample("GlobalControlsOnGUI()");
					this.globalControls.GlobalControlsOnGUI();
					Profiler.EndSample();
					Profiler.BeginSample("ResourceReadoutOnGUI()");
					this.resourceReadout.ResourceReadoutOnGUI();
					Profiler.EndSample();
				}
			}
		}

		public void MapInterfaceOnGUI_AfterMainTabs()
		{
			if (Find.VisibleMap == null)
			{
				return;
			}
			if (!WorldRendererUtility.WorldRenderedNow)
			{
				ScreenshotModeHandler screenshotMode = Find.UIRoot.screenshotMode;
				if (!screenshotMode.FiltersCurrentEvent)
				{
					Profiler.BeginSample("MouseoverReadoutOnGUI()");
					this.mouseoverReadout.MouseoverReadoutOnGUI();
					Profiler.EndSample();
					Profiler.BeginSample("RoomStatsOnGUI()");
					EnvironmentInspectDrawer.EnvironmentInspectOnGUI();
					Profiler.EndSample();
					Profiler.BeginSample("DebugDrawerOnGUI()");
					Find.VisibleMap.debugDrawer.DebugDrawerOnGUI();
					Profiler.EndSample();
				}
			}
		}

		public void HandleMapClicks()
		{
			if (Find.VisibleMap == null)
			{
				return;
			}
			if (!WorldRendererUtility.WorldRenderedNow)
			{
				Profiler.BeginSample("DesignatorManager.ProcessInputEvents()");
				this.designatorManager.ProcessInputEvents();
				Profiler.EndSample();
				Profiler.BeginSample("targeter.ProcessInputEvents()");
				this.targeter.ProcessInputEvents();
				Profiler.EndSample();
			}
		}

		public void HandleLowPriorityInput()
		{
			if (Find.VisibleMap == null)
			{
				return;
			}
			if (!WorldRendererUtility.WorldRenderedNow)
			{
				Profiler.BeginSample("SelectorOnGUI()");
				this.selector.SelectorOnGUI();
				Profiler.EndSample();
				Profiler.BeginSample("LordManagerOnGUI()");
				Find.VisibleMap.lordManager.LordManagerOnGUI();
				Profiler.EndSample();
			}
		}

		public void MapInterfaceUpdate()
		{
			if (Find.VisibleMap == null)
			{
				return;
			}
			if (!WorldRendererUtility.WorldRenderedNow)
			{
				Profiler.BeginSample("TargeterUpdate()");
				this.targeter.TargeterUpdate();
				Profiler.EndSample();
				Profiler.BeginSample("DrawSelectionOverlays()");
				SelectionDrawer.DrawSelectionOverlays();
				Profiler.EndSample();
				Profiler.BeginSample("RoomStatsDrawer.DrawRoomOverlays()");
				EnvironmentInspectDrawer.DrawRoomOverlays();
				Profiler.EndSample();
				Profiler.BeginSample("DesignatorManagerUpdate()");
				this.designatorManager.DesignatorManagerUpdate();
				Profiler.EndSample();
				Profiler.BeginSample("RoofGridUpdate()");
				Find.VisibleMap.roofGrid.RoofGridUpdate();
				Profiler.EndSample();
				Profiler.BeginSample("ExitMapGridUpdate()");
				Find.VisibleMap.exitMapGrid.ExitMapGridUpdate();
				Profiler.EndSample();
				Profiler.BeginSample("Debug drawing");
				if (DebugViewSettings.drawPawnDebug)
				{
					Find.VisibleMap.pawnDestinationManager.DebugDrawDestinations();
					Find.VisibleMap.reservationManager.DebugDrawReservations();
				}
				if (DebugViewSettings.drawFoodSearchFromMouse)
				{
					FoodUtility.DebugFoodSearchFromMouse_Update();
				}
				if (DebugViewSettings.drawPreyInfo)
				{
					FoodUtility.DebugDrawPredatorFoodSource();
				}
				Find.VisibleMap.debugDrawer.DebugDrawerUpdate();
				Find.VisibleMap.regionGrid.DebugDraw();
				InfestationCellFinder.DebugDraw();
				StealAIDebugDrawer.DebugDraw();
				Profiler.EndSample();
			}
		}

		public void Notify_SwitchedMap()
		{
			this.designatorManager.ResetSelectedDesignator();
			this.reverseDesignatorDatabase.Reinit();
			this.selector.ClearSelection();
			this.targeter.StopTargeting();
			MainTabDef openTab = Find.MainTabsRoot.OpenTab;
			bool everOpened = ((MainTabWindow_World)MainTabDefOf.World.Window).everOpened;
			List<MainTabDef> allDefsListForReading = DefDatabase<MainTabDef>.AllDefsListForReading;
			for (int i = 0; i < allDefsListForReading.Count; i++)
			{
				allDefsListForReading[i].Notify_SwitchedMap();
			}
			((MainTabWindow_World)MainTabDefOf.World.Window).everOpened = everOpened;
			if (openTab != null && openTab != MainTabDefOf.Inspect)
			{
				Find.MainTabsRoot.SetCurrentTab(openTab, false);
			}
			if (Find.VisibleMap != null)
			{
				RememberedCameraPos rememberedCameraPos = Find.VisibleMap.rememberedCameraPos;
				Find.CameraDriver.SetRootPosAndSize(rememberedCameraPos.rootPos, rememberedCameraPos.rootSize);
			}
		}
	}
}
