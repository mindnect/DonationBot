using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Verse
{
	public struct CellRect : IEquatable<CellRect>
	{
		public struct Enumerator : IEnumerator, IDisposable, IEnumerator<IntVec3>
		{
			private CellRect ir;

			private int x;

			private int z;

			object IEnumerator.Current
			{
				get
				{
					return new IntVec3(this.x, 0, this.z);
				}
			}

			public IntVec3 Current
			{
				get
				{
					return new IntVec3(this.x, 0, this.z);
				}
			}

			public Enumerator(CellRect ir)
			{
				this.ir = ir;
				this.x = ir.minX - 1;
				this.z = ir.minZ;
			}

			void IDisposable.Dispose()
			{
			}

			public bool MoveNext()
			{
				this.x++;
				if (this.x > this.ir.maxX)
				{
					this.x = this.ir.minX;
					this.z++;
				}
				return this.z <= this.ir.maxZ;
			}

			public void Reset()
			{
				this.x = this.ir.minX - 1;
				this.z = this.ir.minZ;
			}
		}

		public struct CellRectIterator
		{
			private int maxX;

			private int minX;

			private int maxZ;

			private int x;

			private int z;

			public IntVec3 Current
			{
				get
				{
					return new IntVec3(this.x, 0, this.z);
				}
			}

			public CellRectIterator(CellRect cr)
			{
				this.minX = cr.minX;
				this.maxX = cr.maxX;
				this.maxZ = cr.maxZ;
				this.x = cr.minX;
				this.z = cr.minZ;
			}

			public void MoveNext()
			{
				this.x++;
				if (this.x > this.maxX)
				{
					this.x = this.minX;
					this.z++;
				}
			}

			public bool Done()
			{
				return this.z > this.maxZ;
			}
		}

		public int minX;

		public int maxX;

		public int minZ;

		public int maxZ;

		public int Area
		{
			get
			{
				return this.Width * this.Height;
			}
		}

		public int Width
		{
			get
			{
				return this.maxX - this.minX + 1;
			}
			set
			{
				this.maxX = this.minX + value - 1;
			}
		}

		public int Height
		{
			get
			{
				return this.maxZ - this.minZ + 1;
			}
			set
			{
				this.maxZ = this.minZ + value - 1;
			}
		}

		public IntVec3 BottomLeft
		{
			get
			{
				return new IntVec3(this.minX, 0, this.minZ);
			}
		}

		public IntVec3 TopRight
		{
			get
			{
				return new IntVec3(this.maxX, 0, this.maxZ);
			}
		}

		public IntVec3 RandomCell
		{
			get
			{
				return new IntVec3(Rand.RangeInclusive(this.minX, this.maxX), 0, Rand.RangeInclusive(this.minZ, this.maxZ));
			}
		}

		public IntVec3 CenterCell
		{
			get
			{
				return new IntVec3(this.minX + this.Width / 2, 0, this.minZ + this.Height / 2);
			}
		}

		public Vector3 CenterVector3
		{
			get
			{
				return new Vector3((float)this.minX + (float)this.Width / 2f, 0f, (float)this.minZ + (float)this.Height / 2f);
			}
		}

		public Vector3 RandomVector3
		{
			get
			{
				return new Vector3(Rand.Range((float)this.minX, (float)this.maxX), 0f, Rand.Range((float)this.minZ, (float)this.maxZ));
			}
		}

		public IEnumerable<IntVec3> Cells
		{
			get
			{
				CellRect.<>c__Iterator208 <>c__Iterator = new CellRect.<>c__Iterator208();
				<>c__Iterator.<>f__this = this;
				CellRect.<>c__Iterator208 expr_13 = <>c__Iterator;
				expr_13.$PC = -2;
				return expr_13;
			}
		}

		public IEnumerable<IntVec2> Cells2D
		{
			get
			{
				CellRect.<>c__Iterator209 <>c__Iterator = new CellRect.<>c__Iterator209();
				<>c__Iterator.<>f__this = this;
				CellRect.<>c__Iterator209 expr_13 = <>c__Iterator;
				expr_13.$PC = -2;
				return expr_13;
			}
		}

		public IEnumerable<IntVec3> EdgeCells
		{
			get
			{
				CellRect.<>c__Iterator20A <>c__Iterator20A = new CellRect.<>c__Iterator20A();
				<>c__Iterator20A.<>f__this = this;
				CellRect.<>c__Iterator20A expr_13 = <>c__Iterator20A;
				expr_13.$PC = -2;
				return expr_13;
			}
		}

		public CellRect(int minX, int minZ, int width, int height)
		{
			this.minX = minX;
			this.minZ = minZ;
			this.maxX = minX + width - 1;
			this.maxZ = minZ + height - 1;
		}

		public CellRect.CellRectIterator GetIterator()
		{
			return new CellRect.CellRectIterator(this);
		}

		public static CellRect WholeMap(Map map)
		{
			return new CellRect(0, 0, map.Size.x, map.Size.z);
		}

		public static CellRect FromLimits(int minX, int minZ, int maxX, int maxZ)
		{
			return new CellRect
			{
				minX = Mathf.Min(minX, maxX),
				minZ = Mathf.Min(minZ, maxZ),
				maxX = Mathf.Max(maxX, minX),
				maxZ = Mathf.Max(maxZ, minZ)
			};
		}

		public static CellRect FromLimits(IntVec3 first, IntVec3 second)
		{
			return new CellRect
			{
				minX = Mathf.Min(first.x, second.x),
				minZ = Mathf.Min(first.z, second.z),
				maxX = Mathf.Max(first.x, second.x),
				maxZ = Mathf.Max(first.z, second.z)
			};
		}

		public static CellRect CenteredOn(IntVec3 center, int radius)
		{
			return new CellRect
			{
				minX = center.x - radius,
				maxX = center.x + radius,
				minZ = center.z - radius,
				maxZ = center.z + radius
			};
		}

		public static CellRect SingleCell(IntVec3 c)
		{
			return new CellRect(c.x, c.z, 1, 1);
		}

		public bool InBounds(Map map)
		{
			return this.minX >= 0 && this.minZ >= 0 && this.maxX <= map.Size.x - 1 && this.maxZ <= map.Size.z - 1;
		}

		public bool FullyContainedWithin(CellRect within)
		{
			CellRect rhs = this;
			rhs.ClipInsideRect(within);
			return this == rhs;
		}

		public CellRect ClipInsideMap(Map map)
		{
			if (this.minX < 0)
			{
				this.minX = 0;
			}
			if (this.minZ < 0)
			{
				this.minZ = 0;
			}
			if (this.maxX > map.Size.x - 1)
			{
				this.maxX = map.Size.x - 1;
			}
			if (this.maxZ > map.Size.z - 1)
			{
				this.maxZ = map.Size.z - 1;
			}
			return this;
		}

		public CellRect ClipInsideRect(CellRect otherRect)
		{
			if (this.minX < otherRect.minX)
			{
				this.minX = otherRect.minX;
			}
			if (this.maxX > otherRect.maxX)
			{
				this.maxX = otherRect.maxX;
			}
			if (this.minZ < otherRect.minZ)
			{
				this.minZ = otherRect.minZ;
			}
			if (this.maxZ > otherRect.maxZ)
			{
				this.maxZ = otherRect.maxZ;
			}
			return this;
		}

		public bool Contains(IntVec3 c)
		{
			return c.x >= this.minX && c.x <= this.maxX && c.z >= this.minZ && c.z <= this.maxZ;
		}

		public float ClosestDistSquaredTo(IntVec3 c)
		{
			if (this.Contains(c))
			{
				return 0f;
			}
			if (c.x < this.minX)
			{
				if (c.z < this.minZ)
				{
					return (c - new IntVec3(this.minX, 0, this.minZ)).LengthHorizontalSquared;
				}
				if (c.z > this.maxZ)
				{
					return (c - new IntVec3(this.minX, 0, this.maxZ)).LengthHorizontalSquared;
				}
				return (float)((this.minX - c.x) * (this.minX - c.x));
			}
			else if (c.x > this.maxX)
			{
				if (c.z < this.minZ)
				{
					return (c - new IntVec3(this.maxX, 0, this.minZ)).LengthHorizontalSquared;
				}
				if (c.z > this.maxZ)
				{
					return (c - new IntVec3(this.maxX, 0, this.maxZ)).LengthHorizontalSquared;
				}
				return (float)((c.x - this.maxX) * (c.x - this.maxX));
			}
			else
			{
				if (c.z < this.minZ)
				{
					return (float)((this.minZ - c.z) * (this.minZ - c.z));
				}
				return (float)((c.z - this.maxZ) * (c.z - this.maxZ));
			}
		}

		public IntVec3 ClosestCellTo(IntVec3 c)
		{
			if (this.Contains(c))
			{
				return c;
			}
			if (c.x < this.minX)
			{
				if (c.z < this.minZ)
				{
					return new IntVec3(this.minX, 0, this.minZ);
				}
				if (c.z > this.maxZ)
				{
					return new IntVec3(this.minX, 0, this.maxZ);
				}
				return new IntVec3(this.minX, 0, c.z);
			}
			else if (c.x > this.maxX)
			{
				if (c.z < this.minZ)
				{
					return new IntVec3(this.maxX, 0, this.minZ);
				}
				if (c.z > this.maxZ)
				{
					return new IntVec3(this.maxX, 0, this.maxZ);
				}
				return new IntVec3(this.maxX, 0, c.z);
			}
			else
			{
				if (c.z < this.minZ)
				{
					return new IntVec3(c.x, 0, this.minZ);
				}
				return new IntVec3(c.x, 0, this.maxZ);
			}
		}

		[DebuggerHidden]
		public IEnumerable<IntVec3> GetEdgeCells(Rot4 dir)
		{
			CellRect.<GetEdgeCells>c__Iterator20B <GetEdgeCells>c__Iterator20B = new CellRect.<GetEdgeCells>c__Iterator20B();
			<GetEdgeCells>c__Iterator20B.dir = dir;
			<GetEdgeCells>c__Iterator20B.<$>dir = dir;
			<GetEdgeCells>c__Iterator20B.<>f__this = this;
			CellRect.<GetEdgeCells>c__Iterator20B expr_21 = <GetEdgeCells>c__Iterator20B;
			expr_21.$PC = -2;
			return expr_21;
		}

		public CellRect ExpandedBy(int dist)
		{
			CellRect result = this;
			result.minX -= dist;
			result.minZ -= dist;
			result.maxX += dist;
			result.maxZ += dist;
			return result;
		}

		public CellRect ContractedBy(int dist)
		{
			return this.ExpandedBy(-dist);
		}

		public void DebugDraw()
		{
			float y = Altitudes.AltitudeFor(AltitudeLayer.MetaOverlays);
			Vector3 vector = new Vector3((float)this.minX, y, (float)this.minZ);
			Vector3 vector2 = new Vector3((float)this.minX, y, (float)(this.maxZ + 1));
			Vector3 vector3 = new Vector3((float)(this.maxX + 1), y, (float)(this.maxZ + 1));
			Vector3 vector4 = new Vector3((float)(this.maxX + 1), y, (float)this.minZ);
			GenDraw.DrawLineBetween(vector, vector2);
			GenDraw.DrawLineBetween(vector2, vector3);
			GenDraw.DrawLineBetween(vector3, vector4);
			GenDraw.DrawLineBetween(vector4, vector);
		}

		public IEnumerator<IntVec3> GetEnumerator()
		{
			return new CellRect.Enumerator(this);
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"(",
				this.minX,
				",",
				this.minZ,
				",",
				this.maxX,
				",",
				this.maxZ,
				")"
			});
		}

		public static CellRect FromString(string str)
		{
			str = str.TrimStart(new char[]
			{
				'('
			});
			str = str.TrimEnd(new char[]
			{
				')'
			});
			string[] array = str.Split(new char[]
			{
				','
			});
			int num = Convert.ToInt32(array[0]);
			int num2 = Convert.ToInt32(array[1]);
			int num3 = Convert.ToInt32(array[2]);
			int num4 = Convert.ToInt32(array[3]);
			return new CellRect(num, num2, num3 - num + 1, num4 - num2 + 1);
		}

		public override int GetHashCode()
		{
			int seed = 0;
			seed = Gen.HashCombineInt(seed, this.minX);
			seed = Gen.HashCombineInt(seed, this.maxX);
			seed = Gen.HashCombineInt(seed, this.minZ);
			return Gen.HashCombineInt(seed, this.maxZ);
		}

		public override bool Equals(object obj)
		{
			return obj is CellRect && this.Equals((CellRect)obj);
		}

		public bool Equals(CellRect other)
		{
			return this.minX == other.minX && this.maxX == other.maxX && this.minZ == other.minZ && this.maxZ == other.maxZ;
		}

		public static bool operator ==(CellRect lhs, CellRect rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CellRect lhs, CellRect rhs)
		{
			return !(lhs == rhs);
		}
	}
}
