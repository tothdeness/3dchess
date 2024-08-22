using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Moves
{
    public class Vector3Comparer : IEqualityComparer<Vector3>
    {
		//private const float Tolerance = 0.001f;

		public bool Equals(Vector3 x, Vector3 y)
		{
			return x.X == y.X && x.Z == y.Z;
		}

		public int GetHashCode(Vector3 obj)
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 23 + obj.X.GetHashCode();
				hash = hash * 23 + obj.Z.GetHashCode();
				return hash;
			}

		}
	}
}
