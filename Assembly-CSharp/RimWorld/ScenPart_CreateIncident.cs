using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld
{
	internal class ScenPart_CreateIncident : ScenPart_IncidentBase
	{
		private const float IntervalMidpoint = 30f;

		private const float IntervalDeviation = 15f;

		private float intervalDays;

		private bool repeat;

		private string intervalDaysBuffer;

		private float occurTick;

		private bool isFinished;

		protected override string IncidentTag
		{
			get
			{
				return "CreateIncident";
			}
		}

		private float IntervalTicks
		{
			get
			{
				return 60000f * this.intervalDays;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<float>(ref this.intervalDays, "intervalDays", 0f, false);
			Scribe_Values.LookValue<bool>(ref this.repeat, "repeat", false, false);
			Scribe_Values.LookValue<float>(ref this.occurTick, "occurTick", 0f, false);
			Scribe_Values.LookValue<bool>(ref this.isFinished, "isFinished", false, false);
		}

		public override void DoEditInterface(Listing_ScenEdit listing)
		{
			Rect scenPartRect = listing.GetScenPartRect(this, ScenPart.RowHeight * 3f);
			Rect rect = new Rect(scenPartRect.x, scenPartRect.y, scenPartRect.width, scenPartRect.height / 3f);
			Rect rect2 = new Rect(scenPartRect.x, scenPartRect.y + scenPartRect.height / 3f, scenPartRect.width, scenPartRect.height / 3f);
			Rect rect3 = new Rect(scenPartRect.x, scenPartRect.y + scenPartRect.height * 2f / 3f, scenPartRect.width, scenPartRect.height / 3f);
			base.DoIncidentEditInterface(rect);
			Widgets.TextFieldNumericLabeled<float>(rect2, "intervalDays".Translate(), ref this.intervalDays, ref this.intervalDaysBuffer, 0f, 1E+09f);
			Widgets.CheckboxLabeled(rect3, "repeat".Translate(), ref this.repeat, false);
		}

		public override void Randomize()
		{
			base.Randomize();
			this.intervalDays = 15f * Rand.Gaussian(0f, 1f) + 30f;
			if (this.intervalDays < 0f)
			{
				this.intervalDays = 0f;
			}
			this.repeat = (Rand.Range(0, 100) < 50);
		}

		[DebuggerHidden]
		protected override IEnumerable<IncidentDef> RandomizableIncidents()
		{
			ScenPart_CreateIncident.<RandomizableIncidents>c__IteratorFF <RandomizableIncidents>c__IteratorFF = new ScenPart_CreateIncident.<RandomizableIncidents>c__IteratorFF();
			ScenPart_CreateIncident.<RandomizableIncidents>c__IteratorFF expr_07 = <RandomizableIncidents>c__IteratorFF;
			expr_07.$PC = -2;
			return expr_07;
		}

		public override void PostGameStart()
		{
			base.PostGameStart();
			this.occurTick = (float)Find.TickManager.TicksGame + this.IntervalTicks;
		}

		public override void Tick()
		{
			base.Tick();
			if (Find.AnyPlayerHomeMap == null)
			{
				return;
			}
			if (this.isFinished)
			{
				return;
			}
			if (this.incident == null)
			{
				Log.Error("Trying to tick ScenPart_CreateIncident but the incident is null");
				this.isFinished = true;
				return;
			}
			if ((float)Find.TickManager.TicksGame >= this.occurTick)
			{
				IncidentParms parms = StorytellerUtility.DefaultParmsNow(Find.Storyteller.def, this.incident.category, (from x in Find.Maps
				where x.IsPlayerHome
				select x).RandomElement<Map>());
				if (!this.incident.Worker.TryExecute(parms))
				{
					this.isFinished = true;
					return;
				}
				if (this.repeat && this.intervalDays > 0f)
				{
					this.occurTick += this.IntervalTicks;
				}
				else
				{
					this.isFinished = true;
				}
			}
		}
	}
}
