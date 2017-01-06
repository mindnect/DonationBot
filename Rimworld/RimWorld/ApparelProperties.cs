using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
	public class ApparelProperties
	{
		public List<BodyPartGroupDef> bodyPartGroups = new List<BodyPartGroupDef>();

		public List<ApparelLayer> layers = new List<ApparelLayer>();

		public string wornGraphicPath = string.Empty;

		public float commonality = 100f;

		public List<string> tags = new List<string>();

		public List<string> defaultOutfitTags;

		public float wearPerDay = 0.4f;

		public bool careIfWornByCorpse = true;

		[Unsaved]
		private float cachedHumanBodyCoverage = -1f;

		public ApparelLayer LastLayer
		{
			get
			{
				return this.layers[this.layers.Count - 1];
			}
		}

		public float HumanBodyCoverage
		{
			get
			{
				if (this.cachedHumanBodyCoverage < 0f)
				{
					this.cachedHumanBodyCoverage = 0f;
					List<BodyPartRecord> allParts = BodyDefOf.Human.AllParts;
					for (int i = 0; i < allParts.Count; i++)
					{
						if (this.CoversBodyPart(allParts[i]))
						{
							this.cachedHumanBodyCoverage += allParts[i].absoluteFleshCoverage;
						}
					}
				}
				return this.cachedHumanBodyCoverage;
			}
		}

		[DebuggerHidden]
		public IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			ApparelProperties.<ConfigErrors>c__Iterator73 <ConfigErrors>c__Iterator = new ApparelProperties.<ConfigErrors>c__Iterator73();
			<ConfigErrors>c__Iterator.parentDef = parentDef;
			<ConfigErrors>c__Iterator.<$>parentDef = parentDef;
			<ConfigErrors>c__Iterator.<>f__this = this;
			ApparelProperties.<ConfigErrors>c__Iterator73 expr_1C = <ConfigErrors>c__Iterator;
			expr_1C.$PC = -2;
			return expr_1C;
		}

		public bool CoversBodyPart(BodyPartRecord partRec)
		{
			for (int i = 0; i < partRec.groups.Count; i++)
			{
				if (this.bodyPartGroups.Contains(partRec.groups[i]))
				{
					return true;
				}
			}
			return false;
		}

		public string GetCoveredOuterPartsString(BodyDef body)
		{
			IEnumerable<BodyPartRecord> source = from x in body.AllParts
			where x.depth == BodyPartDepth.Outside && x.groups.Any((BodyPartGroupDef y) => this.bodyPartGroups.Contains(y))
			select x;
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (BodyPartRecord current in source.Distinct<BodyPartRecord>())
			{
				if (!flag)
				{
					stringBuilder.Append(", ");
				}
				flag = false;
				stringBuilder.Append(current.def.label);
			}
			return stringBuilder.ToString().CapitalizeFirst();
		}
	}
}
