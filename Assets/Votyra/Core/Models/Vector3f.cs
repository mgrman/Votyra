using System;
using System.Collections.Generic;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Vector3f : IEquatable<Vector3f>
    {
        public readonly float x;
        public readonly float y;
        public readonly float z;

        public Vector3f normalized => this / magnitude;

        public float magnitude => (float)Math.Sqrt(sqrMagnitude);
        public float sqrMagnitude => x * x + y * y + z * z;

        public Vector3f(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public bool Positive
        {
            get
            {
                return this.x > 0 && this.y > 0 && this.z > 0;
            }
        }

        public bool ZeroOrPositive
        {
            get
            {
                return this.x >= 0 && this.y >= 0 && this.z >= 0;
            }
        }

        public Vector3i ToVector3i()
        {
            return RoundToVector3i();
        }

        public Vector3i FloorToVector3i()
        {
            return new Vector3i(x.FloorToInt(), y.FloorToInt(), z.FloorToInt());
        }

        public Vector3i RoundToVector3i()
        {
            return new Vector3i(x.RoundToInt(), y.RoundToInt(), z.RoundToInt());
        }

        public Vector3i CeilToVector3i()
        {
            return new Vector3i(x.CeilToInt(), y.CeilToInt(), z.CeilToInt());
        }

        public float VolumeSum { get { return x * y * z; } }

        public Vector2f XY { get { return new Vector2f(x, y); } }

        public static float Dot(Vector3f lhs, Vector3f rhs)
        {
            return (float)((double)lhs.x * (double)rhs.x + (double)lhs.y * (double)rhs.y + (double)lhs.z * (double)rhs.z);
        }

        public static Vector3f Cross(Vector3f lhs, Vector3f rhs)
        {
            return new Vector3f((float)((double)lhs.y * (double)rhs.z - (double)lhs.z * (double)rhs.y), (float)((double)lhs.z * (double)rhs.x - (double)lhs.x * (double)rhs.z), (float)((double)lhs.x * (double)rhs.y - (double)lhs.y * (double)rhs.x));
        }

        public static readonly Vector3f Zero = new Vector3f();

        public static readonly Vector3f One = new Vector3f(1, 1, 1);

        public static bool operator >(Vector3f a, Vector3f b)
        {
            return a.x > b.x && a.y > b.y && a.z > b.z;
        }

        public static bool operator >=(Vector3f a, Vector3f b)
        {
            return a.x >= b.x && a.y >= b.y && a.z >= b.z;
        }

        public static bool operator <(Vector3f a, Vector3f b)
        {
            return a.x < b.x && a.y < b.y && a.z < b.z;
        }

        public static bool operator <=(Vector3f a, Vector3f b)
        {
            return a.x <= b.x && a.y <= b.y && a.z <= b.z;
        }

        public static Vector3f operator +(Vector3f a, float b)
        {
            return new Vector3f(a.x + b, a.y + b, a.z + b);
        }

        public static Vector3f operator -(Vector3f a, float b)
        {
            return new Vector3f(a.x - b, a.y - b, a.z - b);
        }

        public static Vector3f operator -(Vector3f a)
        {
            return new Vector3f(-a.x, -a.y, -a.z);
        }

        public static Vector3f operator +(Vector3f a, Vector3f b)
        {
            return new Vector3f(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3f operator -(Vector3f a, Vector3f b)
        {
            return new Vector3f(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3f operator *(Vector3f a, Vector3f b)
        {
            return new Vector3f(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        public static Vector3f operator *(Vector3f a, float b)
        {
            return new Vector3f(a.x * b, a.y * b, a.z * b);
        }

        public static Vector3f operator /(Vector3f a, float b)
        {
            return new Vector3f(a.x / b, a.y / b, a.z / b);
        }

        public static Vector3f Max(Vector3f a, Vector3f b)
        {
            return new Vector3f(Math.Max(a.x, b.x), Math.Max(a.y, b.y), Math.Max(a.z, b.z));
        }

        public static Vector3f Min(Vector3f a, Vector3f b)
        {
            return new Vector3f(Math.Min(a.x, b.x), Math.Min(a.y, b.y), Math.Min(a.z, b.z));
        }

        public static bool operator ==(Vector3f a, Vector3f b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(Vector3f a, Vector3f b)
        {
            return a.x != b.x || a.y != b.y || a.z != b.z;
        }
        public static Vector3f Half { get { return new Vector3f(0.5f, 0.5f, 0.5f); } }


        public Vector3f SetX(float x)
        {
            return new Vector3f(x, this.y, this.z);
        }

        public Vector3f SetY(float y)
        {
            return new Vector3f(this.x, y, this.z);
        }

        public Vector3f SetZ(float z)
        {
            return new Vector3f(this.x, this.y, z);
        }

        public Vector3f ClipX(float minX, float maxX)
        {
            return this.SetX(this.x.Clip(minX, maxX));
        }

        public Vector3f ClipY(float minY, float maxY)
        {
            return this.SetY(this.y.Clip(minY, maxY));
        }

        public Vector3f ClipZ(float minZ, float maxZ)
        {
            return this.SetZ(this.z.Clip(minZ, maxZ));
        }

        public Vector3f Normalize(Rect3f bounds)
        {
            float x = this.x.Normalize(bounds.min.x, bounds.max.x);
            float y = this.y.Normalize(bounds.min.y, bounds.max.y);
            float z = this.z.Normalize(bounds.min.z, bounds.max.z);
            return new Vector3f(x, y, z);
        }

        public Vector3f Denormalize(Rect3f bounds)
        {
            float x = this.x.Denormalize(bounds.min.x, bounds.max.x);
            float y = this.y.Denormalize(bounds.min.y, bounds.max.y);
            float z = this.z.Denormalize(bounds.min.z, bounds.max.z);
            return new Vector3f(x, y, z);
        }

        public bool Equals(Vector3f other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector3f))
                return false;

            return this.Equals((Vector3f)obj);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() + y.GetHashCode() * 7 + z.GetHashCode() * 13;
        }

        public override string ToString()
        {
            return string.Format("({0} , {1}, {2})", x, y, z);
        }
    }

    public static class Vector3fExtensions
    {

        public static Vector3f Average(this IEnumerable<Vector3f> items)
        {
            Vector3f sum = new Vector3f();
            int count = 0;
            foreach (var item in items)
            {
                sum += item;
                count++;
            }
            return sum / count;
        }
    }
}