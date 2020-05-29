using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Models
{
    public struct Triangle4i
    {
        public static readonly IEqualityComparer<Triangle4i> OrderInvariantComparer = new TriangleInvariantComparer();
        public readonly Vector4i A;
        public readonly Vector4i B;
        public readonly Vector4i C;

        public Triangle4i(Vector4i a, Vector4i b, Vector4i c)
        {
            this.A = a;
            this.B = b;
            this.C = c;
        }

        public IEnumerable<Vector4i> Points
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
            if (obj is Triangle4i)
            {
                var that = (Triangle4i)obj;
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

        private class TriangleInvariantComparer : IEqualityComparer<Triangle4i>
        {
            public bool Equals(Triangle4i x, Triangle4i y)
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

            public int GetHashCode(Triangle4i obj) => (obj.A + obj.B + obj.C).GetHashCode();
        }
    }
}
