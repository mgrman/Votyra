using System;

namespace UnityEngine
{
    public struct Vector2 : IEquatable<Vector2>
    {
        public readonly float x;
        public readonly float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        
        public bool Positive
        {
            get
            {
                return this.x > 0 && this.y > 0;
            }
        }

        public bool ZeroOrPositive
        {
            get
            {
                return this.x >= 0 && this.y >= 0;
            }
        }

        public float AreaSum { get { return x * y; } }

        public static readonly Vector2 Zero = new Vector2();
        public static readonly Vector2 One = new Vector2(1, 1);


        public static bool operator >(Vector2 a, Vector2 b)
        {
            return a.x > b.x && a.y > b.y;
        }

        public static bool operator >=(Vector2 a, Vector2 b)
        {
            return a.x >= b.x && a.y >= b.y;
        }

        public static bool operator <(Vector2 a, Vector2 b)
        {
            return a.x < b.x && a.y < b.y;
        }

        public static bool operator <=(Vector2 a, Vector2 b)
        {
            return a.x <= b.x && a.y <= b.y;
        }

        public static Vector2 operator +(Vector2 a, float b)
        {
            return new Vector2(a.x + b, a.y + b);
        }

        public static Vector2 operator -(Vector2 a, float b)
        {
            return new Vector2(a.x - b, a.y - b);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        public static Vector2 operator *(Vector2 a, float b)
        {
            return new Vector2(a.x * b, a.y * b);
        }

        public static Vector2 operator *(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }

        public static Vector2 operator /(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x / b.x, a.y / b.y);
        }

        public static Vector2 operator /(Vector2 a, float b)
        {
            return new Vector2(a.x / b, a.y / b);
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public bool Equals(Vector2 other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector2))
                return false;

            return this.Equals((Vector2)obj);
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