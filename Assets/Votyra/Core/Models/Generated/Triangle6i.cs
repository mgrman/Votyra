using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Models
{
    public struct Triangle6i
    {
        public static readonly IEqualityComparer<Triangle6i> OrderInvariantComparer = new TriangleInvariantComparer();
        public readonly Vector6i A;
        public readonly Vector6i B;
        public readonly Vector6i C;

        public Triangle6i(Vector6i a, Vector6i b, Vector6i c)
        {
            this.A = a;
            this.B = b;
            this.C = c;
        }

        public IEnumerable<Vector6i> Points
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
            if (obj is Triangle6i)
            {
                var that = (Triangle6i)obj;
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

        private class TriangleInvariantComparer : IEqualityComparer<Triangle6i>
        {
            public bool Equals(Triangle6i x, Triangle6i y)
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

            public int GetHashCode(Triangle6i obj) => (obj.A + obj.B + obj.C).GetHashCode();
        }
    }
}
