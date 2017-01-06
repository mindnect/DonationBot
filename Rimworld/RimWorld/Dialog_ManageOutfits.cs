using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class Dialog_ManageOutfits : Window
	{
		private const float TopAreaHeight = 40f;

		private const float TopButtonHeight = 35f;

		private const float TopButtonWidth = 150f;

		private Vector2 scrollPosition;

		private Outfit selOutfitInt;

		private static ThingFilter apparelGlobalFilter;

		private Outfit SelectedOutfit
		{
			get
			{
				return this.selOutfitInt;
			}
			set
			{
				this.CheckSelectedOutfitHasName();
				this.selOutfitInt = value;
			}
		}

		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(700f, 700f);
			}
		}

		public Dialog_ManageOutfits(Outfit selectedOutfit)
		{
			this.forcePause = true;
			this.doCloseX = true;
			this.closeOnEscapeKey = true;
			this.doCloseButton = true;
			this.closeOnClickedOutside = true;
			this.absorbInputAroundWindow = true;
			if (Dialog_ManageOutfits.apparelGlobalFilter == null)
			{
				Dialog_ManageOutfits.apparelGlobalFilter = new ThingFilter();
				Dialog_ManageOutfits.apparelGlobalFilter.SetAllow(ThingCategoryDefOf.Apparel, true);
			}
			this.SelectedOutfit = selectedOutfit;
		}

		private void CheckSelectedOutfitHasName()
		{
			if (this.SelectedOutfit != null && this.SelectedOutfit.label.NullOrEmpty())
			{
				this.SelectedOutfit.label = "Unnamed";
			}
		}

		public override void DoWindowContents(Rect inRect)
		{
			float num = 0f;
			Rect rect = new Rect(0f, 0f, 150f, 35f);
			num += 150f;
			if (Widgets.ButtonText(rect, "SelectOutfit".Translate(), true, false, true))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				foreach (Outfit current in Current.Game.outfitDatabase.AllOutfits)
				{
					Outfit localOut = current;
					list.Add(new FloatMenuOption(localOut.label, delegate
					{
						this.SelectedOutfit = localOut;
					}, MenuOptionPriority.Default, null, null, 0f, null, null));
				}
				Find.WindowStack.Add(new FloatMenu(list));
			}
			num += 10f;
			Rect rect2 = new Rect(num, 0f, 150f, 35f);
			num += 150f;
			if (Widgets.ButtonText(rect2, "NewOutfit".Translate(), true, false, true))
			{
				this.SelectedOutfit = Current.Game.outfitDatabase.MakeNewOutfit();
			}
			num += 10f;
			Rect rect3 = new Rect(num, 0f, 150f, 35f);
			num += 150f;
			if (Widgets.ButtonText(rect3, "DeleteOutfit".Translate(), true, false, true))
			{
				List<FloatMenuOption> list2 = new List<FloatMenuOption>();
				foreach (Outfit current2 in Current.Game.outfitDatabase.AllOutfits)
				{
					Outfit localOut = current2;
					list2.Add(new FloatMenuOption(localOut.label, delegate
					{
						AcceptanceReport acceptanceReport = Current.Game.outfitDatabase.TryDelete(localOut);
						if (!acceptanceReport.Accepted)
						{
							Messages.Message(acceptanceReport.Reason, MessageSound.RejectInput);
						}
						else if (localOut == this.SelectedOutfit)
						{
							this.SelectedOutfit = null;
						}
					}, MenuOptionPriority.Default, null, null, 0f, null, null));
				}
				Find.WindowStack.Add(new FloatMenu(list2));
			}
			Rect rect4 = new Rect(0f, 40f, inRect.width, inRect.height - 40f - this.CloseButSize.y).ContractedBy(10f);
			if (this.SelectedOutfit == null)
			{
				GUI.color = Color.grey;
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.Label(rect4, "NoOutfitSelected".Translate());
				Text.Anchor = TextAnchor.UpperLeft;
				GUI.color = Color.white;
				return;
			}
			GUI.BeginGroup(rect4);
			Rect rect5 = new Rect(0f, 0f, 200f, 30f);
			Dialog_ManageOutfits.DoNameInputRect(rect5, ref this.SelectedOutfit.label);
			Rect rect6 = new Rect(0f, 40f, 300f, rect4.height - 45f - 10f);
			IEnumerable<SpecialThingFilterDef> forceHiddenFilters = this.HiddenSpecialThingFilters();
			ThingFilterUI.DoThingFilterConfigWindow(rect6, ref this.scrollPosition, this.SelectedOutfit.filter, Dialog_ManageOutfits.apparelGlobalFilter, 16, null, forceHiddenFilters);
			GUI.EndGroup();
		}

		[DebuggerHidden]
		private IEnumerable<SpecialThingFilterDef> HiddenSpecialThingFilters()
		{
			Dialog_ManageOutfits.<HiddenSpecialThingFilters>c__Iterator178 <HiddenSpecialThingFilters>c__Iterator = new Dialog_ManageOutfits.<HiddenSpecialThingFilters>c__Iterator178();
			Dialog_ManageOutfits.<HiddenSpecialThingFilters>c__Iterator178 expr_07 = <HiddenSpecialThingFilters>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}

		public override void PreClose()
		{
			base.PreClose();
			this.CheckSelectedOutfitHasName();
		}

		public static void DoNameInputRect(Rect rect, ref string name)
		{
			name = Widgets.TextField(rect, name, 30, Outfit.ValidNameRegex);
		}
	}
}
