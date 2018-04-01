using System;
using System.Globalization;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Vector3i : IEquatable<Vector3i>
    {
        public readonly int x;
        public readonly int y;
        public readonly int z;

        public Vector3i(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public bool AllPositive
        {
            get
            {
                return this.x > 0 && this.y > 0 && this.z > 0;
            }
        }

        public bool AllZeroOrPositive
        {
            get
            {
                return this.x >= 0 && this.y >= 0 && this.z >= 0;
            }
        }

        public bool AnyNegative
        {
            get
            {
                return this.x < 0 || this.y < 0 || this.z < 0;
            }
        }

        public bool IsAsIndexContained(Vector3i size)
        {
            return x >= 0 && y >= 0 && z >= 0 && x < size.x && y < size.y && z < size.z;
        }

        public int Volume { get { return x * y * z; } }

        public static readonly Vector3i Zero = new Vector3i();
        public static readonly Vector3i One = new Vector3i(1, 1, 1);

        public static Vector3i operator +(Vector3i a, int b)
        {
            return new Vector3i(a.x + b, a.y + b, a.z + b);
        }

        public static Vector3i operator -(Vector3i a, int b)
        {
            return new Vector3i(a.x - b, a.y - b, a.z - b);
        }

        public static Vector3i operator +(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3f operator +(Vector3i a, Vector3f b)
        {
            return new Vector3f(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3i operator -(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3f operator *(Vector3f a, Vector3i b)
        {
            return new Vector3f(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3f operator *(Vector3i a, Vector3f b)
        {
            return new Vector3f(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3i operator *(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3i operator /(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        public static Vector3f operator /(Vector3i a, Vector3f b)
        {
            return new Vector3f(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        public static Vector3i operator /(Vector3i a, int b)
        {
            return new Vector3i(a.x / b, a.y / b, a.z / b);
        }

        public static Vector3f operator /(Vector3i a, float b)
        {
            return new Vector3f(a.x / b, a.y / b, a.z / b);
        }

        public Vector3i DivideUp(Vector3i a, int b)
        {
            return new Vector3i(a.x.DivideUp(b), a.y.DivideUp(b), a.z.DivideUp(b));
        }

        public static Vector3i FromSame(int value)
        {
            return new Vector3i(value, value, value);
        }

        public Vector3f ToVector3f()
        {
            return new Vector3f(x, y, z);
        }

        public Vector2i XY
        {
            get
            {
                return new Vector2i(x, y);
            }
        }

        public static Vector3i Cross(Vector3i lhs, Vector3i rhs)
        {
            return new Vector3i(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
        }

        public float magnitude
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y + z * z);
            }
        }

        public float magnitudeManhatan
        {
            get
            {
                return Math.Abs(x) + Math.Abs(y) + Math.Abs(z);
            }
        }
        public float magnitudeManhatanRing
        {
            get
            {
                return Math.Max(Math.Max(Math.Abs(x), Math.Abs(y)), Math.Abs(z));
            }
        }

        public Vector3f normalized
        {
            get
            {
                var mag = magnitude;
                return new Vector3f(x / mag, y / mag, z / mag);
            }
        }

        public Rect3i ToRect3i()
        {
            return new Rect3i(Vector3i.Zero, this);
        }

        public static bool operator <(Vector3i a, Vector3i b)
        {
            return a.x < b.x && a.y < b.y && a.z < b.z;
        }

        public static bool operator <=(Vector3i a, Vector3i b)
        {
            return a.x < b.x && a.y < b.y && a.z < b.z;
        }

        public static bool operator >(Vector3i a, Vector3i b)
        {
            return a.x > b.x && a.y > b.y && a.z > b.z;
        }

        public static bool operator >=(Vector3i a, Vector3i b)
        {
            return a.x >= b.x && a.y >= b.y && a.z >= b.z;
        }

        public static bool operator ==(Vector3i a, Vector3i b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(Vector3i a, Vector3i b)
        {
            return a.x != b.x || a.y != b.y || a.z != b.z;
        }

        public bool Equals(Vector3i other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector3i))
                return false;

            return this.Equals((Vector3i)obj);
        }

        public override int GetHashCode()
        {
            return x + y * 7 + z * 13;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}, {1}, {2})", x, y, z);
        }
    }
}