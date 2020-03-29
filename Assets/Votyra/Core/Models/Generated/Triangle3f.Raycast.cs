using System;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public partial struct Triangle3f
    {
        public float DotWithObserver(Vector3f observer)
        {
            var center = (A + B + C) / 3f;
            var normal = Vector3fUtils.Cross(B - A, C - A);
            return Vector3fUtils.Dot(observer - center, normal);
        }

        public float BarycentricCoords(Vector2f p)
        {
            var area = 0.5f * (-B.Y * C.X + A.Y * (-B.X + C.X) + A.X * (B.Y - C.Y) + B.X * C.Y);

            var a = 1f / (2f * area) * (A.Y * C.X - A.X * C.Y + (C.Y - A.Y) * p.X + (A.X - C.X) * p.Y);

            var b = 1f / (2f * area) * (A.X * B.Y - A.Y * B.X + (A.Y - B.Y) * p.X + (B.X - A.X) * p.Y);
            var c = 1f - a - b;

            if (a.IsApproximatelyLessOrEqual(1f) && a.IsApproximatelyGreaterOrEqual(0f) && b.IsApproximatelyLessOrEqual(1f) && b.IsApproximatelyGreaterOrEqual(0f) && c.IsApproximatelyLessOrEqual(1f) && c.IsApproximatelyGreaterOrEqual(0f))
            {
                var res = A.Z * a + B.Z * b + C.Z * c;
                return res;
            }

            return float.NaN;
        }

        public Vector3f Intersect(Ray3f ray)
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
            {
                return Vector3f.NaN;
            }

            invDet = 1.0f / det;

            //calculate distance from p1 to ray origin
            t = ray.Origin - A;

            //Calculate u parameter
            u = Vector3fUtils.Dot(t, p) * invDet;

            //Check for ray hit
            if (u < 0 || u > 1)
            {
                return Vector3f.NaN;
            }

            //Prepare to test v parameter
            q = Vector3fUtils.Cross(t, e1);

            //Calculate v parameter
            v = Vector3fUtils.Dot(ray.Direction, q) * invDet;

            //Check for ray hit
            if (v < 0 || u + v > 1)
            {
                return Vector3f.NaN;
            }

            var rayDistance = Vector3fUtils.Dot(e2, q) * invDet;
            if (rayDistance > float.Epsilon)
            {
                return ray.GetPoint(rayDistance);
            }

            // No hit at all
            return Vector3f.NaN;
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

            return GetReversedOrder();
        }
    }
}
