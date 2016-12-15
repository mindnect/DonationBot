using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Verse
{
	public sealed class DesignationManager : IExposable
	{
		public Map map;

		public List<Designation> allDesignations = new List<Designation>();

		public DesignationManager(Map map)
		{
			this.map = map;
		}

		public void ExposeData()
		{
			Scribe_Collections.LookList<Designation>(ref this.allDesignations, "allDesignations", LookMode.Deep, new object[0]);
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				for (int i = 0; i < this.allDesignations.Count; i++)
				{
					this.allDesignations[i].designationManager = this;
				}
			}
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				for (int j = this.allDesignations.Count - 1; j >= 0; j--)
				{
					TargetType targetType = this.allDesignations[j].def.targetType;
					if (targetType != TargetType.Thing)
					{
						if (targetType == TargetType.Cell)
						{
							if (!this.allDesignations[j].target.Cell.IsValid)
							{
								Log.Error("Cell-needing designation " + this.allDesignations[j] + " had no cell target. Removing...");
								this.allDesignations.RemoveAt(j);
							}
						}
					}
					else if (!this.allDesignations[j].target.HasThing)
					{
						Log.Error("Thing-needing designation " + this.allDesignations[j] + " had no thing target. Removing...");
						this.allDesignations.RemoveAt(j);
					}
				}
			}
		}

		public void DrawDesignations()
		{
			for (int i = 0; i < this.allDesignations.Count; i++)
			{
				this.allDesignations[i].DesignationDraw();
			}
		}

		public void AddDesignation(Designation newDes)
		{
			if (newDes.def.targetType == TargetType.Cell && this.DesignationAt(newDes.target.Cell, newDes.def) != null)
			{
				Log.Error("Tried to double-add designation at location " + newDes.target);
				return;
			}
			if (newDes.def.targetType == TargetType.Thing && this.DesignationOn(newDes.target.Thing, newDes.def) != null)
			{
				Log.Error("Tried to double-add designation on Thing " + newDes.target);
				return;
			}
			if (newDes.def.targetType == TargetType.Thing)
			{
				newDes.target.Thing.SetForbidden(false, false);
			}
			this.allDesignations.Add(newDes);
			newDes.designationManager = this;
			newDes.Notify_Added();
			MoteMaker.ThrowMetaPuffs(newDes.target.ToTargetInfo(this.map));
		}

		public Designation DesignationOn(Thing t)
		{
			for (int i = 0; i < this.allDesignations.Count; i++)
			{
				Designation designation = this.allDesignations[i];
				if (designation.target.Thing == t)
				{
					return designation;
				}
			}
			return null;
		}

		public Designation DesignationOn(Thing t, DesignationDef def)
		{
			if (def.targetType == TargetType.Cell)
			{
				Log.Error("Designations of type " + def.defName + " are indexed by location only and you are trying to get one on a Thing.");
				return null;
			}
			for (int i = 0; i < this.allDesignations.Count; i++)
			{
				Designation designation = this.allDesignations[i];
				if (designation.target.Thing == t && designation.def == def)
				{
					return designation;
				}
			}
			return null;
		}

		public Designation DesignationAt(IntVec3 c, DesignationDef def)
		{
			if (def.targetType == TargetType.Thing)
			{
				Log.Error("Designations of type " + def.defName + " are indexed by Thing only and you are trying to get one on a location.");
				return null;
			}
			for (int i = 0; i < this.allDesignations.Count; i++)
			{
				Designation designation = this.allDesignations[i];
				if (designation.target.Cell == c && designation.def == def)
				{
					return designation;
				}
			}
			return null;
		}

		[DebuggerHidden]
		public IEnumerable<Designation> AllDesignationsOn(Thing t)
		{
			DesignationManager.<AllDesignationsOn>c__Iterator1CE <AllDesignationsOn>c__Iterator1CE = new DesignationManager.<AllDesignationsOn>c__Iterator1CE();
			<AllDesignationsOn>c__Iterator1CE.t = t;
			<AllDesignationsOn>c__Iterator1CE.<$>t = t;
			<AllDesignationsOn>c__Iterator1CE.<>f__this = this;
			DesignationManager.<AllDesignationsOn>c__Iterator1CE expr_1C = <AllDesignationsOn>c__Iterator1CE;
			expr_1C.$PC = -2;
			return expr_1C;
		}

		[DebuggerHidden]
		public IEnumerable<Designation> AllDesignationsAt(IntVec3 c)
		{
			DesignationManager.<AllDesignationsAt>c__Iterator1CF <AllDesignationsAt>c__Iterator1CF = new DesignationManager.<AllDesignationsAt>c__Iterator1CF();
			<AllDesignationsAt>c__Iterator1CF.c = c;
			<AllDesignationsAt>c__Iterator1CF.<$>c = c;
			<AllDesignationsAt>c__Iterator1CF.<>f__this = this;
			DesignationManager.<AllDesignationsAt>c__Iterator1CF expr_1C = <AllDesignationsAt>c__Iterator1CF;
			expr_1C.$PC = -2;
			return expr_1C;
		}

		[DebuggerHidden]
		public IEnumerable<Designation> DesignationsOfDef(DesignationDef def)
		{
			DesignationManager.<DesignationsOfDef>c__Iterator1D0 <DesignationsOfDef>c__Iterator1D = new DesignationManager.<DesignationsOfDef>c__Iterator1D0();
			<DesignationsOfDef>c__Iterator1D.def = def;
			<DesignationsOfDef>c__Iterator1D.<$>def = def;
			<DesignationsOfDef>c__Iterator1D.<>f__this = this;
			DesignationManager.<DesignationsOfDef>c__Iterator1D0 expr_1C = <DesignationsOfDef>c__Iterator1D;
			expr_1C.$PC = -2;
			return expr_1C;
		}

		public void RemoveDesignation(Designation des)
		{
			des.Notify_Removing();
			this.allDesignations.Remove(des);
		}

		public void RemoveAllDesignationsOn(Thing t, bool standardCanceling = false)
		{
			for (int i = 0; i < this.allDesignations.Count; i++)
			{
				Designation designation = this.allDesignations[i];
				if (!standardCanceling || designation.def.designateCancelable)
				{
					if (designation.target.Thing == t)
					{
						designation.Notify_Removing();
					}
				}
			}
			this.allDesignations.RemoveAll((Designation d) => d.target.Thing == t);
		}

		public void Notify_BuildingDespawned(Thing b)
		{
			CellRect cellRect = b.OccupiedRect();
			for (int i = this.allDesignations.Count - 1; i >= 0; i--)
			{
				Designation designation = this.allDesignations[i];
				if (cellRect.Contains(designation.target.Cell) && designation.def.removeIfBuildingDespawned)
				{
					this.RemoveDesignation(designation);
				}
			}
		}
	}
}
