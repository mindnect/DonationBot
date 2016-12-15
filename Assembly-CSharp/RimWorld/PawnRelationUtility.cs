using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public static class PawnRelationUtility
	{
		[DebuggerHidden]
		public static IEnumerable<PawnRelationDef> GetRelations(this Pawn me, Pawn other)
		{
			PawnRelationUtility.<GetRelations>c__IteratorD5 <GetRelations>c__IteratorD = new PawnRelationUtility.<GetRelations>c__IteratorD5();
			<GetRelations>c__IteratorD.me = me;
			<GetRelations>c__IteratorD.other = other;
			<GetRelations>c__IteratorD.<$>me = me;
			<GetRelations>c__IteratorD.<$>other = other;
			PawnRelationUtility.<GetRelations>c__IteratorD5 expr_23 = <GetRelations>c__IteratorD;
			expr_23.$PC = -2;
			return expr_23;
		}

		public static PawnRelationDef GetMostImportantRelation(this Pawn me, Pawn other)
		{
			PawnRelationDef pawnRelationDef = null;
			foreach (PawnRelationDef current in me.GetRelations(other))
			{
				if (pawnRelationDef == null || current.importance > pawnRelationDef.importance)
				{
					pawnRelationDef = current;
				}
			}
			return pawnRelationDef;
		}

		public static void Notify_PawnsSeenByPlayer(IEnumerable<Pawn> seenPawns, out string pawnRelationsInfo, bool informEvenIfSeenBefore = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			IEnumerable<Pawn> enumerable = from x in seenPawns
			where x.RaceProps.IsFlesh
			select x;
			IEnumerable<Pawn> enumerable2 = from x in PawnsFinder.AllMapsCaravansAndTravelingTransportPods
			where x.RaceProps.Humanlike && (x.Faction == Faction.OfPlayer || x.HostFaction == Faction.OfPlayer) && x.relations.everSeenByPlayer
			select x;
			if (!informEvenIfSeenBefore)
			{
				enumerable = from x in enumerable
				where !x.relations.everSeenByPlayer
				select x;
			}
			bool flag = false;
			foreach (Pawn current in enumerable)
			{
				bool flag2 = false;
				foreach (Pawn current2 in enumerable2)
				{
					if (current != current2)
					{
						PawnRelationDef mostImportantRelation = current.GetMostImportantRelation(current2);
						if (mostImportantRelation != null)
						{
							if (!flag2)
							{
								flag2 = true;
								if (flag)
								{
									stringBuilder.AppendLine();
								}
								stringBuilder.AppendLine(current.KindLabel.CapitalizeFirst() + " " + current.LabelShort + ":");
							}
							flag = true;
							stringBuilder.AppendLine(string.Concat(new string[]
							{
								"  ",
								mostImportantRelation.GetGenderSpecificLabelCap(current2),
								" - ",
								current2.KindLabel,
								" ",
								current2.LabelShort
							}));
							current.relations.everSeenByPlayer = true;
						}
					}
				}
			}
			if (flag)
			{
				pawnRelationsInfo = stringBuilder.ToString().TrimEndNewlines();
			}
			else
			{
				pawnRelationsInfo = null;
			}
		}

		public static void Notify_PawnsSeenByPlayer(IEnumerable<Pawn> seenPawns, ref string letterLabel, ref string letterText, string relationsInfoHeader, bool informEvenIfSeenBefore = false)
		{
			string text;
			PawnRelationUtility.Notify_PawnsSeenByPlayer(seenPawns, out text, informEvenIfSeenBefore);
			if (!text.NullOrEmpty())
			{
				if (letterLabel.NullOrEmpty())
				{
					letterLabel = "LetterLabelNoticedRelatedPawns".Translate();
				}
				else
				{
					letterLabel = letterLabel + " " + "RelationshipAppendedLetterSuffix".Translate();
				}
				if (!letterText.NullOrEmpty())
				{
					letterText += "\n\n";
				}
				letterText = letterText + relationsInfoHeader + "\n\n" + text;
			}
		}

		public static bool TryAppendRelationsWithColonistsInfo(ref string text, Pawn pawn)
		{
			string text2 = null;
			return PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, ref text2, pawn);
		}

		public static bool TryAppendRelationsWithColonistsInfo(ref string text, ref string title, Pawn pawn)
		{
			Pawn mostImportantColonyRelative = PawnRelationUtility.GetMostImportantColonyRelative(pawn);
			if (mostImportantColonyRelative == null)
			{
				return false;
			}
			if (title != null)
			{
				title = title + " " + "RelationshipAppendedLetterSuffix".Translate();
			}
			string genderSpecificLabel = mostImportantColonyRelative.GetMostImportantRelation(pawn).GetGenderSpecificLabel(pawn);
			string text2 = "\n\n";
			if (mostImportantColonyRelative.IsColonist)
			{
				text2 += "RelationshipAppendedLetterTextColonist".Translate(new object[]
				{
					mostImportantColonyRelative.LabelShort,
					genderSpecificLabel
				});
			}
			else
			{
				text2 += "RelationshipAppendedLetterTextPrisoner".Translate(new object[]
				{
					mostImportantColonyRelative.LabelShort,
					genderSpecificLabel
				});
			}
			text += text2.AdjustedFor(pawn);
			return true;
		}

		public static Pawn GetMostImportantColonyRelative(Pawn pawn)
		{
			IEnumerable<Pawn> enumerable = from x in PawnsFinder.AllMaps_FreeColonistsAndPrisoners
			where x.relations.everSeenByPlayer
			select x;
			float num = 0f;
			Pawn pawn2 = null;
			foreach (Pawn current in enumerable)
			{
				PawnRelationDef mostImportantRelation = pawn.GetMostImportantRelation(current);
				if (mostImportantRelation != null)
				{
					if (pawn2 == null || mostImportantRelation.importance > num)
					{
						num = mostImportantRelation.importance;
						pawn2 = current;
					}
				}
			}
			return pawn2;
		}

		public static float MaxPossibleBioAgeAt(float myBiologicalAge, float myChronologicalAge, float atChronologicalAge)
		{
			float num = Mathf.Min(myBiologicalAge, myChronologicalAge - atChronologicalAge);
			if (num < 0f)
			{
				return -1f;
			}
			return num;
		}

		public static float MinPossibleBioAgeAt(float myBiologicalAge, float atChronologicalAge)
		{
			return Mathf.Max(myBiologicalAge - atChronologicalAge, 0f);
		}
	}
}
