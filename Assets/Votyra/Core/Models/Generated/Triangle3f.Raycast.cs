using System;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public partial struct Triangle3f
    {
        public float DotWithObserver(Vector3f observer)
        {
            var center = (this.A + this.B + this.C) / 3f;
            var normal = Vector3fUtils.Cross(this.B - this.A, this.C - this.A);
            return Vector3fUtils.Dot(observer - center, normal);
        }

        public float BarycentricCoords(Vector2f p)
        {
            var area = 0.5f * ((-this.B.Y * this.C.X) + (this.A.Y * (-this.B.X + this.C.X)) + (this.A.X * (this.B.Y - this.C.Y)) + (this.B.X * this.C.Y));

            var a = (1f / (2f * area)) * (((this.A.Y * this.C.X) - (this.A.X * this.C.Y)) + ((this.C.Y - this.A.Y) * p.X) + ((this.A.X - this.C.X) * p.Y));

            var b = (1f / (2f * area)) * (((this.A.X * this.B.Y) - (this.A.Y * this.B.X)) + ((this.A.Y - this.B.Y) * p.X) + ((this.B.X - this.A.X) * p.Y));
            var c = 1f - a - b;

            if (a.IsApproximatelyLessOrEqual(1f) && a.IsApproximatelyGreaterOrEqual(0f) && b.IsApproximatelyLessOrEqual(1f) && b.IsApproximatelyGreaterOrEqual(0f) && c.IsApproximatelyLessOrEqual(1f) && c.IsApproximatelyGreaterOrEqual(0f))
            {
                var res = (this.A.Z * a) + (this.B.Z * b) + (this.C.Z * c);
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

            // Find vectors for two edges sharing vertex/point p1
            e1 = this.B - this.A;
            e2 = this.C - this.A;

            // calculating determinant
            p = Vector3fUtils.Cross(ray.Direction, e2);

            // Calculate determinat
            det = Vector3fUtils.Dot(e1, p);

            // if determinant is near zero, ray lies in plane of triangle otherwise not
            if ((det > -float.Epsilon) && (det < float.Epsilon))
            {
                return Vector3f.NaN;
            }

            invDet = 1.0f / det;

            // calculate distance from p1 to ray origin
            t = ray.Origin - this.A;

            // Calculate u parameter
            u = Vector3fUtils.Dot(t, p) * invDet;

            // Check for ray hit
            if ((u < 0) || (u > 1))
            {
                return Vector3f.NaN;
            }

            // Prepare to test v parameter
            q = Vector3fUtils.Cross(t, e1);

            // Calculate v parameter
            v = Vector3fUtils.Dot(ray.Direction, q) * invDet;

            // Check for ray hit
            if ((v < 0) || ((u + v) > 1))
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
            var dot = this.DotWithObserver(observer);
            if (dot == 0f)
            {
                throw new InvalidOperationException($"Wrong observer! Observer '{observer}' cannot be used with triangle '{this}'.");
            }

            return dot >= 0;
        }

        public Triangle3f EnsureCCW(Vector3f observer)
        {
            if (this.IsCCW(observer))
            {
                return this;
            }

            return this.GetReversedOrder();
        }
    }
}
