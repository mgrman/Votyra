using System;
using System.Globalization;
using Newtonsoft.Json;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Vector3i : IEquatable<Vector3i>
    {
        [JsonIgnore]
        public static readonly Vector3i Zero = new Vector3i();

        [JsonIgnore]
        public static readonly Vector3i One = new Vector3i(1, 1, 1);

        public readonly int X;

        public readonly int Y;

        public readonly int Z;

        [JsonConstructor]
        public Vector3i(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        [JsonIgnore]
        public Vector2i XY => new Vector2i(X, Y);

        [JsonIgnore]
        public bool AllPositive => X > 0 && Y > 0 && Z > 0;

        [JsonIgnore]
        public bool AllZeroOrPositive => X >= 0 && Y >= 0 && Z >= 0;

        [JsonIgnore]
        public bool AnyNegative => X < 0 || Y < 0 || Z < 0;

        [JsonIgnore]
        public bool AnyZero => X == 0 || Y == 0 || Z == 0;

        [JsonIgnore]
        public bool AnyZeroOrNegative => X <= 0 || Y <= 0 || Z <= 0;

        [JsonIgnore]
        public int Volume => X * Y * Z;

        public static Vector3i operator +(Vector3i a, int b) => new Vector3i(a.X + b, a.Y + b, a.Z + b);

        public static Vector3i operator -(Vector3i a, int b) => new Vector3i(a.X - b, a.Y - b, a.Z - b);

        public static Vector3i operator +(Vector3i a, Vector3i b) => new Vector3i(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static Vector3f operator +(Vector3i a, Vector3f b) => new Vector3f(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static Vector3i operator -(Vector3i a, Vector3i b) => new Vector3i(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static Vector3f operator *(Vector3f a, Vector3i b) => new Vector3f(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

        public static Vector3f operator *(Vector3i a, Vector3f b) => new Vector3f(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

        public static Vector3i operator *(Vector3i a, Vector3i b) => new Vector3i(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

        public static Vector3i operator %(Vector3i a, Vector3i b) => new Vector3i(a.X % b.X, a.Y % b.Y, a.Z % b.Z);

        public static Vector3i operator %(Vector3i a, int b) => new Vector3i(a.X % b, a.Y % b, a.Z % b);

        public static Vector3i operator /(Vector3i a, Vector3i b) => new Vector3i(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

        public static Vector3f operator /(Vector3i a, Vector3f b) => new Vector3f(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

        public static Vector3i operator /(Vector3i a, int b) => new Vector3i(a.X / b, a.Y / b, a.Z / b);

        public static Vector3f operator /(Vector3i a, float b) => new Vector3f(a.X / b, a.Y / b, a.Z / b);

        public static Vector3i Max(Vector3i a, Vector3i b) => new Vector3i(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

        public static Vector3i Min(Vector3i a, Vector3i b) => new Vector3i(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));

        public static Vector3i FromSame(int value) => new Vector3i(value, value, value);

        public static Vector3i Cross(Vector3i lhs, Vector3i rhs) => new Vector3i(lhs.Y * rhs.Z - lhs.Z * rhs.Y, lhs.Z * rhs.X - lhs.X * rhs.Z, lhs.X * rhs.Y - lhs.Y * rhs.X);

        public static bool operator <(Vector3i a, Vector3i b) => a.X < b.X && a.Y < b.Y && a.Z < b.Z;

        public static bool operator <=(Vector3i a, Vector3i b) => a.X <= b.X && a.Y <= b.Y && a.Z <= b.Z;

        public static bool operator >(Vector3i a, Vector3i b) => a.X > b.X && a.Y > b.Y && a.Z > b.Z;

        public static bool operator >=(Vector3i a, Vector3i b) => a.X >= b.X && a.Y >= b.Y && a.Z >= b.Z;

        public static bool operator ==(Vector3i a, Vector3i b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z;

        public static bool operator !=(Vector3i a, Vector3i b) => a.X != b.X || a.Y != b.Y || a.Z != b.Z;

        public Vector3i DivideUp(Vector3i a, int b) => new Vector3i(a.X.DivideUp(b), a.Y.DivideUp(b), a.Z.DivideUp(b));

        public Vector3f ToVector3f() => new Vector3f(X, Y, Z);

        public Range3i ToRange3i() => Range3i.FromMinAndSize(Zero, this);

        public bool Equals(Vector3i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Vector3i))
                return false;

            return Equals((Vector3i) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return X + Y * 7 + Z * 13;
            }
        }

        public override string ToString() => string.Format(CultureInfo.InvariantCulture, "({0}, {1}, {2})", X, Y, Z);
    }
}