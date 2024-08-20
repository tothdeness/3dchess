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
        public bool Equals(Vector3 x, Vector3 y)
        {
            return x.X == y.X && x.Z == y.Z;
        }

        public int GetHashCode(Vector3 obj)
        {
            return obj.X.GetHashCode() * obj.Z.GetHashCode();
        }
    }
}
