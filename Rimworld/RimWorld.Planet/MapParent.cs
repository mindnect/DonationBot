using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld.Planet
{
	[StaticConstructorOnStartup]
	public class MapParent : WorldObject
	{
		public const int DefaultForceExitAndRemoveMapCountdownHours = 24;

		private int ticksLeftToForceExitAndRemoveMap = -1;

		private static readonly Texture2D ShowMapCommand = ContentFinder<Texture2D>.Get("UI/Commands/ShowMap", true);

		public static readonly Texture2D FormCaravanCommand = ContentFinder<Texture2D>.Get("UI/Commands/FormCaravan", true);

		private static List<Pawn> tmpPawns = new List<Pawn>();

		public bool HasMap
		{
			get
			{
				return this.Map != null;
			}
		}

		protected virtual bool UseGenericEnterMapFloatMenuOption
		{
			get
			{
				return true;
			}
		}

		public Map Map
		{
			get
			{
				return Current.Game.FindMap(this);
			}
		}

		public bool ForceExitAndRemoveMapCountdownActive
		{
			get
			{
				return this.ticksLeftToForceExitAndRemoveMap >= 0;
			}
		}

		public string ForceExitAndRemoveMapCountdownHoursLeftString
		{
			get
			{
				if (!this.ForceExitAndRemoveMapCountdownActive)
				{
					return string.Empty;
				}
				float num = (float)this.ticksLeftToForceExitAndRemoveMap / 2500f;
				if (num < 1f)
				{
					return num.ToString("0.#");
				}
				return Mathf.RoundToInt(num).ToString();
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<int>(ref this.ticksLeftToForceExitAndRemoveMap, "ticksLeftToForceExitAndRemoveMap", -1, false);
		}

		public virtual bool ShouldRemoveMapNow(out bool alsoRemoveWorldObject)
		{
			alsoRemoveWorldObject = false;
			return false;
		}

		public override void PostRemove()
		{
			base.PostRemove();
			if (this.HasMap)
			{
				Current.Game.DeinitAndRemoveMap(this.Map);
			}
		}

		public void StartForceExitAndRemoveMapCountdown()
		{
			this.StartForceExitAndRemoveMapCountdown(60000);
		}

		public void StartForceExitAndRemoveMapCountdown(int duration)
		{
			this.ticksLeftToForceExitAndRemoveMap = duration;
		}

		public override void Tick()
		{
			base.Tick();
			this.TickForceExitAndRemoveMapCountdown();
			this.CheckRemoveMapNow();
		}

		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			MapParent.<GetGizmos>c__IteratorEF <GetGizmos>c__IteratorEF = new MapParent.<GetGizmos>c__IteratorEF();
			<GetGizmos>c__IteratorEF.<>f__this = this;
			MapParent.<GetGizmos>c__IteratorEF expr_0E = <GetGizmos>c__IteratorEF;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		[DebuggerHidden]
		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
		{
			MapParent.<GetFloatMenuOptions>c__IteratorF0 <GetFloatMenuOptions>c__IteratorF = new MapParent.<GetFloatMenuOptions>c__IteratorF0();
			<GetFloatMenuOptions>c__IteratorF.caravan = caravan;
			<GetFloatMenuOptions>c__IteratorF.<$>caravan = caravan;
			<GetFloatMenuOptions>c__IteratorF.<>f__this = this;
			MapParent.<GetFloatMenuOptions>c__IteratorF0 expr_1C = <GetFloatMenuOptions>c__IteratorF;
			expr_1C.$PC = -2;
			return expr_1C;
		}

		public override string GetInspectString()
		{
			string text = base.GetInspectString();
			if (this.ForceExitAndRemoveMapCountdownActive)
			{
				if (text.Length > 0)
				{
					text += "\n";
				}
				text = text + "ForceExitAndRemoveMapCountdown".Translate(new object[]
				{
					this.ForceExitAndRemoveMapCountdownHoursLeftString
				}) + ".";
			}
			return text;
		}

		private void TickForceExitAndRemoveMapCountdown()
		{
			if (this.ForceExitAndRemoveMapCountdownActive)
			{
				if (this.HasMap)
				{
					this.ticksLeftToForceExitAndRemoveMap--;
					if (this.ticksLeftToForceExitAndRemoveMap == 0)
					{
						MapParent.tmpPawns.Clear();
						MapParent.tmpPawns.AddRange(from x in this.Map.mapPawns.AllPawns
						where x.Faction == Faction.OfPlayer || x.HostFaction == Faction.OfPlayer
						select x);
						if (MapParent.tmpPawns.Any<Pawn>())
						{
							if (MapParent.tmpPawns.Any((Pawn x) => CaravanUtility.IsOwner(x, Faction.OfPlayer)))
							{
								Caravan o = CaravanExitMapUtility.ExitMapAndCreateCaravan(MapParent.tmpPawns, Faction.OfPlayer, base.Tile);
								Messages.Message("MessageAutomaticallyReformedCaravan".Translate(), o, MessageSound.Benefit);
							}
							else
							{
								StringBuilder stringBuilder = new StringBuilder();
								for (int i = 0; i < MapParent.tmpPawns.Count; i++)
								{
									stringBuilder.AppendLine("    " + MapParent.tmpPawns[i].LabelCap);
								}
								Find.LetterStack.ReceiveLetter("LetterLabelPawnsLostDueToMapCountdown".Translate(), "LetterPawnsLostDueToMapCountdown".Translate(new object[]
								{
									stringBuilder.ToString().TrimEndNewlines()
								}), LetterType.BadNonUrgent, new GlobalTargetInfo(base.Tile), null);
							}
							MapParent.tmpPawns.Clear();
						}
						this.OpenWorldTabIfVisibleMapAboutToBeRemoved(this.Map);
						Find.WorldObjects.Remove(this);
					}
				}
				else
				{
					this.ticksLeftToForceExitAndRemoveMap = -1;
				}
			}
		}

		public void CheckRemoveMapNow()
		{
			bool flag;
			if (this.HasMap && this.ShouldRemoveMapNow(out flag))
			{
				Map map = this.Map;
				this.OpenWorldTabIfVisibleMapAboutToBeRemoved(map);
				Current.Game.DeinitAndRemoveMap(map);
				if (flag)
				{
					Find.WorldObjects.Remove(this);
				}
			}
		}

		private void OpenWorldTabIfVisibleMapAboutToBeRemoved(Map map)
		{
			if (map == Find.VisibleMap)
			{
				Find.MainTabsRoot.SetCurrentTab(MainTabDefOf.World, true);
			}
		}
	}
}
