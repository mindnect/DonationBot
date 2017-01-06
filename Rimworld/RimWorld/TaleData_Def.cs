using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.Grammar;

namespace RimWorld
{
	public class TaleData_Def : TaleData
	{
		public Def def;

		private string tmpDefName;

		private Type tmpDefType;

		public override void ExposeData()
		{
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				this.tmpDefName = this.def.defName;
				this.tmpDefType = this.def.GetType();
			}
			Scribe_Values.LookValue<string>(ref this.tmpDefName, "defName", null, false);
			Scribe_Values.LookValue<Type>(ref this.tmpDefType, "defType", null, false);
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				this.def = GenDefDatabase.GetDef(this.tmpDefType, this.tmpDefName, true);
			}
		}

		[DebuggerHidden]
		public override IEnumerable<Rule> GetRules(string prefix)
		{
			TaleData_Def.<GetRules>c__Iterator112 <GetRules>c__Iterator = new TaleData_Def.<GetRules>c__Iterator112();
			<GetRules>c__Iterator.prefix = prefix;
			<GetRules>c__Iterator.<$>prefix = prefix;
			<GetRules>c__Iterator.<>f__this = this;
			TaleData_Def.<GetRules>c__Iterator112 expr_1C = <GetRules>c__Iterator;
			expr_1C.$PC = -2;
			return expr_1C;
		}

		public static TaleData_Def GenerateFrom(Def def)
		{
			return new TaleData_Def
			{
				def = def
			};
		}
	}
}
