using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	[StaticConstructorOnStartup]
	public class LearningReadout : IExposable
	{
		private const float OuterMargin = 8f;

		private const float InnerMargin = 7f;

		private const float ReadoutWidth = 200f;

		private const float InfoPaneWidth = 310f;

		private const float OpenButtonSize = 24f;

		private List<ConceptDef> activeConcepts = new List<ConceptDef>();

		private ConceptDef selectedConcept;

		private bool showAllMode;

		private float contentHeight;

		private Vector2 scrollPosition = Vector2.zero;

		private string searchString = string.Empty;

		private float lastConceptActivateRealTime = -999f;

		private ConceptDef mouseoverConcept;

		public static readonly Texture2D ProgressBarFillTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.745098054f, 0.6039216f, 0.2f));

		public static readonly Texture2D ProgressBarBGTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.509803951f, 0.407843143f, 0.13333334f));

		public int ActiveConceptsCount
		{
			get
			{
				return this.activeConcepts.Count;
			}
		}

		public bool ShowAllMode
		{
			get
			{
				return this.showAllMode;
			}
		}

		public void ExposeData()
		{
			Scribe_Collections.LookList<ConceptDef>(ref this.activeConcepts, "activeConcepts", LookMode.Undefined, new object[0]);
			Scribe_Defs.LookDef<ConceptDef>(ref this.selectedConcept, "selectedConcept");
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				this.activeConcepts.RemoveAll((ConceptDef c) => PlayerKnowledgeDatabase.IsComplete(c));
			}
		}

		public bool TryActivateConcept(ConceptDef conc)
		{
			if (this.activeConcepts.Contains(conc))
			{
				return false;
			}
			this.activeConcepts.Add(conc);
			SoundDefOf.LessonActivated.PlayOneShotOnCamera();
			this.lastConceptActivateRealTime = RealTime.LastRealTime;
			return true;
		}

		public bool IsActive(ConceptDef conc)
		{
			return this.activeConcepts.Contains(conc);
		}

		public void LearningReadoutUpdate()
		{
		}

		public void Notify_ConceptNewlyLearned(ConceptDef conc)
		{
			if (this.activeConcepts.Contains(conc) || this.selectedConcept == conc)
			{
				SoundDefOf.LessonDeactivated.PlayOneShotOnCamera();
				SoundDefOf.CommsWindow_Close.PlayOneShotOnCamera();
			}
			if (this.activeConcepts.Contains(conc))
			{
				this.activeConcepts.Remove(conc);
			}
			if (this.selectedConcept == conc)
			{
				this.selectedConcept = null;
			}
		}

		private string FilterSearchStringInput(string input)
		{
			if (input == this.searchString)
			{
				return input;
			}
			if (input.Length > 20)
			{
				input = input.Substring(0, 20);
			}
			return input;
		}

		public void LearningReadoutOnGUI()
		{
			if (TutorSystem.TutorialMode || !TutorSystem.AdaptiveTrainingEnabled)
			{
				return;
			}
			if (!Find.PlaySettings.showLearningHelper && this.activeConcepts.Count == 0)
			{
				return;
			}
			if (Find.WindowStack.IsOpen<Screen_Credits>())
			{
				return;
			}
			float b = (float)UI.screenHeight / 2f;
			float a = this.contentHeight + 14f;
			Rect outRect = new Rect((float)UI.screenWidth - 8f - 200f, 8f, 200f, Mathf.Min(a, b));
			Rect outRect2 = outRect;
			Find.WindowStack.ImmediateWindow(76136312, outRect, WindowLayer.Super, delegate
			{
				outRect = outRect.AtZero();
				Rect rect = outRect.ContractedBy(7f);
				Rect viewRect = rect.AtZero();
				bool flag = this.contentHeight > rect.height;
				Widgets.DrawWindowBackgroundTutor(outRect);
				if (flag)
				{
					viewRect.height = this.contentHeight + 40f;
					viewRect.width -= 20f;
					this.scrollPosition = GUI.BeginScrollView(rect, this.scrollPosition, viewRect);
				}
				else
				{
					GUI.BeginGroup(rect);
				}
				float num2 = 0f;
				Text.Font = GameFont.Small;
				Rect rect2 = new Rect(0f, 0f, viewRect.width - 24f, 24f);
				Widgets.Label(rect2, "LearningHelper".Translate());
				num2 = rect2.yMax;
				Rect butRect = new Rect(rect2.xMax, rect2.y, 24f, 24f);
				if (Widgets.ButtonImage(butRect, this.showAllMode ? TexButton.Minus : TexButton.Plus))
				{
					this.showAllMode = !this.showAllMode;
					if (this.showAllMode)
					{
						SoundDefOf.TickHigh.PlayOneShotOnCamera();
					}
					else
					{
						SoundDefOf.TickLow.PlayOneShotOnCamera();
					}
				}
				if (this.showAllMode)
				{
					Rect rect3 = new Rect(0f, num2, viewRect.width - 20f - 2f, 28f);
					this.searchString = this.FilterSearchStringInput(Widgets.TextField(rect3, this.searchString));
					if (this.searchString == string.Empty)
					{
						GUI.color = new Color(0.6f, 0.6f, 0.6f, 1f);
						Text.Anchor = TextAnchor.MiddleLeft;
						Rect rect4 = rect3;
						rect4.xMin += 7f;
						Widgets.Label(rect4, "Filter".Translate() + "...");
						Text.Anchor = TextAnchor.UpperLeft;
						GUI.color = Color.white;
					}
					Rect butRect2 = new Rect(viewRect.width - 20f, num2 + 14f - 10f, 20f, 20f);
					if (Widgets.ButtonImage(butRect2, TexButton.CloseXSmall))
					{
						this.searchString = string.Empty;
						SoundDefOf.TickTiny.PlayOneShotOnCamera();
					}
					num2 = rect3.yMax + 4f;
				}
				IEnumerable<ConceptDef> arg_2E0_0;
				if (!this.showAllMode)
				{
					IEnumerable<ConceptDef> enumerable = this.activeConcepts;
					arg_2E0_0 = enumerable;
				}
				else
				{
					arg_2E0_0 = DefDatabase<ConceptDef>.AllDefs;
				}
				IEnumerable<ConceptDef> enumerable2 = arg_2E0_0;
				if (enumerable2.Any<ConceptDef>())
				{
					GUI.color = new Color(1f, 1f, 1f, 0.5f);
					Widgets.DrawLineHorizontal(0f, num2, viewRect.width);
					GUI.color = Color.white;
					num2 += 4f;
				}
				if (this.showAllMode)
				{
					enumerable2 = from c in enumerable2
					orderby this.DisplayPriority(c) descending, c.label
					select c;
				}
				foreach (ConceptDef current in enumerable2)
				{
					if (!current.TriggeredDirect)
					{
						num2 = this.DrawConceptListRow(0f, num2, viewRect.width, current).yMax;
					}
				}
				this.contentHeight = num2;
				if (flag)
				{
					GUI.EndScrollView();
				}
				else
				{
					GUI.EndGroup();
				}
			}, false, false, 1f);
			float num = Time.realtimeSinceStartup - this.lastConceptActivateRealTime;
			if (num < 1f && num > 0f)
			{
				GenUI.DrawFlash(outRect2.x, outRect2.center.y, (float)UI.screenWidth * 0.6f, Pulser.PulseBrightness(1f, 1f, num) * 0.85f, new Color(0.8f, 0.77f, 0.53f));
			}
			ConceptDef conceptDef = (this.selectedConcept == null) ? this.mouseoverConcept : this.selectedConcept;
			if (conceptDef != null)
			{
				this.DrawInfoPane(conceptDef);
				conceptDef.HighlightAllTags();
			}
			this.mouseoverConcept = null;
		}

		private int DisplayPriority(ConceptDef conc)
		{
			int num = 1;
			if (this.MatchesSearchString(conc))
			{
				num += 10000;
			}
			return num;
		}

		private bool MatchesSearchString(ConceptDef conc)
		{
			return this.searchString != string.Empty && conc.label.IndexOf(this.searchString, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		private Rect DrawConceptListRow(float x, float y, float width, ConceptDef conc)
		{
			float knowledge = PlayerKnowledgeDatabase.GetKnowledge(conc);
			bool flag = PlayerKnowledgeDatabase.IsComplete(conc);
			bool flag2 = !flag && knowledge > 0f;
			float num = Text.CalcHeight(conc.LabelCap, width);
			if (flag2)
			{
				num = num;
			}
			Rect rect = new Rect(x, y, width, num);
			if (flag2)
			{
				Rect rect2 = new Rect(rect);
				rect2.yMin += 1f;
				rect2.yMax -= 1f;
				Widgets.FillableBar(rect2, PlayerKnowledgeDatabase.GetKnowledge(conc), LearningReadout.ProgressBarFillTex, LearningReadout.ProgressBarBGTex, false);
			}
			if (flag)
			{
				GUI.DrawTexture(rect, BaseContent.GreyTex);
			}
			if (this.selectedConcept == conc)
			{
				GUI.DrawTexture(rect, TexUI.HighlightSelectedTex);
			}
			Widgets.DrawHighlightIfMouseover(rect);
			if (this.MatchesSearchString(conc))
			{
				Widgets.DrawHighlight(rect);
			}
			Widgets.Label(rect, conc.LabelCap);
			if (Mouse.IsOver(rect) && this.selectedConcept == null)
			{
				this.mouseoverConcept = conc;
			}
			if (Widgets.ButtonInvisible(rect, true))
			{
				if (this.selectedConcept == conc)
				{
					this.selectedConcept = null;
				}
				else
				{
					this.selectedConcept = conc;
				}
				SoundDefOf.PageChange.PlayOneShotOnCamera();
			}
			return rect;
		}

		private Rect DrawInfoPane(ConceptDef conc)
		{
			float knowledge = PlayerKnowledgeDatabase.GetKnowledge(conc);
			bool complete = PlayerKnowledgeDatabase.IsComplete(conc);
			bool drawProgressBar = !complete && knowledge > 0f;
			Text.Font = GameFont.Medium;
			float titleHeight = Text.CalcHeight(conc.LabelCap, 276f);
			Text.Font = GameFont.Small;
			float textHeight = Text.CalcHeight(conc.HelpTextAdjusted, 296f);
			float num = titleHeight + textHeight + 14f + 5f;
			if (this.selectedConcept == conc)
			{
				num += 40f;
			}
			if (drawProgressBar)
			{
				num += 30f;
			}
			Rect outRect = new Rect((float)UI.screenWidth - 8f - 200f - 8f - 310f, 8f, 310f, num);
			Rect outRect2 = outRect;
			Find.WindowStack.ImmediateWindow(987612111, outRect, WindowLayer.Super, delegate
			{
				outRect = outRect.AtZero();
				Rect rect = outRect.ContractedBy(7f);
				Widgets.DrawShadowAround(outRect);
				Widgets.DrawWindowBackgroundTutor(outRect);
				Rect rect2 = rect;
				rect2.width -= 20f;
				rect2.height = titleHeight + 5f;
				Text.Font = GameFont.Medium;
				Widgets.Label(rect2, conc.LabelCap);
				Text.Font = GameFont.Small;
				Rect rect3 = rect;
				rect3.yMin = rect2.yMax;
				rect3.height = textHeight;
				Widgets.Label(rect3, conc.HelpTextAdjusted);
				if (drawProgressBar)
				{
					Rect rect4 = rect;
					rect4.yMin = rect3.yMax;
					rect4.height = 30f;
					Widgets.FillableBar(rect4, PlayerKnowledgeDatabase.GetKnowledge(conc), LearningReadout.ProgressBarFillTex);
				}
				if (this.selectedConcept == conc)
				{
					if (Widgets.CloseButtonFor(outRect))
					{
						this.selectedConcept = null;
						SoundDefOf.PageChange.PlayOneShotOnCamera();
					}
					Rect rect5 = new Rect(rect.center.x - 70f, rect.yMax - 30f, 140f, 30f);
					if (!complete)
					{
						if (Widgets.ButtonText(rect5, "MarkLearned".Translate(), true, false, true))
						{
							this.selectedConcept = null;
							SoundDefOf.PageChange.PlayOneShotOnCamera();
							PlayerKnowledgeDatabase.SetKnowledge(conc, 1f);
						}
					}
					else
					{
						GUI.color = new Color(1f, 1f, 1f, 0.5f);
						Text.Anchor = TextAnchor.MiddleCenter;
						Widgets.Label(rect5, "AlreadyLearned".Translate());
						Text.Anchor = TextAnchor.UpperLeft;
						GUI.color = Color.white;
					}
				}
			}, false, false, 1f);
			return outRect2;
		}
	}
}
