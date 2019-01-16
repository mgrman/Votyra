using System;
using Newtonsoft.Json;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Vector2f : IEquatable<Vector2f>
    {
        public static readonly Vector2f One = new Vector2f(1, 1);
        public static readonly Vector2f Zero = new Vector2f();
        public readonly float X;

        public readonly float Y;

        [JsonConstructor]
        public Vector2f(float x, float y)
        {
            X = x;
            Y = y;
        }

        public bool AnyNegative => X < 0 || Y < 0;

        public float AreaSum => X * Y;

        [JsonIgnore]
        public float Magnitude => (float) Math.Sqrt(SqrMagnitude);

        [JsonIgnore]
        public Vector2f Normalized => Magnitude <= float.Epsilon ? this : this / Magnitude;

        [JsonIgnore]
        public float SqrMagnitude => X * X + Y * Y;
        
        public  Vector2f Perpendicular=> new Vector2f(Y,-X);

        public static Vector2f FromSame(float value) => new Vector2f(value, value);

        public static float Dot(Vector2f lhs, Vector2f rhs) => (float) (lhs.X * (double) rhs.X + lhs.Y * (double) rhs.Y);

        public static Vector2f Max(Vector2f a, Vector2f b) => new Vector2f(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

        public static Vector2f Min(Vector2f a, Vector2f b) => new Vector2f(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));

        public static Vector2f operator -(Vector2f a) => new Vector2f(-a.X, -a.Y);

        public static Vector2f operator -(Vector2f a, int b) => new Vector2f(a.X - b, a.Y - b);

        public static Vector2f operator -(Vector2f a, Vector2f b) => new Vector2f(a.X - b.X, a.Y - b.Y);

        public static bool operator !=(Vector2f a, Vector2f b) => a.X != b.X || a.Y != b.Y;

        public static Vector2f operator *(Vector2f a, Vector2f b) => new Vector2f(a.X * b.X, a.Y * b.Y);

        public static Vector2f operator +(Vector2f a, Vector2i b) => new Vector2f(a.X + b.X, a.Y + b.Y);

        public static Vector2f operator *(Vector2f a, float b) => new Vector2f(a.X * b, a.Y * b);

        public static Vector2f operator %(Vector2f a, Vector2f b) => new Vector2f(a.X % b.X, a.Y % b.Y);

        public static Vector2f operator %(Vector2f a, int b) => new Vector2f(a.X % b, a.Y % b);


        public static Vector2f operator /(Vector2f a, Vector2f b) => new Vector2f(a.X / b.X, a.Y / b.Y);

        public static Vector2f operator /(Vector2f a, float b) => new Vector2f(a.X / b, a.Y / b);

        public static Vector2f operator +(Vector2f a, float b) => new Vector2f(a.X + b, a.Y + b);

        public static Vector2f operator +(Vector2f a, Vector2f b) => new Vector2f(a.X + b.X, a.Y + b.Y);

        public static bool operator <(Vector2f a, Vector2f b) => a.X < b.X && a.Y < b.Y;

        public static bool operator <=(Vector2f a, Vector2f b) => a.X < b.X && a.Y < b.Y;

        public static bool operator ==(Vector2f a, Vector2f b) => a.X == b.X && a.Y == b.Y;

        public static bool operator >(Vector2f a, Vector2f b) => a.X > b.X && a.Y > b.Y;

        public static bool operator >=(Vector2f a, Vector2f b) => a.X >= b.X && a.Y >= b.Y;

        public Vector2i CeilToVector2i() => new Vector2i(X.CeilToInt(), Y.CeilToInt());

        public bool Equals(Vector2f other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Vector2f))
                return false;

            return Equals((Vector2f) obj);
        }

        public Vector2f Floor() => new Vector2f(X.FloorToInt(), Y.FloorToInt());

        public Vector2i FloorToVector2i() => new Vector2i(X.FloorToInt(), Y.FloorToInt());

        public override int GetHashCode()
        {
            unchecked
            {
                return X.GetHashCode() + Y.GetHashCode() * 7;
            }
        }

        public Vector2i RoundToVector2i() => new Vector2i(X.RoundToInt(), Y.RoundToInt());

        public Area2f ToRange2f() => Area2f.FromMinAndSize(Zero, this);

        public override string ToString() => string.Format("({0} , {1})", X, Y);

        public Vector3f ToVector3f(float z) => new Vector3f(X, Y, z);
    }
}