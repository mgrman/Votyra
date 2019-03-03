using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Models
{
    
    
    
    
    
    
        
        
    
    
    public partial struct Triangle5i
    {
        public static readonly IEqualityComparer<Triangle5i> OrderInvariantComparer = new TriangleInvariantComparer();
        public readonly Vector5i A;
        public readonly Vector5i B;
        public readonly Vector5i C;

        public Triangle5i(Vector5i a, Vector5i b, Vector5i c)
        {
            A = a;
            B = b;
            C = c;
        }

        public IEnumerable<Vector5i> Points
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
            if (obj is Triangle5i)
            {
                var that = (Triangle5i) obj;
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

        private class TriangleInvariantComparer : IEqualityComparer<Triangle5i>
        {
            public bool Equals(Triangle5i x, Triangle5i y)
            {
                foreach (var point in x.Points)
                {
                    if (!y.Points.Contains(point))
                        return false;
                }

                return true;
            }

            public int GetHashCode(Triangle5i obj) => (obj.A + obj.B + obj.C).GetHashCode();
        }
    }
}