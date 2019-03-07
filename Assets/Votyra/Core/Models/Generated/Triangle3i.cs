using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Models
{
    public struct Triangle3i
    {
        public static readonly IEqualityComparer<Triangle3i> OrderInvariantComparer = new TriangleInvariantComparer();
        public readonly Vector3i A;
        public readonly Vector3i B;
        public readonly Vector3i C;

        public Triangle3i(Vector3i a, Vector3i b, Vector3i c)
        {
            A = a;
            B = b;
            C = c;
        }

        public IEnumerable<Vector3i> Points
        {
            get
            {
                yield return A;
                yield return B;
                yield return C;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Triangle3i)
            {
                var that = (Triangle3i) obj;
                return A == that.A && B == that.B && C == that.C;
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return A.GetHashCode() + B.GetHashCode() * 3 + C.GetHashCode() * 7;
            }
        }

        public override string ToString() => $"{A},{B},{C}";

        private class TriangleInvariantComparer : IEqualityComparer<Triangle3i>
        {
            public bool Equals(Triangle3i x, Triangle3i y)
            {
                foreach (var point in x.Points)
                {
                    if (!y.Points.Contains(point))
                        return false;
                }

                return true;
            }

            public int GetHashCode(Triangle3i obj) => (obj.A + obj.B + obj.C).GetHashCode();
        }
    }
}