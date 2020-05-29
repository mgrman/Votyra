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
            this.A = a;
            this.B = b;
            this.C = c;
        }

        public IEnumerable<Vector4f> Points
        {
            get
            {
                yield return this.A;
                yield return this.B;
                yield return this.C;
            }
        }

        public Triangle4f GetReversedOrder() => new Triangle4f(this.A, this.C, this.B);

        public override bool Equals(object obj)
        {
            if (obj is Triangle4f)
            {
                var that = (Triangle4f)obj;
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

        private class TriangleInvariantComparer : IEqualityComparer<Triangle4f>
        {
            public bool Equals(Triangle4f x, Triangle4f y)
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

            public int GetHashCode(Triangle4f obj) => 0;
        }
    }

    public static class Triangle4fExtensions
    {
        public static IEnumerable<Triangle4f> ChangeOrderIfTrue(this IEnumerable<Triangle4f> triangles, bool value)
        {
            if (value)
            {
                return triangles.Select(o => o.GetReversedOrder());
            }

            return triangles;
        }
    }
}
