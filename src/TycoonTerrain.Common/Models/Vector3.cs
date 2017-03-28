using System;

namespace TycoonTerrain.Common.Models
{
    public struct Vector3 : IEquatable<Vector3>
    {
        public readonly float x;
        public readonly float y;
        public readonly float z;

        public Vector3(float x, float y, float z)
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

        public float VolumeSum { get { return x * y * z; } }

        public Vector2 XY { get { return new Vector2(x,y); } }

        public static readonly Vector3 Zero = new Vector3();
        public static readonly Vector3 One = new Vector3(1, 1, 1);

        public static bool operator >(Vector3 a, Vector3 b)
        {
            return a.x > b.x && a.y > b.y && a.z > b.z;
        }

        public static bool operator >=(Vector3 a, Vector3 b)
        {
            return a.x >= b.x && a.y >= b.y && a.z >= b.z;
        }

        public static bool operator <(Vector3 a, Vector3 b)
        {
            return a.x < b.x && a.y < b.y && a.z < b.z;
        }

        public static bool operator <=(Vector3 a, Vector3 b)
        {
            return a.x <= b.x && a.y <= b.y && a.z <= b.z;
        }

        public static Vector3 operator +(Vector3 a, float b)
        {
            return new Vector3(a.x + b, a.y + b, a.z + b);
        }

        public static Vector3 operator -(Vector3 a, float b)
        {
            return new Vector3(a.x - b, a.y - b, a.z - b);
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3 operator /(Vector3 a, float b)
        {
            return new Vector3(a.x / b, a.y / b, a.z / b);
        }

        public static Vector3 Max(Vector3 a, Vector3 b)
        {
            return new Vector3(Math.Max(a.x, b.x), Math.Max(a.y, b.y), Math.Max(a.z, b.z));
        }

        public static Vector3 Min(Vector3 a, Vector3 b)
        {
            return new Vector3(Math.Min(a.x, b.x), Math.Min(a.y, b.y), Math.Min(a.z, b.z));
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return a.x != b.x || a.y != b.y || a.z != b.z;
        }

        public bool Equals(Vector3 other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector3))
                return false;

            return this.Equals((Vector3)obj);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() + y.GetHashCode() * 7 + z.GetHashCode() * 13;
        }

        public override string ToString()
        {
            return string.Format("({0} , {1}, {1})", x, y, z);
        }
    }
}