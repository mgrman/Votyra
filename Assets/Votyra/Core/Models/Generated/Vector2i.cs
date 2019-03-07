using System;
using Newtonsoft.Json;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Vector2i : IEquatable<Vector2i>
    {
        public static readonly Vector2i Zero = new Vector2i(0, 0);

        public static readonly Vector2i One = new Vector2i(1, 1);

        public readonly int X;

        public readonly int Y;

        [JsonConstructor]
        public Vector2i(int x, int y)
        {
            X = x;

            Y = y;
        }

        public static Vector2i operator -(Vector2i a) => new Vector2i(-a.X, -a.Y);

        public static Vector2i operator *(Vector2i a, Vector2i b) => new Vector2i(a.X * b.X, a.Y * b.Y);

        public static Vector2i operator *(Vector2i a, int b) => new Vector2i(a.X * b, a.Y * b);
        public static Vector2i operator *(int a, Vector2i b) => new Vector2i(a * b.X, a * b.Y);

        public static Vector2i operator +(Vector2i a, Vector2i b) => new Vector2i(a.X + b.X, a.Y + b.Y);

        public static Vector2i operator +(Vector2i a, int b) => new Vector2i(a.X + b, a.Y + b);
        public static Vector2i operator +(int a, Vector2i b) => new Vector2i(a + b.X, a + b.Y);

        public static Vector2i operator -(Vector2i a, Vector2i b) => new Vector2i(a.X - b.X, a.Y - b.Y);

        public static Vector2i operator -(Vector2i a, int b) => new Vector2i(a.X - b, a.Y - b);
        public static Vector2i operator -(int a, Vector2i b) => new Vector2i(a - b.X, a - b.Y);

        public static Vector2i operator /(Vector2i a, Vector2i b) => new Vector2i(a.X / b.X, a.Y / b.Y);

        public static Vector2i operator /(Vector2i a, int b) => new Vector2i(a.X / b, a.Y / b);
        public static Vector2i operator /(int a, Vector2i b) => new Vector2i(a / b.X, a / b.Y);

        public static Vector2i operator %(Vector2i a, Vector2i b) => new Vector2i(a.X % b.X, a.Y % b.Y);

        public static Vector2i operator %(Vector2i a, int b) => new Vector2i(a.X % b, a.Y % b);
        public static Vector2i operator %(int a, Vector2i b) => new Vector2i(a % b.X, a % b.Y);

        public static bool operator <(Vector2i a, Vector2i b) => a.X < b.X && a.Y < b.Y;

        public static bool operator <(Vector2i a, int b) => a.X < b && a.Y < b;
        public static bool operator <(int a, Vector2i b) => a < b.X && a < b.Y;

        public static bool operator >(Vector2i a, Vector2i b) => a.X > b.X && a.Y > b.Y;

        public static bool operator >(Vector2i a, int b) => a.X > b && a.Y > b;
        public static bool operator >(int a, Vector2i b) => a > b.X && a > b.Y;

        public static bool operator <=(Vector2i a, Vector2i b) => a.X <= b.X && a.Y <= b.Y;

        public static bool operator <=(Vector2i a, int b) => a.X <= b && a.Y <= b;
        public static bool operator <=(int a, Vector2i b) => a <= b.X && a <= b.Y;

        public static bool operator >=(Vector2i a, Vector2i b) => a.X >= b.X && a.Y >= b.Y;

        public static bool operator >=(Vector2i a, int b) => a.X >= b && a.Y >= b;
        public static bool operator >=(int a, Vector2i b) => a >= b.X && a >= b.Y;

        public static bool operator ==(Vector2i a, Vector2i b) => a.X == b.X && a.Y == b.Y;

        public static bool operator ==(Vector2i a, int b) => a.X == b && a.Y == b;
        public static bool operator ==(int a, Vector2i b) => a == b.X && a == b.Y;

        public static bool operator !=(Vector2i a, Vector2i b) => a.X != b.X || a.Y != b.Y;

        public static bool operator !=(Vector2i a, int b) => a.X != b || a.Y != b;
        public static bool operator !=(int a, Vector2i b) => a != b.X || a != b.Y;

        public static Vector2f operator *(Vector2i a, Vector2f b) => new Vector2f(a.X * b.X, a.Y * b.Y);
        public static Vector2f operator *(Vector2f a, Vector2i b) => new Vector2f(a.X * b.X, a.Y * b.Y);
        public static Vector2f operator *(Vector2i a, float b) => new Vector2f(a.X * b, a.Y * b);
        public static Vector2f operator *(float a, Vector2i b) => new Vector2f(a * b.X, a * b.Y);

        public static Vector2f operator +(Vector2i a, Vector2f b) => new Vector2f(a.X + b.X, a.Y + b.Y);
        public static Vector2f operator +(Vector2f a, Vector2i b) => new Vector2f(a.X + b.X, a.Y + b.Y);
        public static Vector2f operator +(Vector2i a, float b) => new Vector2f(a.X + b, a.Y + b);
        public static Vector2f operator +(float a, Vector2i b) => new Vector2f(a + b.X, a + b.Y);

        public static Vector2f operator -(Vector2i a, Vector2f b) => new Vector2f(a.X - b.X, a.Y - b.Y);
        public static Vector2f operator -(Vector2f a, Vector2i b) => new Vector2f(a.X - b.X, a.Y - b.Y);
        public static Vector2f operator -(Vector2i a, float b) => new Vector2f(a.X - b, a.Y - b);
        public static Vector2f operator -(float a, Vector2i b) => new Vector2f(a - b.X, a - b.Y);

        public static Vector2f operator /(Vector2i a, Vector2f b) => new Vector2f(a.X / b.X, a.Y / b.Y);
        public static Vector2f operator /(Vector2f a, Vector2i b) => new Vector2f(a.X / b.X, a.Y / b.Y);
        public static Vector2f operator /(Vector2i a, float b) => new Vector2f(a.X / b, a.Y / b);
        public static Vector2f operator /(float a, Vector2i b) => new Vector2f(a / b.X, a / b.Y);

        public static Vector2f operator %(Vector2i a, Vector2f b) => new Vector2f(a.X % b.X, a.Y % b.Y);
        public static Vector2f operator %(Vector2f a, Vector2i b) => new Vector2f(a.X % b.X, a.Y % b.Y);
        public static Vector2f operator %(Vector2i a, float b) => new Vector2f(a.X % b, a.Y % b);
        public static Vector2f operator %(float a, Vector2i b) => new Vector2f(a % b.X, a % b.Y);

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
                return (X * 397) ^ (Y * 397);
            }
        }

        public override string ToString() => "(" + X + "," + Y + ")";
    }

    public static class Vector2iUtils
    {
        public static float Magnitude(this Vector2i @this) => (float) Math.Sqrt(@this.SqrMagnitude());

        public static Vector2f Normalized(this Vector2i @this)
        {
            var magnitude = @this.Magnitude();
            return magnitude <= float.Epsilon ? @this.ToVector2f() : @this / magnitude;
        }

        public static float SqrMagnitude(this Vector2i @this) => @this.X * @this.X + @this.Y * @this.Y;

        public static int ManhattanMagnitude(this Vector2i @this) => @this.X + @this.Y;

        public static int AreaSum(this Vector2i @this) => @this.X * @this.Y;

        public static int Volume(this Vector2i @this) => @this.X * @this.Y;

        public static Vector2i XX(this Vector2i @this) => new Vector2i(@this.X, @this.X);

        public static Vector2i XY(this Vector2i @this) => new Vector2i(@this.X, @this.Y);

        public static Vector2i YX(this Vector2i @this) => new Vector2i(@this.Y, @this.X);

        public static Vector2i YY(this Vector2i @this) => new Vector2i(@this.Y, @this.Y);

        public static bool AllPositive(this Vector2i @this) => @this.X > 0 && @this.Y > 0;

        public static bool AllZeroOrPositive(this Vector2i @this) => @this.X >= 0 && @this.Y >= 0;

        public static bool AnyNegative(this Vector2i @this) => @this.X < 0 || @this.Y < 0;

        public static bool AnyZero(this Vector2i @this) => @this.X == 0 || @this.Y == 0;

        public static bool AnyZeroOrNegative(this Vector2i @this) => @this.X <= 0 || @this.Y <= 0;

        public static Vector2i FromSame(int value) => new Vector2i(value, value);

        public static Vector2f ToVector2f(this Vector2i @this) => new Vector2f(@this.X, @this.Y);

        public static Range2i ToRange2i(this Vector2i @this) => Range2i.FromMinAndSize(Vector2i.Zero, @this);
        public static Area2i ToArea2i(this Vector2i @this) => Area2i.FromMinAndSize(Vector2i.Zero, @this);

        public static Vector3i ToVector3i(this Vector2i @this, int x3) => new Vector3i(@this.X, @this.Y, x3);
        public static Vector3f ToVector3f(this Vector2i @this, float x3) => new Vector3f(@this.X, @this.Y, x3);

        public static Vector4i ToVector4i(this Vector2i @this, int x3, int x4) => new Vector4i(@this.X, @this.Y, x3, x4);
        public static Vector4f ToVector4f(this Vector2i @this, float x3, float x4) => new Vector4f(@this.X, @this.Y, x3, x4);

        public static Vector5i ToVector5i(this Vector2i @this, int x3, int x4, int x5) => new Vector5i(@this.X, @this.Y, x3, x4, x5);
        public static Vector5f ToVector5f(this Vector2i @this, float x3, float x4, float x5) => new Vector5f(@this.X, @this.Y, x3, x4, x5);

        public static Vector6i ToVector6i(this Vector2i @this, int x3, int x4, int x5, int x6) => new Vector6i(@this.X, @this.Y, x3, x4, x5, x6);
        public static Vector6f ToVector6f(this Vector2i @this, float x3, float x4, float x5, float x6) => new Vector6f(@this.X, @this.Y, x3, x4, x5, x6);

        public static Vector2i Max(Vector2i a, Vector2i b) => new Vector2i(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

        public static Vector2i Max(Vector2i a, int b) => new Vector2i(Math.Max(a.X, b), Math.Max(a.Y, b));
        public static Vector2i Max(int a, Vector2i b) => new Vector2i(Math.Max(a, b.X), Math.Max(a, b.Y));

        public static Vector2i Min(Vector2i a, Vector2i b) => new Vector2i(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));

        public static Vector2i Min(Vector2i a, int b) => new Vector2i(Math.Min(a.X, b), Math.Min(a.Y, b));
        public static Vector2i Min(int a, Vector2i b) => new Vector2i(Math.Min(a, b.X), Math.Min(a, b.Y));

        public static Vector2i DivideUp(Vector2i a, Vector2i b) => new Vector2i(a.X.DivideUp(b.X), a.Y.DivideUp(b.Y));

        public static Vector2i DivideUp(Vector2i a, int b) => new Vector2i(a.X.DivideUp(b), a.Y.DivideUp(b));
        public static Vector2i DivideUp(int a, Vector2i b) => new Vector2i(a.DivideUp(b.X), a.DivideUp(b.Y));
    }
}