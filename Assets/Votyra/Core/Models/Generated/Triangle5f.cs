using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Models
{
    public struct Triangle5f
    {
        public static readonly IEqualityComparer<Triangle5f> OrderInvariantComparer = new TriangleInvariantComparer();
        public readonly Vector5f A;
        public readonly Vector5f B;
        public readonly Vector5f C;

        public Triangle5f(Vector5f a, Vector5f b, Vector5f c)
        {
            A = a;
            B = b;
            C = c;
        }

        public IEnumerable<Vector5f> Points
        {
            get
            {
                yield return A;
                yield return B;
                yield return C;
            }
        }

        public Triangle5f GetReversedOrder() => new Triangle5f(A, C, B);

        public override bool Equals(object obj)
        {
            if (obj is Triangle5f)
            {
                var that = (Triangle5f) obj;
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

        private class TriangleInvariantComparer : IEqualityComparer<Triangle5f>
        {
            public bool Equals(Triangle5f x, Triangle5f y)
            {
                foreach (var xP in x.Points)
                {
                    if (!y.Points.Any(yP => (xP - yP).SqrMagnitude() < 0.1f))
                    {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(Triangle5f obj) => 0;
        }
    }

    public static class Triangle5fExtensions
    {
        public static IEnumerable<Triangle5f> ChangeOrderIfTrue(this IEnumerable<Triangle5f> triangles, bool value)
        {
            if (value)
            {
                return triangles.Select(o => o.GetReversedOrder());
            }

            return triangles;
        }
    }
}
