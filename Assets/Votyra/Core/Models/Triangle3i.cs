using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Models
{
    public struct Triangle3i
    {
        public readonly Vector3i a;
        public readonly Vector3i b;
        public readonly Vector3i c;

        public Triangle3i(Vector3i a, Vector3i b, Vector3i c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        public Triangle3i(Vector3f a, Vector3f b, Vector3f c)
        {
            this.a = a.ToVector3i();
            this.b = b.ToVector3i();
            this.c = c.ToVector3i();
        }

        public IEnumerable<Vector3i> Points
        {
            get
            {
                yield return a;
                yield return b;
                yield return c;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Triangle3i)
            {
                var that = (Triangle3i)obj;
                return this.a == that.a && this.b == that.b && this.c == that.c;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return a.GetHashCode() + b.GetHashCode() * 3 + c.GetHashCode() * 7;
        }

        public override string ToString()
        {
            return $"{a},{b},{c}";
        }

        public static readonly IEqualityComparer<Triangle3i> OrderInvariantComparer = new TriangleInvariantComparer();

        private class TriangleInvariantComparer : IEqualityComparer<Triangle3i>
        {
            public TriangleInvariantComparer()
            {
            }

            public bool Equals(Triangle3i x, Triangle3i y)
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

            public int GetHashCode(Triangle3i obj)
            {
                return (obj.a + obj.b + obj.c).GetHashCode();
            }
        }
    }
}