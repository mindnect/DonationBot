using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public class WorkGiverDef : Def
	{
		public Type giverClass;

		public WorkTypeDef workType;

		public int priorityInType;

		[MustTranslate]
		public string verb;

		[MustTranslate]
		public string gerund;

		public bool scanThings = true;

		public bool scanCells;

		public bool emergency;

		public List<PawnCapacityDef> requiredCapacities = new List<PawnCapacityDef>();

		public bool directOrderable = true;

		public bool prioritizeSustains;

		public bool canBeDoneByNonColonists;

		public List<ThingDef> fixedBillGiverDefs;

		public bool billGiversAllHumanlikes;

		public bool billGiversAllHumanlikesCorpses;

		public bool billGiversAllMechanoids;

		public bool billGiversAllMechanoidsCorpses;

		public bool billGiversAllAnimals;

		public bool billGiversAllAnimalsCorpses;

		[Unsaved]
		private WorkGiver workerInt;

		public WorkGiver Worker
		{
			get
			{
				if (this.workerInt == null)
				{
					this.workerInt = (WorkGiver)Activator.CreateInstance(this.giverClass);
					this.workerInt.def = this;
				}
				return this.workerInt;
			}
		}

		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			WorkGiverDef.<ConfigErrors>c__Iterator1C6 <ConfigErrors>c__Iterator1C = new WorkGiverDef.<ConfigErrors>c__Iterator1C6();
			<ConfigErrors>c__Iterator1C.<>f__this = this;
			WorkGiverDef.<ConfigErrors>c__Iterator1C6 expr_0E = <ConfigErrors>c__Iterator1C;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
