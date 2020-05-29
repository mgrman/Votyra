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
            this.A = a;
            this.B = b;
            this.C = c;
        }

        public IEnumerable<Vector6f> Points
        {
            get
            {
                yield return this.A;
                yield return this.B;
                yield return this.C;
            }
        }

        public Triangle6f GetReversedOrder() => new Triangle6f(this.A, this.C, this.B);

        public override bool Equals(object obj)
        {
            if (obj is Triangle6f)
            {
                var that = (Triangle6f)obj;
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

        private class TriangleInvariantComparer : IEqualityComparer<Triangle6f>
        {
            public bool Equals(Triangle6f x, Triangle6f y)
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

            public int GetHashCode(Triangle6f obj) => 0;
        }
    }

    public static class Triangle6fExtensions
    {
        public static IEnumerable<Triangle6f> ChangeOrderIfTrue(this IEnumerable<Triangle6f> triangles, bool value)
        {
            if (value)
            {
                return triangles.Select(o => o.GetReversedOrder());
            }

            return triangles;
        }
    }
}
