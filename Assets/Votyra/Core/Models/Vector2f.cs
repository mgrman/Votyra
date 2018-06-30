using System;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Vector2f : IEquatable<Vector2f>
    {
        public static readonly Vector2f One = new Vector2f(1, 1);
        public static readonly Vector2f Zero = new Vector2f();
        public readonly float X;

        public readonly float Y;

        public Vector2f(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public bool AnyNegative => this.X < 0 || this.Y < 0;

        public float AreaSum => X * Y;

        public static Vector2f FromSame(float value)
        {
            return new Vector2f(value, value);
        }

        public static Vector2f Max(Vector2f a, Vector2f b)
        {
            return new Vector2f(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
        }

        public static Vector2f Min(Vector2f a, Vector2f b)
        {
            return new Vector2f(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
        }

        public static Vector2f operator -(Vector2f a, int b)
        {
            return new Vector2f(a.X - b, a.Y - b);
        }

        public static Vector2f operator -(Vector2f a, Vector2f b)
        {
            return new Vector2f(a.X - b.X, a.Y - b.Y);
        }

        public static bool operator !=(Vector2f a, Vector2f b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static Vector2f operator *(Vector2f a, Vector2f b)
        {
            return new Vector2f(a.X * b.X, a.Y * b.Y);
        }

        public static Vector2f operator *(Vector2f a, float b)
        {
            return new Vector2f(a.X * b, a.Y * b);
        }

        public static Vector2f operator /(Vector2f a, Vector2f b)
        {
            return new Vector2f(a.X / b.X, a.Y / b.Y);
        }

        public static Vector2f operator /(Vector2f a, float b)
        {
            return new Vector2f(a.X / b, a.Y / b);
        }

        public static Vector2f operator +(Vector2f a, float b)
        {
            return new Vector2f(a.X + b, a.Y + b);
        }

        public static Vector2f operator +(Vector2f a, Vector2f b)
        {
            return new Vector2f(a.X + b.X, a.Y + b.Y);
        }

        public static bool operator <(Vector2f a, Vector2f b)
        {
            return a.X < b.X && a.Y < b.Y;
        }

        public static bool operator <=(Vector2f a, Vector2f b)
        {
            return a.X < b.X && a.Y < b.Y;
        }

        public static bool operator ==(Vector2f a, Vector2f b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator >(Vector2f a, Vector2f b)
        {
            return a.X > b.X && a.Y > b.Y;
        }

        public static bool operator >=(Vector2f a, Vector2f b)
        {
            return a.X >= b.X && a.Y >= b.Y;
        }

        public Vector2i CeilToVector2i()
        {
            return new Vector2i(MathUtils.CeilToInt(this.X), MathUtils.CeilToInt(this.Y));
        }

        public bool Equals(Vector2f other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector2f))
                return false;

            return this.Equals((Vector2f)obj);
        }

        public Vector2i FloorToVector2i()
        {
            return new Vector2i(MathUtils.FloorToInt(this.X), MathUtils.FloorToInt(this.Y));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return X.GetHashCode() + Y.GetHashCode() * 7;
            }
        }

        public Vector2i RoundToVector2i()
        {
            return new Vector2i(MathUtils.RoundToInt(this.X), MathUtils.RoundToInt(this.Y));
        }

        public Range2f ToRange2f()
        {
            return Range2f.FromMinAndSize(Vector2f.Zero, this);
        }

        public override string ToString()
        {
            return string.Format("({0} , {1})", X, Y);
        }

        public Vector3f ToVector3f(float z)
        {
            return new Vector3f(X, Y, z);
        }
    }
}