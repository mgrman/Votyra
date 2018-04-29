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
        public bool AnyNegative => this.X < 0 || this.Y < 0;
        public bool AnyZero => this.X == 0 || this.Y == 0;
        public bool AnyZeroOrNegative => this.X <= 0 || this.Y <= 0;

        public int AreaSum => X * Y;

        public Vector2i(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static Vector2i FromSame(int value)
        {
            return new Vector2i(value, value);
        }

        public Vector3i ToVector3i(int z)
        {
            return new Vector3i(X, Y, z);
        }

        public void ForeachPointExlusive(Action<Vector2i> action)
        {
            for (int ix = 0; ix < this.X; ix++)
            {
                for (int iy = 0; iy < this.Y; iy++)
                {
                    action(new Vector2i(ix, iy));
                }
            }
        }

        public static Vector2i operator +(Vector2i a, int b)
        {
            return new Vector2i(a.X + b, a.Y + b);
        }

        public static Vector2i operator -(Vector2i a, int b)
        {
            return new Vector2i(a.X - b, a.Y - b);
        }

        public static Vector2i operator +(Vector2i a, Vector2i b)
        {
            return new Vector2i(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2i operator -(Vector2i a, Vector2i b)
        {
            return new Vector2i(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2f operator *(Vector2f a, Vector2i b)
        {
            return new Vector2f(a.X * b.X, a.Y * b.Y);
        }

        public static Vector2f operator *(Vector2i a, Vector2f b)
        {
            return new Vector2f(a.X * b.X, a.Y * b.Y);
        }

        public static Vector2i operator *(Vector2i a, Vector2i b)
        {
            return new Vector2i(a.X * b.X, a.Y * b.Y);
        }

        public static Vector2i operator /(Vector2i a, Vector2i b)
        {
            return new Vector2i(a.X / b.X, a.Y / b.Y);
        }

        public static Vector2f operator /(Vector2i a, Vector2f b)
        {
            return new Vector2f(a.X / b.X, a.Y / b.Y);
        }

        public static Vector2i operator /(Vector2i a, int b)
        {
            return new Vector2i(a.X / b, a.Y / b);
        }

        public static Vector2f operator /(Vector2i a, float b)
        {
            return new Vector2f(a.X / b, a.Y / b);
        }

        public static Vector2i operator *(Vector2i a, int b)
        {
            return new Vector2i(a.X * b, a.Y * b);
        }

        public Vector2i DivideUp(Vector2i a, int b)
        {
            return new Vector2i(a.X.DivideUp(b), a.Y.DivideUp(b));
        }

        public Vector2f ToVector2f()
        {
            return new Vector2f(X, Y);
        }

        public Range2i ToRange2i()
        {
            return Range2i.FromMinAndSize(Vector2i.Zero, this);
        }

        public static bool operator <(Vector2i a, Vector2i b)
        {
            return a.X < b.X && a.Y < b.Y;
        }

        public static bool operator <=(Vector2i a, Vector2i b)
        {
            return a.X < b.X && a.Y < b.Y;
        }

        public static bool operator >(Vector2i a, Vector2i b)
        {
            return a.X > b.X && a.Y > b.Y;
        }

        public static bool operator >=(Vector2i a, Vector2i b)
        {
            return a.X >= b.X && a.Y >= b.Y;
        }

        public static bool operator ==(Vector2i a, Vector2i b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Vector2i a, Vector2i b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static Vector2i Max(Vector2i a, Vector2i b)
        {
            return new Vector2i(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
        }

        public static Vector2i Min(Vector2i a, Vector2i b)
        {
            return new Vector2i(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
        }

        public bool Equals(Vector2i other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector2i))
                return false;

            return this.Equals((Vector2i)obj);
        }

        public override int GetHashCode()
        {
            return X + Y * 7;
        }

        public override string ToString()
        {
            return string.Format("({0} , {1})", X, Y);
        }
    }
}