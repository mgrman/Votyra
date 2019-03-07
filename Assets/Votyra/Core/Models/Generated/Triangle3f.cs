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
            A = a;
            B = b;
            C = c;
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
            var normal = Vector3fUtils.Cross(B - A, C - A);
            return Vector3fUtils.Dot(observer - center, normal);
        }

        public Vector3f? Intersect(Ray3f ray)
        {
            // Vectors from p1 to p2/p3 (edges)
            Vector3f e1, e2;

            Vector3f p, q, t;
            float det, invDet, u, v;

            //Find vectors for two edges sharing vertex/point p1
            e1 = B - A;
            e2 = C - A;

            // calculating determinant 
            p = Vector3fUtils.Cross(ray.Direction, e2);

            //Calculate determinat
            det = Vector3fUtils.Dot(e1, p);

            //if determinant is near zero, ray lies in plane of triangle otherwise not
            if (det > -float.Epsilon && det < float.Epsilon)
                return null;

            invDet = 1.0f / det;

            //calculate distance from p1 to ray origin
            t = ray.Origin - A;

            //Calculate u parameter
            u = Vector3fUtils.Dot(t, p) * invDet;

            //Check for ray hit
            if (u < 0 || u > 1)
                return null;

            //Prepare to test v parameter
            q = Vector3fUtils.Cross(t, e1);

            //Calculate v parameter
            v = Vector3fUtils.Dot(ray.Direction, q) * invDet;

            //Check for ray hit
            if (v < 0 || u + v > 1)
                return null;

            var rayDistance = Vector3fUtils.Dot(e2, q) * invDet;
            if (rayDistance > float.Epsilon)
                return ray.GetPoint(rayDistance);

            // No hit at all
            return null;
        }

        public bool IsCCW(Vector3f observer)
        {
            var dot = DotWithObserver(observer);
            if (dot == 0f)
                throw new InvalidOperationException($"Wrong observer! Observer '{observer}' cannot be used with triangle '{this}'.");
            return dot >= 0;
        }

        public Triangle3f EnsureCCW(Vector3f observer)
        {
            if (IsCCW(observer))
                return this;
            return GetReversedOrder();
        }

        public Triangle3f GetReversedOrder() => new Triangle3f(A, C, B);

        public override bool Equals(object obj)
        {
            if (obj is Triangle3f)
            {
                var that = (Triangle3f) obj;
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

        private class TriangleInvariantComparer : IEqualityComparer<Triangle3f>
        {
            public bool Equals(Triangle3f x, Triangle3f y)
            {
                foreach (var xP in x.Points)
                {
                    if (!y.Points.Any(yP => (xP - yP).SqrMagnitude() < 0.1f))
                        return false;
                }

                return true;
            }

            public int GetHashCode(Triangle3f obj) => 0;
        }
    }

    public static class Triangle3fExtensions
    {
        public static IEnumerable<Triangle3f> ChangeOrderIfTrue(this IEnumerable<Triangle3f> triangles, bool value)
        {
            if (value)
                return triangles.Select(o => o.GetReversedOrder());
            return triangles;
        }
    }
}