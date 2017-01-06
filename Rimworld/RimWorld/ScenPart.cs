using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public abstract class ScenPart : IExposable
	{
		public ScenPartDef def;

		public bool visible = true;

		public bool summarized;

		public static float RowHeight
		{
			get
			{
				return Text.LineHeight;
			}
		}

		public virtual string Label
		{
			get
			{
				return this.def.LabelCap;
			}
		}

		public virtual void ExposeData()
		{
			Scribe_Defs.LookDef<ScenPartDef>(ref this.def, "def");
		}

		public ScenPart CopyForEditing()
		{
			ScenPart scenPart = this.CopyForEditingInner();
			scenPart.def = this.def;
			return scenPart;
		}

		protected virtual ScenPart CopyForEditingInner()
		{
			return (ScenPart)base.MemberwiseClone();
		}

		public virtual void DoEditInterface(Listing_ScenEdit listing)
		{
			listing.GetScenPartRect(this, ScenPart.RowHeight);
		}

		public virtual string Summary(Scenario scen)
		{
			return this.def.description;
		}

		[DebuggerHidden]
		public virtual IEnumerable<string> GetSummaryListEntries(string tag)
		{
			ScenPart.<GetSummaryListEntries>c__IteratorF7 <GetSummaryListEntries>c__IteratorF = new ScenPart.<GetSummaryListEntries>c__IteratorF7();
			ScenPart.<GetSummaryListEntries>c__IteratorF7 expr_07 = <GetSummaryListEntries>c__IteratorF;
			expr_07.$PC = -2;
			return expr_07;
		}

		public virtual void Randomize()
		{
		}

		public virtual bool TryMerge(ScenPart other)
		{
			return false;
		}

		public virtual bool CanCoexistWith(ScenPart other)
		{
			return true;
		}

		[DebuggerHidden]
		public virtual IEnumerable<Page> GetConfigPages()
		{
			ScenPart.<GetConfigPages>c__IteratorF8 <GetConfigPages>c__IteratorF = new ScenPart.<GetConfigPages>c__IteratorF8();
			ScenPart.<GetConfigPages>c__IteratorF8 expr_07 = <GetConfigPages>c__IteratorF;
			expr_07.$PC = -2;
			return expr_07;
		}

		public virtual bool AllowPlayerStartingPawn(Pawn pawn)
		{
			return true;
		}

		public virtual void Notify_PawnGenerated(Pawn pawn, PawnGenerationContext context)
		{
		}

		public virtual void Notify_PawnDied(Corpse corpse)
		{
		}

		public virtual void PreConfigure()
		{
		}

		public virtual void PostWorldLoad()
		{
		}

		public virtual void PreMapGenerate()
		{
		}

		[DebuggerHidden]
		public virtual IEnumerable<Thing> PlayerStartingThings()
		{
			ScenPart.<PlayerStartingThings>c__IteratorF9 <PlayerStartingThings>c__IteratorF = new ScenPart.<PlayerStartingThings>c__IteratorF9();
			ScenPart.<PlayerStartingThings>c__IteratorF9 expr_07 = <PlayerStartingThings>c__IteratorF;
			expr_07.$PC = -2;
			return expr_07;
		}

		public virtual void GenerateIntoMap(Map map)
		{
		}

		public virtual void PostMapGenerate(Map map)
		{
		}

		public virtual void PostGameStart()
		{
		}

		public virtual void Tick()
		{
		}

		[DebuggerHidden]
		public virtual IEnumerable<string> ConfigErrors()
		{
			ScenPart.<ConfigErrors>c__IteratorFA <ConfigErrors>c__IteratorFA = new ScenPart.<ConfigErrors>c__IteratorFA();
			<ConfigErrors>c__IteratorFA.<>f__this = this;
			ScenPart.<ConfigErrors>c__IteratorFA expr_0E = <ConfigErrors>c__IteratorFA;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
