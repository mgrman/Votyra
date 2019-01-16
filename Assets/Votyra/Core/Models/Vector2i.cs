using System;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Vector2i : IEquatable<Vector2i>
    {
        public static readonly Vector2i Zero = new Vector2i();
        public static readonly Vector2i One = new Vector2i(1, 1);
        public readonly int X;
        public readonly int Y;

        public Vector2i(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool AnyNegative => X < 0 || Y < 0;
        public bool AnyZero => X == 0 || Y == 0;
        public bool AnyZeroOrNegative => X <= 0 || Y <= 0;

        public int AreaSum => X * Y;

        public static Vector2i FromSame(int value) => new Vector2i(value, value);

        public static Vector2i operator +(Vector2i a, int b) => new Vector2i(a.X + b, a.Y + b);

        public static Vector2i operator -(Vector2i a, int b) => new Vector2i(a.X - b, a.Y - b);

        public static Vector2i operator +(Vector2i a, Vector2i b) => new Vector2i(a.X + b.X, a.Y + b.Y);

        public static Vector2i operator -(Vector2i a, Vector2i b) => new Vector2i(a.X - b.X, a.Y - b.Y);

        public static Vector2f operator *(Vector2f a, Vector2i b) => new Vector2f(a.X * b.X, a.Y * b.Y);

        public static Vector2f operator *(Vector2i a, Vector2f b) => new Vector2f(a.X * b.X, a.Y * b.Y);

        public static Vector2i operator *(Vector2i a, Vector2i b) => new Vector2i(a.X * b.X, a.Y * b.Y);

        public static Vector2i operator /(Vector2i a, Vector2i b) => new Vector2i(a.X / b.X, a.Y / b.Y);

        public static Vector2f operator /(Vector2i a, Vector2f b) => new Vector2f(a.X / b.X, a.Y / b.Y);

        public static Vector2i operator /(Vector2i a, int b) => new Vector2i(a.X / b, a.Y / b);

        public static Vector2f operator /(Vector2i a, float b) => new Vector2f(a.X / b, a.Y / b);

        public static Vector2i operator *(Vector2i a, int b) => new Vector2i(a.X * b, a.Y * b);

        public static Vector2i operator %(Vector2i a, Vector2i b) => new Vector2i(a.X % b.X, a.Y % b.Y);

        public static Vector2i operator %(Vector2i a, int b) => new Vector2i(a.X % b, a.Y % b);

        public static bool operator <(Vector2i a, Vector2i b) => a.X < b.X && a.Y < b.Y;

        public static bool operator <=(Vector2i a, Vector2i b) => a.X <= b.X && a.Y <= b.Y;

        public static bool operator >(Vector2i a, Vector2i b) => a.X > b.X && a.Y > b.Y;

        public static bool operator >=(Vector2i a, Vector2i b) => a.X >= b.X && a.Y >= b.Y;

        public static bool operator ==(Vector2i a, Vector2i b) => a.X == b.X && a.Y == b.Y;

        public static bool operator !=(Vector2i a, Vector2i b) => a.X != b.X || a.Y != b.Y;

        public static Vector2i Max(Vector2i a, Vector2i b) => new Vector2i(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

        public static Vector2i Min(Vector2i a, Vector2i b) => new Vector2i(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));

        public Vector3i ToVector3i(int z) => new Vector3i(X, Y, z);

        public Vector2i DivideUp(Vector2i a, int b) => new Vector2i(a.X.DivideUp(b), a.Y.DivideUp(b));

        public Vector2f ToVector2f() => new Vector2f(X, Y);

        public Vector3f ToVector3f(float z) => new Vector3f(X, Y, z);

        public Range2i ToRange2i() => Range2i.FromMinAndSize(Zero, this);

        public bool Equals(Vector2i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Vector2i))
                return false;

            return Equals((Vector2i) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return X + Y * 7;
            }
        }

        public override string ToString() => string.Format("({0} , {1})", X, Y);
    }
}