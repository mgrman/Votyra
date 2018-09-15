using System;
using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Models
{
    public struct Triangle3f
    {
        public static readonly IEqualityComparer<Triangle3f> OrderInvariantComparer = new TriangleInvariantComparer();
        public readonly Vector3f A;
        public readonly Vector3f B;
        public readonly Vector3f C;

        public Triangle3f(Vector3f a, Vector3f b, Vector3f c)
        {
            this.A = a;
            this.B = b;
            this.C = c;
        }

        public IEnumerable<Vector3f> Points
        {
            get
            {
                yield return A;
                yield return B;
                yield return C;
            }
        }

        public float DotWithObserver(Vector3f observer)
        {
            var center = (A + B + C) / 3f;
            var normal = Vector3f.Cross(B - A, C - A);
            return Vector3f.Dot(observer - center, normal);
        }

        public bool IsCCW(Vector3f observer)
        {
            var dot = DotWithObserver(observer);
            if (dot == 0f)
            {
                throw new InvalidOperationException($"Wrong observer! Observer '{observer}' cannot be used with triangle '{this}'.");
            }
            return dot >= 0;
        }

        public Triangle3f EnsureCCW(Vector3f observer)
        {
            if (IsCCW(observer))
            {
                return this;
            }
            else
            {
                return GetReversedOrder();
            }
        }

        public Triangle3f GetReversedOrder()
        {
            return new Triangle3f(A, C, B);
        }

        public override bool Equals(object obj)
        {
            if (obj is Triangle3f)
            {
                var that = (Triangle3f)obj;
                return this.A == that.A && this.B == that.B && this.C == that.C;
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

        public override string ToString()
        {
            return $"{A},{B},{C}";
        }

        private class TriangleInvariantComparer : IEqualityComparer<Triangle3f>
        {
            public TriangleInvariantComparer()
            {
            }

            public bool Equals(Triangle3f x, Triangle3f y)
            {
                foreach (var xP in x.Points)
                {
                    if (!y.Points.Any(yP => (xP - yP).SqrMagnitude < 0.1f))
                    {
                        return false;
                    }
                }
                return true;
            }

            public int GetHashCode(Triangle3f obj)
            {
                return 0;
            }
        }
    }

    public static class Triangle3Extensions
    {
        public static IEnumerable<Triangle3f> ChangeOrderIfTrue(this IEnumerable<Triangle3f> triangles, bool value)
        {
            if (value)
            {
                return triangles.Select(o => o.GetReversedOrder());
            }
            else
            {
                return triangles;
            }
        }
    }
}