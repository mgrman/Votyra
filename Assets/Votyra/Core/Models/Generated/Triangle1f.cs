using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Models
{
    public struct Triangle1f
    {
        public static readonly IEqualityComparer<Triangle1f> OrderInvariantComparer = new TriangleInvariantComparer();
        public readonly float A;
        public readonly float B;
        public readonly float C;

        public Triangle1f(float a, float b, float c)
        {
            A = a;
            B = b;
            C = c;
        }

        public IEnumerable<float> Points
        {
            get
            {
                yield return A;
                yield return B;
                yield return C;
            }
        }

        public Triangle1f GetReversedOrder() => new Triangle1f(A, C, B);

        public override bool Equals(object obj)
        {
            if (obj is Triangle1f)
            {
                var that = (Triangle1f) obj;
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

        private class TriangleInvariantComparer : IEqualityComparer<Triangle1f>
        {
            public bool Equals(Triangle1f x, Triangle1f y)
            {
                foreach (var xP in x.Points)
                {
                    if (!y.Points.Any(yP => (xP - yP).SqrMagnitude() < 0.1f))
                        return false;
                }

                return true;
            }

            public int GetHashCode(Triangle1f obj) => 0;
        }
    }

    public static class Triangle1fExtensions
    {
        public static IEnumerable<Triangle1f> ChangeOrderIfTrue(this IEnumerable<Triangle1f> triangles, bool value)
        {
            if (value)
                return triangles.Select(o => o.GetReversedOrder());
            return triangles;
        }
    }
}