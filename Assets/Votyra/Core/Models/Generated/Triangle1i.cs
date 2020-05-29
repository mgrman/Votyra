using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Models
{
    public struct Triangle1i
    {
        public static readonly IEqualityComparer<Triangle1i> OrderInvariantComparer = new TriangleInvariantComparer();
        public readonly int A;
        public readonly int B;
        public readonly int C;

        public Triangle1i(int a, int b, int c)
        {
            this.A = a;
            this.B = b;
            this.C = c;
        }

        public IEnumerable<int> Points
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
            if (obj is Triangle1i)
            {
                var that = (Triangle1i)obj;
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

        private class TriangleInvariantComparer : IEqualityComparer<Triangle1i>
        {
            public bool Equals(Triangle1i x, Triangle1i y)
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

            public int GetHashCode(Triangle1i obj) => (obj.A + obj.B + obj.C).GetHashCode();
        }
    }
}
