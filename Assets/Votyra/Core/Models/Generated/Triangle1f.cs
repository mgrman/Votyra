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
            this.A = a;
            this.B = b;
            this.C = c;
        }

        public IEnumerable<float> Points
        {
            get
            {
                yield return this.A;
                yield return this.B;
                yield return this.C;
            }
        }

        public Triangle1f GetReversedOrder() => new Triangle1f(this.A, this.C, this.B);

        public override bool Equals(object obj)
        {
            if (obj is Triangle1f)
            {
                var that = (Triangle1f)obj;
                return (this.A == that.A) && (this.B == that.B) && (this.C == that.C);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.A.GetHashCode() + (this.B.GetHashCode() * 3) + (this.C.GetHashCode() * 7);
            }
        }

        public override string ToString() => $"{this.A},{this.B},{this.C}";

        private class TriangleInvariantComparer : IEqualityComparer<Triangle1f>
        {
            public bool Equals(Triangle1f x, Triangle1f y)
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

            public int GetHashCode(Triangle1f obj) => 0;
        }
    }

    public static class Triangle1fExtensions
    {
        public static IEnumerable<Triangle1f> ChangeOrderIfTrue(this IEnumerable<Triangle1f> triangles, bool value)
        {
            if (value)
            {
                return triangles.Select(o => o.GetReversedOrder());
            }

            return triangles;
        }
    }
}
