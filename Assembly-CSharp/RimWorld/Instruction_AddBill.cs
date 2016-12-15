using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;

namespace RimWorld
{
	public class Instruction_AddBill : Lesson_Instruction
	{
		protected override float ProgressPercent
		{
			get
			{
				int num = this.def.recipeTargetCount + 1;
				int num2 = 0;
				Bill_Production bill_Production = this.RelevantBill();
				if (bill_Production != null)
				{
					num2++;
					if (bill_Production.repeatMode == BillRepeatMode.RepeatCount)
					{
						num2 += bill_Production.repeatCount;
					}
				}
				return (float)num2 / (float)num;
			}
		}

		private Bill_Production RelevantBill()
		{
			if (Find.Selector.SingleSelectedThing != null && Find.Selector.SingleSelectedThing.def == this.def.thingDef)
			{
				IBillGiver billGiver = Find.Selector.SingleSelectedThing as IBillGiver;
				if (billGiver != null)
				{
					return (Bill_Production)billGiver.BillStack.Bills.FirstOrDefault((Bill b) => b.recipe == this.def.recipeDef);
				}
			}
			return null;
		}

		[DebuggerHidden]
		private IEnumerable<Thing> ThingsToSelect()
		{
			Instruction_AddBill.<ThingsToSelect>c__Iterator182 <ThingsToSelect>c__Iterator = new Instruction_AddBill.<ThingsToSelect>c__Iterator182();
			<ThingsToSelect>c__Iterator.<>f__this = this;
			Instruction_AddBill.<ThingsToSelect>c__Iterator182 expr_0E = <ThingsToSelect>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		public override void LessonOnGUI()
		{
			foreach (Thing current in this.ThingsToSelect())
			{
				TutorUtility.DrawLabelOnThingOnGUI(current, this.def.onMapInstruction);
			}
			if (this.RelevantBill() == null)
			{
				UIHighlighter.HighlightTag("AddBill");
			}
			base.LessonOnGUI();
		}

		public override void LessonUpdate()
		{
			foreach (Thing current in this.ThingsToSelect())
			{
				GenDraw.DrawArrowPointingAt(current.DrawPos, false);
			}
			if (this.ProgressPercent > 0.999f)
			{
				Find.ActiveLesson.Deactivate();
			}
		}
	}
}
