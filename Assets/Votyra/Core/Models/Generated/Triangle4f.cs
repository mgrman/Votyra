using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Models
{
    public struct Triangle4f
    {
        public static readonly IEqualityComparer<Triangle4f> OrderInvariantComparer = new TriangleInvariantComparer();
        public readonly Vector4f A;
        public readonly Vector4f B;
        public readonly Vector4f C;

        public Triangle4f(Vector4f a, Vector4f b, Vector4f c)
        {
            A = a;
            B = b;
            C = c;
        }

        public IEnumerable<Vector4f> Points
        {
            get
            {
                yield return A;
                yield return B;
                yield return C;
            }
        }

        public Triangle4f GetReversedOrder() => new Triangle4f(A, C, B);

        public override bool Equals(object obj)
        {
            if (obj is Triangle4f)
            {
                var that = (Triangle4f) obj;
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

        private class TriangleInvariantComparer : IEqualityComparer<Triangle4f>
        {
            public bool Equals(Triangle4f x, Triangle4f y)
            {
                foreach (var xP in x.Points)
                {
                    if (!y.Points.Any(yP => (xP - yP).SqrMagnitude() < 0.1f))
                        return false;
                }

                return true;
            }

            public int GetHashCode(Triangle4f obj) => 0;
        }
    }

    public static class Triangle4fExtensions
    {
        public static IEnumerable<Triangle4f> ChangeOrderIfTrue(this IEnumerable<Triangle4f> triangles, bool value)
        {
            if (value)
                return triangles.Select(o => o.GetReversedOrder());
            return triangles;
        }
    }
}