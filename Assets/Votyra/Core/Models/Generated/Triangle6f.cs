using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Models
{
    public struct Triangle6f
    {
        public static readonly IEqualityComparer<Triangle6f> OrderInvariantComparer = new TriangleInvariantComparer();
        public readonly Vector6f A;
        public readonly Vector6f B;
        public readonly Vector6f C;

        public Triangle6f(Vector6f a, Vector6f b, Vector6f c)
        {
            A = a;
            B = b;
            C = c;
        }

        public IEnumerable<Vector6f> Points
        {
            get
            {
                yield return A;
                yield return B;
                yield return C;
            }
        }

        public Triangle6f GetReversedOrder() => new Triangle6f(A, C, B);

        public override bool Equals(object obj)
        {
            if (obj is Triangle6f)
            {
                var that = (Triangle6f) obj;
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

        private class TriangleInvariantComparer : IEqualityComparer<Triangle6f>
        {
            public bool Equals(Triangle6f x, Triangle6f y)
            {
                foreach (var xP in x.Points)
                {
                    if (!y.Points.Any(yP => (xP - yP).SqrMagnitude() < 0.1f))
                        return false;
                }

                return true;
            }

            public int GetHashCode(Triangle6f obj) => 0;
        }
    }

    public static class Triangle6fExtensions
    {
        public static IEnumerable<Triangle6f> ChangeOrderIfTrue(this IEnumerable<Triangle6f> triangles, bool value)
        {
            if (value)
                return triangles.Select(o => o.GetReversedOrder());
            return triangles;
        }
    }
}