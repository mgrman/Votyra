using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Models
{
    public partial struct Triangle3f
    {
        public static readonly IEqualityComparer<Triangle3f> OrderInvariantComparer = new TriangleInvariantComparer();
        public readonly Vector3f A;
        public readonly Vector3f B;
        public readonly Vector3f C;

        public Triangle3f(Vector3f a, Vector3f b, Vector3f c)
        {
            A = a;
            B = b;
            C = c;
        }

        public IEnumerable<Vector3f> Points
        {
            get
            {
                yield return A;
                yield return B;
                yield return C;
            }
        }

        public Triangle3f GetReversedOrder() => new Triangle3f(A, C, B);

        public override bool Equals(object obj)
        {
            if (obj is Triangle3f)
            {
                var that = (Triangle3f) obj;
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

        private class TriangleInvariantComparer : IEqualityComparer<Triangle3f>
        {
            public bool Equals(Triangle3f x, Triangle3f y)
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

            public int GetHashCode(Triangle3f obj) => 0;
        }
    }

    public static class Triangle3fExtensions
    {
        public static IEnumerable<Triangle3f> ChangeOrderIfTrue(this IEnumerable<Triangle3f> triangles, bool value)
        {
            if (value)
            {
                return triangles.Select(o => o.GetReversedOrder());
            }

            return triangles;
        }
    }
}
