using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Verse.Grammar
{
	public static class GrammarUtility
	{
		[DebuggerHidden]
		public static IEnumerable<Rule> RulesForPawn(string prefix, Name name, PawnKindDef kind, Gender gender, Faction faction = null)
		{
			GrammarUtility.<RulesForPawn>c__Iterator1C8 <RulesForPawn>c__Iterator1C = new GrammarUtility.<RulesForPawn>c__Iterator1C8();
			<RulesForPawn>c__Iterator1C.name = name;
			<RulesForPawn>c__Iterator1C.kind = kind;
			<RulesForPawn>c__Iterator1C.prefix = prefix;
			<RulesForPawn>c__Iterator1C.faction = faction;
			<RulesForPawn>c__Iterator1C.gender = gender;
			<RulesForPawn>c__Iterator1C.<$>name = name;
			<RulesForPawn>c__Iterator1C.<$>kind = kind;
			<RulesForPawn>c__Iterator1C.<$>prefix = prefix;
			<RulesForPawn>c__Iterator1C.<$>faction = faction;
			<RulesForPawn>c__Iterator1C.<$>gender = gender;
			GrammarUtility.<RulesForPawn>c__Iterator1C8 expr_4F = <RulesForPawn>c__Iterator1C;
			expr_4F.$PC = -2;
			return expr_4F;
		}
	}
}
