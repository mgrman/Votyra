using System;
using Newtonsoft.Json;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Vector3f : IEquatable<Vector3f>
    {
        [JsonIgnore]
        public static readonly Vector3f Zero = new Vector3f();

        [JsonIgnore]
        public static readonly Vector3f One = FromSame(1);

        public readonly float X;

        public readonly float Y;

        public readonly float Z;

        [JsonConstructor]
        public Vector3f(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        [JsonIgnore]
        public Vector3f Normalized => this / Magnitude;

        [JsonIgnore]
        public float Magnitude => (float) Math.Sqrt(SqrMagnitude);

        [JsonIgnore]
        public float SqrMagnitude => X * X + Y * Y + Z * Z;

        [JsonIgnore]
        public bool Positive => X > 0 && Y > 0 && Z > 0;

        [JsonIgnore]
        public bool ZeroOrPositive => X >= 0 && Y >= 0 && Z >= 0;

        [JsonIgnore]
        public float VolumeSum => X * Y * Z;

        [JsonIgnore]
        public Vector2f XY => new Vector2f(X, Y);

        [JsonIgnore]
        public bool AnyNegative => X < 0 || Y < 0 || Z < 0;

        public static Vector3f FromSame(float value) => new Vector3f(value, value, value);

        public static float Dot(Vector3f lhs, Vector3f rhs) => (float) (lhs.X * (double) rhs.X + lhs.Y * (double) rhs.Y + lhs.Z * (double) rhs.Z);

        public static Vector3f Cross(Vector3f lhs, Vector3f rhs) => new Vector3f((float) (lhs.Y * (double) rhs.Z - lhs.Z * (double) rhs.Y), (float) (lhs.Z * (double) rhs.X - lhs.X * (double) rhs.Z), (float) (lhs.X * (double) rhs.Y - lhs.Y * (double) rhs.X));

        public static bool operator >(Vector3f a, Vector3f b) => a.X > b.X && a.Y > b.Y && a.Z > b.Z;

        public static bool operator >=(Vector3f a, Vector3f b) => a.X >= b.X && a.Y >= b.Y && a.Z >= b.Z;

        public static bool operator <(Vector3f a, Vector3f b) => a.X < b.X && a.Y < b.Y && a.Z < b.Z;

        public static bool operator <=(Vector3f a, Vector3f b) => a.X <= b.X && a.Y <= b.Y && a.Z <= b.Z;

        public static Vector3f operator +(Vector3f a, float b) => new Vector3f(a.X + b, a.Y + b, a.Z + b);

        public static Vector3f operator -(Vector3f a, float b) => new Vector3f(a.X - b, a.Y - b, a.Z - b);

        public static Vector3f operator -(Vector3f a) => new Vector3f(-a.X, -a.Y, -a.Z);

        public static Vector3f operator +(Vector3f a, Vector3f b) => new Vector3f(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static Vector3f operator -(Vector3f a, Vector3f b) => new Vector3f(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static Vector3f operator *(Vector3f a, Vector3f b) => new Vector3f(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

        public static Vector3f operator *(Vector3f a, float b) => new Vector3f(a.X * b, a.Y * b, a.Z * b);

        public static Vector3f operator %(Vector3f a, Vector3f b) => new Vector3f(a.X % b.X, a.Y % b.Y, a.Z % b.Z);

        public static Vector3f operator %(Vector3f a, float b) => new Vector3f(a.X % b, a.Y % b, a.Z % b);

        public static Vector3f operator /(Vector3f a, float b) => new Vector3f(a.X / b, a.Y / b, a.Z / b);

        public static Vector3f operator /(Vector3f a, Vector3f b) => new Vector3f(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

        public static Vector3f Max(Vector3f a, Vector3f b) => new Vector3f(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

        public static Vector3f Min(Vector3f a, Vector3f b) => new Vector3f(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));

        public static bool operator ==(Vector3f a, Vector3f b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z;

        public static bool operator !=(Vector3f a, Vector3f b) => a.X != b.X || a.Y != b.Y || a.Z != b.Z;

        public Vector3i FloorToVector3i() => new Vector3i(X.FloorToInt(), Y.FloorToInt(), Z.FloorToInt());

        public Vector3i RoundToVector3i() => new Vector3i(X.RoundToInt(), Y.RoundToInt(), Z.RoundToInt());

        public Vector3i CeilToVector3i() => new Vector3i(X.CeilToInt(), Y.CeilToInt(), Z.CeilToInt());

        public bool Equals(Vector3f other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Vector3f))
                return false;

            return Equals((Vector3f) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return X.GetHashCode() + Y.GetHashCode() * 7 + Z.GetHashCode() * 13;
            }
        }

        public override string ToString() => string.Format("({0} , {1}, {2})", X, Y, Z);
    }
}