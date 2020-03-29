using System;
using Newtonsoft.Json;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Vector2f : IEquatable<Vector2f>
    {
        public static readonly Vector2f One = new Vector2f(1, 1);
        public static readonly Vector2f Zero = new Vector2f(0, 0);
        public static readonly Vector2f NaN = new Vector2f(float.NaN, float.NaN);

        public readonly float X;

        public readonly float Y;

        [JsonConstructor]
        public Vector2f(float x, float y)
        {
            X = x;

            Y = y;
        }

        public static Vector2f operator -(Vector2f a) => new Vector2f(-a.X, -a.Y);

        public static Vector2f operator *(Vector2f a, Vector2f b) => new Vector2f(a.X * b.X, a.Y * b.Y);

        public static Vector2f operator *(Vector2f a, float b) => new Vector2f(a.X * b, a.Y * b);

        public static Vector2f operator *(float a, Vector2f b) => new Vector2f(a * b.X, a * b.Y);

        public static Vector2f operator *(Vector2f a, int b) => new Vector2f(a.X * b, a.Y * b);

        public static Vector2f operator *(int a, Vector2f b) => new Vector2f(a * b.X, a * b.Y);

        public static Vector2f operator +(Vector2f a, Vector2f b) => new Vector2f(a.X + b.X, a.Y + b.Y);

        public static Vector2f operator +(Vector2f a, float b) => new Vector2f(a.X + b, a.Y + b);

        public static Vector2f operator +(float a, Vector2f b) => new Vector2f(a + b.X, a + b.Y);

        public static Vector2f operator +(Vector2f a, int b) => new Vector2f(a.X + b, a.Y + b);

        public static Vector2f operator +(int a, Vector2f b) => new Vector2f(a + b.X, a + b.Y);

        public static Vector2f operator -(Vector2f a, Vector2f b) => new Vector2f(a.X - b.X, a.Y - b.Y);

        public static Vector2f operator -(Vector2f a, float b) => new Vector2f(a.X - b, a.Y - b);

        public static Vector2f operator -(float a, Vector2f b) => new Vector2f(a - b.X, a - b.Y);

        public static Vector2f operator -(Vector2f a, int b) => new Vector2f(a.X - b, a.Y - b);

        public static Vector2f operator -(int a, Vector2f b) => new Vector2f(a - b.X, a - b.Y);

        public static Vector2f operator /(Vector2f a, Vector2f b) => new Vector2f(a.X / b.X, a.Y / b.Y);

        public static Vector2f operator /(Vector2f a, float b) => new Vector2f(a.X / b, a.Y / b);

        public static Vector2f operator /(float a, Vector2f b) => new Vector2f(a / b.X, a / b.Y);

        public static Vector2f operator /(Vector2f a, int b) => new Vector2f(a.X / b, a.Y / b);

        public static Vector2f operator /(int a, Vector2f b) => new Vector2f(a / b.X, a / b.Y);

        public static Vector2f operator %(Vector2f a, Vector2f b) => new Vector2f(a.X % b.X, a.Y % b.Y);

        public static Vector2f operator %(Vector2f a, float b) => new Vector2f(a.X % b, a.Y % b);

        public static Vector2f operator %(float a, Vector2f b) => new Vector2f(a % b.X, a % b.Y);

        public static Vector2f operator %(Vector2f a, int b) => new Vector2f(a.X % b, a.Y % b);

        public static Vector2f operator %(int a, Vector2f b) => new Vector2f(a % b.X, a % b.Y);

        public static bool operator <(Vector2f a, Vector2f b) => a.X < b.X && a.Y < b.Y;

        public static bool operator <(Vector2f a, float b) => a.X < b && a.Y < b;

        public static bool operator <(float a, Vector2f b) => a < b.X && a < b.Y;

        public static bool operator <(Vector2f a, int b) => a.X < b && a.Y < b;

        public static bool operator <(int a, Vector2f b) => a < b.X && a < b.Y;

        public static bool operator >(Vector2f a, Vector2f b) => a.X > b.X && a.Y > b.Y;

        public static bool operator >(Vector2f a, float b) => a.X > b && a.Y > b;

        public static bool operator >(float a, Vector2f b) => a > b.X && a > b.Y;

        public static bool operator >(Vector2f a, int b) => a.X > b && a.Y > b;

        public static bool operator >(int a, Vector2f b) => a > b.X && a > b.Y;

        public static bool operator <=(Vector2f a, Vector2f b) => a.X <= b.X && a.Y <= b.Y;

        public static bool operator <=(Vector2f a, float b) => a.X <= b && a.Y <= b;

        public static bool operator <=(float a, Vector2f b) => a <= b.X && a <= b.Y;

        public static bool operator <=(Vector2f a, int b) => a.X <= b && a.Y <= b;

        public static bool operator <=(int a, Vector2f b) => a <= b.X && a <= b.Y;

        public static bool operator >=(Vector2f a, Vector2f b) => a.X >= b.X && a.Y >= b.Y;

        public static bool operator >=(Vector2f a, float b) => a.X >= b && a.Y >= b;

        public static bool operator >=(float a, Vector2f b) => a >= b.X && a >= b.Y;

        public static bool operator >=(Vector2f a, int b) => a.X >= b && a.Y >= b;

        public static bool operator >=(int a, Vector2f b) => a >= b.X && a >= b.Y;

        public static bool operator ==(Vector2f a, Vector2f b) => a.X == b.X && a.Y == b.Y;

        public static bool operator ==(Vector2f a, float b) => a.X == b && a.Y == b;

        public static bool operator ==(float a, Vector2f b) => a == b.X && a == b.Y;

        public static bool operator ==(Vector2f a, int b) => a.X == b && a.Y == b;

        public static bool operator ==(int a, Vector2f b) => a == b.X && a == b.Y;

        public static bool operator !=(Vector2f a, Vector2f b) => a.X != b.X || a.Y != b.Y;

        public static bool operator !=(Vector2f a, float b) => a.X != b || a.Y != b;

        public static bool operator !=(float a, Vector2f b) => a != b.X || a != b.Y;

        public static bool operator !=(Vector2f a, int b) => a.X != b || a.Y != b;

        public static bool operator !=(int a, Vector2f b) => a != b.X || a != b.Y;

        public bool Equals(Vector2f other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Vector2f))
            {
                return false;
            }

            return Equals((Vector2f) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ (Y.GetHashCode() * 397);
            }
        }

        public override string ToString() => "(" + X + "," + Y + ")";
    }

    public static class Vector2fUtils
    {
        public static Vector2f XX(this Vector2f @this) => new Vector2f(@this.X, @this.X);

        public static Vector2f XY(this Vector2f @this) => new Vector2f(@this.X, @this.Y);

        public static Vector2f YX(this Vector2f @this) => new Vector2f(@this.Y, @this.X);

        public static Vector2f YY(this Vector2f @this) => new Vector2f(@this.Y, @this.Y);

        public static bool AnyNegative(this Vector2f @this) => @this.X < 0 || @this.Y < 0;

        public static bool AnyNan(this Vector2f @this) => float.IsNaN(@this.X) || float.IsNaN(@this.Y);

        public static bool NoNan(this Vector2f @this) => !float.IsNaN(@this.X) && !float.IsNaN(@this.Y);

        public static bool AnyInfinity(this Vector2f @this) => float.IsInfinity(@this.X) || float.IsInfinity(@this.Y);

        public static float AreaSum(this Vector2f @this) => @this.X * @this.Y;

        public static float Magnitude(this Vector2f @this) => (float) Math.Sqrt(@this.SqrMagnitude());

        public static Vector2f Normalized(this Vector2f @this)
        {
            var magnitude = @this.Magnitude();
            return magnitude <= float.Epsilon ? @this : @this / magnitude;
        }

        public static float SqrMagnitude(this Vector2f @this) => @this.X * @this.X + @this.Y * @this.Y;

        public static Vector2f Perpendicular(this Vector2f @this) => new Vector2f(@this.Y, -@this.X);

        public static Vector2f FromSame(float value) => new Vector2f(value, value);

        public static float Dot(Vector2f a, Vector2f b) => (float) ((double) a.X * b.X + (double) a.Y * b.Y);

        public static double Determinant(Vector2f a, Vector2f b) => a.X * b.Y - a.Y * b.X;

        public static Vector2f Ceil(this Vector2f @this) => new Vector2f(@this.X.CeilToInt(), @this.Y.CeilToInt());

        public static Vector2i CeilToVector2i(this Vector2f @this) => new Vector2i(@this.X.CeilToInt(), @this.Y.CeilToInt());

        public static Vector2f Abs(this Vector2f @this) => new Vector2f(Math.Abs(@this.X), Math.Abs(@this.Y));

        public static Vector2f Floor(this Vector2f @this) => new Vector2f(@this.X.FloorToInt(), @this.Y.FloorToInt());

        public static Vector2i FloorToVector2i(this Vector2f @this) => new Vector2i(@this.X.FloorToInt(), @this.Y.FloorToInt());

        public static Vector2f Round(this Vector2f @this) => new Vector2f(@this.X.RoundToInt(), @this.Y.RoundToInt());

        public static Vector2i RoundToVector2i(this Vector2f @this) => new Vector2i(@this.X.RoundToInt(), @this.Y.RoundToInt());

        public static Vector3f ToVector3f(this Vector2f @this, float x3) => new Vector3f(@this.X, @this.Y, x3);

        public static Vector4f ToVector4f(this Vector2f @this, float x3, float x4) => new Vector4f(@this.X, @this.Y, x3, x4);

        public static Vector5f ToVector5f(this Vector2f @this, float x3, float x4, float x5) => new Vector5f(@this.X, @this.Y, x3, x4, x5);

        public static Vector6f ToVector6f(this Vector2f @this, float x3, float x4, float x5, float x6) => new Vector6f(@this.X, @this.Y, x3, x4, x5, x6);

        public static Area2f ToArea2f(this Vector2f @this) => Area2f.FromMinAndSize(Vector2f.Zero, @this);

        public static Vector2f Max(Vector2f a, Vector2f b) => new Vector2f(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

        public static Vector2f Max(Vector2f a, float b) => new Vector2f(Math.Max(a.X, b), Math.Max(a.Y, b));

        public static Vector2f Max(float a, Vector2f b) => new Vector2f(Math.Max(a, b.X), Math.Max(a, b.Y));

        public static Vector2f Max(Vector2f a, int b) => new Vector2f(Math.Max(a.X, b), Math.Max(a.Y, b));

        public static Vector2f Max(int a, Vector2f b) => new Vector2f(Math.Max(a, b.X), Math.Max(a, b.Y));

        public static Vector2f Min(Vector2f a, Vector2f b) => new Vector2f(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));

        public static Vector2f Min(Vector2f a, float b) => new Vector2f(Math.Min(a.X, b), Math.Min(a.Y, b));

        public static Vector2f Min(float a, Vector2f b) => new Vector2f(Math.Min(a, b.X), Math.Min(a, b.Y));

        public static Vector2f Min(Vector2f a, int b) => new Vector2f(Math.Min(a.X, b), Math.Min(a.Y, b));

        public static Vector2f Min(int a, Vector2f b) => new Vector2f(Math.Min(a, b.X), Math.Min(a, b.Y));

        public static bool IsApproximatelyEqual(Vector2f a, Vector2f b) => a.X.IsApproximatelyEqual(b.X) && a.Y.IsApproximatelyEqual(b.Y);

        public static bool IsApproximatelyEqual(Vector2f a, float b) => a.X.IsApproximatelyEqual(b) && a.Y.IsApproximatelyEqual(b);

        public static bool IsApproximatelyEqual(float a, Vector2f b) => a.IsApproximatelyEqual(b.X) && a.IsApproximatelyEqual(b.Y);

        public static bool IsApproximatelyEqual(Vector2f a, int b) => a.X.IsApproximatelyEqual(b) && a.Y.IsApproximatelyEqual(b);

        public static bool IsApproximatelyEqual(int a, Vector2f b) => MathUtils.IsApproximatelyEqual(a, b.X) && MathUtils.IsApproximatelyEqual(a, b.Y);
    }
}
