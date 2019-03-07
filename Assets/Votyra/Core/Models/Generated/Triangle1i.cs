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
            A = a;
            B = b;
            C = c;
        }

        public IEnumerable<int> Points
        {
            get
            {
                yield return A;
                yield return B;
                yield return C;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Triangle1i)
            {
                var that = (Triangle1i) obj;
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

        private class TriangleInvariantComparer : IEqualityComparer<Triangle1i>
        {
            public bool Equals(Triangle1i x, Triangle1i y)
            {
                foreach (var point in x.Points)
                {
                    if (!y.Points.Contains(point))
                        return false;
                }

                return true;
            }

            public int GetHashCode(Triangle1i obj) => (obj.A + obj.B + obj.C).GetHashCode();
        }
    }
}