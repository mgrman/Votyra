using System;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Vector2f : IEquatable<Vector2f>
    {
        public readonly float x;
        public readonly float y;

        public Vector2f(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2f(Vector2f vec)
        {
            this.x = vec.x;
            this.y = vec.y;
        }

        public bool BothPositive
        {
            get
            {
                return this.x > 0 && this.y > 0;
            }
        }

        public bool BothZeroOrPositive
        {
            get
            {
                return this.x >= 0 && this.y >= 0;
            }
        }

        public bool AnyNegative
        {
            get
            {
                return this.x < 0 || this.y < 0;
            }
        }

        public float AreaSum { get { return x * y; } }

        public static readonly Vector2f Zero = new Vector2f();
        public static readonly Vector2f One = new Vector2f(1, 1);

        public static Vector2f operator +(Vector2f a, float b)
        {
            return new Vector2f(a.x + b, a.y + b);
        }

        public static Vector2f operator -(Vector2f a, int b)
        {
            return new Vector2f(a.x - b, a.y - b);
        }

        public static Vector2f operator +(Vector2f a, Vector2f b)
        {
            return new Vector2f(a.x + b.x, a.y + b.y);
        }

        public static Vector2f operator -(Vector2f a, Vector2f b)
        {
            return new Vector2f(a.x - b.x, a.y - b.y);
        }

        public static Vector2f operator *(Vector2f a, Vector2f b)
        {
            return new Vector2f(a.x * b.x, a.y * b.y);
        }

        public static Vector2f operator /(Vector2f a, Vector2f b)
        {
            return new Vector2f(a.x / b.x, a.y / b.y);
        }


        public static Vector2f operator /(Vector2f a, float b)
        {
            return new Vector2f(a.x / b, a.y / b);
        }

        public static Vector2f operator *(Vector2f a, float b)
        {
            return new Vector2f(a.x * b, a.y * b);
        }


        public Rect2f ToRect2f()
        {
            return new Rect2f(Vector2f.Zero, this);
        }

        public static bool operator <(Vector2f a, Vector2f b)
        {
            return a.x < b.x && a.y < b.y;
        }

        public static bool operator <=(Vector2f a, Vector2f b)
        {
            return a.x < b.x && a.y < b.y;
        }

        public static bool operator >(Vector2f a, Vector2f b)
        {
            return a.x > b.x && a.y > b.y;
        }

        public static bool operator >=(Vector2f a, Vector2f b)
        {
            return a.x >= b.x && a.y >= b.y;
        }

        public static bool operator ==(Vector2f a, Vector2f b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Vector2f a, Vector2f b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public static Vector2f Max(Vector2f a, Vector2f b)
        {
            return new Vector2f(Math.Max(a.x, b.x), Math.Max(a.y, b.y));
        }

        public static Vector2f Min(Vector2f a, Vector2f b)
        {
            return new Vector2f(Math.Min(a.x, b.x), Math.Min(a.y, b.y));
        }

        public Vector2i ToVector2i()
        {
            return new Vector2i(this);
        }

        public Vector2i FloorToVector2i()
        {
            return new Vector2i(MathUtils.FloorToInt(this.x), MathUtils.FloorToInt(this.y));
        }

        public Vector2i CeilToVector2i()
        {
            return new Vector2i(MathUtils.CeilToInt(this.x), MathUtils.CeilToInt(this.y));
        }

        public Vector2f ToAbs()
        {
            return new Vector2f(Math.Abs(this.x), Math.Abs(this.y));
        }

        public Vector2f DivideBy(Vector2f that)
        {
            return new Vector2f(this.x / that.x, this.y / that.y);
        }

        public Vector2f DivideBy(Vector2i that)
        {
            return new Vector2f(this.x / that.x, this.y / that.y);
        }

        public Vector2f MinBy(Vector2f that)
        {
            return new Vector2f(Math.Min(this.x, that.x), Math.Min(this.y, that.y));
        }

        public Vector2f MaxBy(Vector2f that)
        {
            return new Vector2f(Math.Max(this.x, that.x), Math.Max(this.y, that.y));
        }

        public Vector3f ToVector3(float z)
        {
            return new Vector3f(x, y, z);
        }

        public bool ZeroOrPositive => x >= 0 && y >= 0;


        public bool Positive => x > 0 && y > 0;


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

        public override int GetHashCode()
        {
            return x.GetHashCode() + y.GetHashCode() * 7;
        }

        public override string ToString()
        {
            return string.Format("({0} , {1})", x, y);
        }
    }
}