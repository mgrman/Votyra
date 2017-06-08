using System;
using Votyra.Common.Utils;
using UnityEngine;

namespace Votyra.Common.Models
{
    public struct Vector2i : IEquatable<Vector2i>
    {
        public readonly int x;
        public readonly int y;

        public Vector2i(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2i(Vector2 vec)
        {
            this.x = vec.x.RoundToInt();
            this.y = vec.y.RoundToInt();
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

        public bool IsAsIndexContained(Vector2i size)
        {
            return x >= 0 && y >= 0 && x < size.x && y < size.y;
        }

        public int AreaSum { get { return x * y; } }

        public static readonly Vector2i Zero = new Vector2i();
        public static readonly Vector2i One = new Vector2i(1, 1);

        public static Vector2i operator +(Vector2i a, int b)
        {
            return new Vector2i(a.x + b, a.y + b);
        }

        public static Vector2i operator -(Vector2i a, int b)
        {
            return new Vector2i(a.x - b, a.y - b);
        }

        public static Vector2i operator +(Vector2i a, Vector2i b)
        {
            return new Vector2i(a.x + b.x, a.y + b.y);
        }

        public static Vector2i operator -(Vector2i a, Vector2i b)
        {
            return new Vector2i(a.x - b.x, a.y - b.y);
        }

        public static Vector2 operator *(Vector2 a, Vector2i b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }

        public static Vector2 operator *(Vector2i a, Vector2 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }

        public static Vector2i operator *(Vector2i a, Vector2i b)
        {
            return new Vector2i(a.x * b.x, a.y * b.y);
        }

        public static Vector2i operator /(Vector2i a, Vector2i b)
        {
            return new Vector2i(a.x / b.x, a.y / b.y);
        }

        public static Vector2 operator /(Vector2i a, Vector2 b)
        {
            return new Vector2(a.x / b.x, a.y / b.y);
        }


        public static Vector2i operator /(Vector2i a, int b)
        {
            return new Vector2i(a.x / b, a.y / b);
        }

        public static Vector2 operator /(Vector2i a, float b)
        {
            return new Vector2(a.x / b, a.y / b);
        }
        public Vector2i DivideUp(Vector2i a, int b)
        {
            return new Vector2i(a.x.DivideUp(b), a.y.DivideUp(b));
        }

        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }

        public static bool operator <(Vector2i a, Vector2i b)
        {
            return a.x < b.x && a.y < b.y;
        }

        public static bool operator <=(Vector2i a, Vector2i b)
        {
            return a.x < b.x && a.y < b.y;
        }

        public static bool operator >(Vector2i a, Vector2i b)
        {
            return a.x > b.x && a.y > b.y;
        }

        public static bool operator >=(Vector2i a, Vector2i b)
        {
            return a.x >= b.x && a.y >= b.y;
        }

        public static bool operator ==(Vector2i a, Vector2i b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Vector2i a, Vector2i b)
        {
            return a.x != b.x || a.y != b.y;
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
            return x + y * 7;
        }

        public override string ToString()
        {
            return string.Format("({0} , {1})", x, y);
        }
    }
}