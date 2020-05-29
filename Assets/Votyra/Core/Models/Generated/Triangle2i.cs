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
            this.A = a;
            this.B = b;
            this.C = c;
        }

        public IEnumerable<Vector2i> Points
        {
            get
            {
                yield return this.A;
                yield return this.B;
                yield return this.C;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Triangle2i)
            {
                var that = (Triangle2i)obj;
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
