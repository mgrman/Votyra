using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Models
{
    public struct Triangle2i
    {
        public static readonly IEqualityComparer<Triangle2i> OrderInvariantComparer = new TriangleInvariantComparer();
        public readonly Vector2i A;
        public readonly Vector2i B;
        public readonly Vector2i C;

        public Triangle2i(Vector2i a, Vector2i b, Vector2i c)
        {
            A = a;
            B = b;
            C = c;
        }

        public IEnumerable<Vector2i> Points
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
            if (obj is Triangle2i)
            {
                var that = (Triangle2i) obj;
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

        private class TriangleInvariantComparer : IEqualityComparer<Triangle2i>
        {
            public bool Equals(Triangle2i x, Triangle2i y)
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

            public int GetHashCode(Triangle2i obj) => (obj.A + obj.B + obj.C).GetHashCode();
        }
    }
}
