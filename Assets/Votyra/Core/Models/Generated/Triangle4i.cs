using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Models
{
    public struct Triangle4i
    {
        public static readonly IEqualityComparer<Triangle4i> OrderInvariantComparer = new TriangleInvariantComparer();
        public readonly Vector4i A;
        public readonly Vector4i B;
        public readonly Vector4i C;

        public Triangle4i(Vector4i a, Vector4i b, Vector4i c)
        {
            A = a;
            B = b;
            C = c;
        }

        public IEnumerable<Vector4i> Points
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
            if (obj is Triangle4i)
            {
                var that = (Triangle4i) obj;
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

        private class TriangleInvariantComparer : IEqualityComparer<Triangle4i>
        {
            public bool Equals(Triangle4i x, Triangle4i y)
            {
                foreach (var point in x.Points)
                {
                    if (!y.Points.Contains(point))
                    {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(Triangle4i obj) => (obj.A + obj.B + obj.C).GetHashCode();
        }
    }
}
