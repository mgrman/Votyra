using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Models
{
    public struct Triangle2f
    {
        public static readonly IEqualityComparer<Triangle2f> OrderInvariantComparer = new TriangleInvariantComparer();
        public readonly Vector2f A;
        public readonly Vector2f B;
        public readonly Vector2f C;

        public Triangle2f(Vector2f a, Vector2f b, Vector2f c)
        {
            A = a;
            B = b;
            C = c;
        }

        public IEnumerable<Vector2f> Points
        {
            get
            {
                yield return A;
                yield return B;
                yield return C;
            }
        }

        public Triangle2f GetReversedOrder() => new Triangle2f(A, C, B);

        public override bool Equals(object obj)
        {
            if (obj is Triangle2f)
            {
                var that = (Triangle2f) obj;
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

        private class TriangleInvariantComparer : IEqualityComparer<Triangle2f>
        {
            public bool Equals(Triangle2f x, Triangle2f y)
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

            public int GetHashCode(Triangle2f obj) => 0;
        }
    }

    public static class Triangle2fExtensions
    {
        public static IEnumerable<Triangle2f> ChangeOrderIfTrue(this IEnumerable<Triangle2f> triangles, bool value)
        {
            if (value)
            {
                return triangles.Select(o => o.GetReversedOrder());
            }

            return triangles;
        }
    }
}
