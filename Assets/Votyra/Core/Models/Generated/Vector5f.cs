using System;
using Newtonsoft.Json;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Vector5f : IEquatable<Vector5f>
    {
        public static readonly Vector5f One = new Vector5f(1, 1, 1, 1, 1);
        public static readonly Vector5f Zero = new Vector5f(0, 0, 0, 0, 0);
        public static readonly Vector5f NaN = new Vector5f(float.NaN, float.NaN, float.NaN, float.NaN, float.NaN);

        public readonly float X0;

        public readonly float X1;

        public readonly float X2;

        public readonly float X3;

        public readonly float X4;

        [JsonConstructor]
        public Vector5f(float x0, float x1, float x2, float x3, float x4)
        {
            X0 = x0;

            X1 = x1;

            X2 = x2;

            X3 = x3;

            X4 = x4;
        }

        public static Vector5f operator -(Vector5f a) => new Vector5f(-a.X0, -a.X1, -a.X2, -a.X3, -a.X4);

        public static Vector5f operator *(Vector5f a, Vector5f b) => new Vector5f(a.X0 * b.X0, a.X1 * b.X1, a.X2 * b.X2, a.X3 * b.X3, a.X4 * b.X4);

        public static Vector5f operator *(Vector5f a, float b) => new Vector5f(a.X0 * b, a.X1 * b, a.X2 * b, a.X3 * b, a.X4 * b);

        public static Vector5f operator *(float a, Vector5f b) => new Vector5f(a * b.X0, a * b.X1, a * b.X2, a * b.X3, a * b.X4);

        public static Vector5f operator *(Vector5f a, int b) => new Vector5f(a.X0 * b, a.X1 * b, a.X2 * b, a.X3 * b, a.X4 * b);

        public static Vector5f operator *(int a, Vector5f b) => new Vector5f(a * b.X0, a * b.X1, a * b.X2, a * b.X3, a * b.X4);

        public static Vector5f operator +(Vector5f a, Vector5f b) => new Vector5f(a.X0 + b.X0, a.X1 + b.X1, a.X2 + b.X2, a.X3 + b.X3, a.X4 + b.X4);

        public static Vector5f operator +(Vector5f a, float b) => new Vector5f(a.X0 + b, a.X1 + b, a.X2 + b, a.X3 + b, a.X4 + b);

        public static Vector5f operator +(float a, Vector5f b) => new Vector5f(a + b.X0, a + b.X1, a + b.X2, a + b.X3, a + b.X4);

        public static Vector5f operator +(Vector5f a, int b) => new Vector5f(a.X0 + b, a.X1 + b, a.X2 + b, a.X3 + b, a.X4 + b);

        public static Vector5f operator +(int a, Vector5f b) => new Vector5f(a + b.X0, a + b.X1, a + b.X2, a + b.X3, a + b.X4);

        public static Vector5f operator -(Vector5f a, Vector5f b) => new Vector5f(a.X0 - b.X0, a.X1 - b.X1, a.X2 - b.X2, a.X3 - b.X3, a.X4 - b.X4);

        public static Vector5f operator -(Vector5f a, float b) => new Vector5f(a.X0 - b, a.X1 - b, a.X2 - b, a.X3 - b, a.X4 - b);

        public static Vector5f operator -(float a, Vector5f b) => new Vector5f(a - b.X0, a - b.X1, a - b.X2, a - b.X3, a - b.X4);

        public static Vector5f operator -(Vector5f a, int b) => new Vector5f(a.X0 - b, a.X1 - b, a.X2 - b, a.X3 - b, a.X4 - b);

        public static Vector5f operator -(int a, Vector5f b) => new Vector5f(a - b.X0, a - b.X1, a - b.X2, a - b.X3, a - b.X4);

        public static Vector5f operator /(Vector5f a, Vector5f b) => new Vector5f(a.X0 / b.X0, a.X1 / b.X1, a.X2 / b.X2, a.X3 / b.X3, a.X4 / b.X4);

        public static Vector5f operator /(Vector5f a, float b) => new Vector5f(a.X0 / b, a.X1 / b, a.X2 / b, a.X3 / b, a.X4 / b);

        public static Vector5f operator /(float a, Vector5f b) => new Vector5f(a / b.X0, a / b.X1, a / b.X2, a / b.X3, a / b.X4);

        public static Vector5f operator /(Vector5f a, int b) => new Vector5f(a.X0 / b, a.X1 / b, a.X2 / b, a.X3 / b, a.X4 / b);

        public static Vector5f operator /(int a, Vector5f b) => new Vector5f(a / b.X0, a / b.X1, a / b.X2, a / b.X3, a / b.X4);

        public static Vector5f operator %(Vector5f a, Vector5f b) => new Vector5f(a.X0 % b.X0, a.X1 % b.X1, a.X2 % b.X2, a.X3 % b.X3, a.X4 % b.X4);

        public static Vector5f operator %(Vector5f a, float b) => new Vector5f(a.X0 % b, a.X1 % b, a.X2 % b, a.X3 % b, a.X4 % b);

        public static Vector5f operator %(float a, Vector5f b) => new Vector5f(a % b.X0, a % b.X1, a % b.X2, a % b.X3, a % b.X4);

        public static Vector5f operator %(Vector5f a, int b) => new Vector5f(a.X0 % b, a.X1 % b, a.X2 % b, a.X3 % b, a.X4 % b);

        public static Vector5f operator %(int a, Vector5f b) => new Vector5f(a % b.X0, a % b.X1, a % b.X2, a % b.X3, a % b.X4);

        public static bool operator <(Vector5f a, Vector5f b) => a.X0 < b.X0 && a.X1 < b.X1 && a.X2 < b.X2 && a.X3 < b.X3 && a.X4 < b.X4;

        public static bool operator <(Vector5f a, float b) => a.X0 < b && a.X1 < b && a.X2 < b && a.X3 < b && a.X4 < b;

        public static bool operator <(float a, Vector5f b) => a < b.X0 && a < b.X1 && a < b.X2 && a < b.X3 && a < b.X4;

        public static bool operator <(Vector5f a, int b) => a.X0 < b && a.X1 < b && a.X2 < b && a.X3 < b && a.X4 < b;

        public static bool operator <(int a, Vector5f b) => a < b.X0 && a < b.X1 && a < b.X2 && a < b.X3 && a < b.X4;

        public static bool operator >(Vector5f a, Vector5f b) => a.X0 > b.X0 && a.X1 > b.X1 && a.X2 > b.X2 && a.X3 > b.X3 && a.X4 > b.X4;

        public static bool operator >(Vector5f a, float b) => a.X0 > b && a.X1 > b && a.X2 > b && a.X3 > b && a.X4 > b;

        public static bool operator >(float a, Vector5f b) => a > b.X0 && a > b.X1 && a > b.X2 && a > b.X3 && a > b.X4;

        public static bool operator >(Vector5f a, int b) => a.X0 > b && a.X1 > b && a.X2 > b && a.X3 > b && a.X4 > b;

        public static bool operator >(int a, Vector5f b) => a > b.X0 && a > b.X1 && a > b.X2 && a > b.X3 && a > b.X4;

        public static bool operator <=(Vector5f a, Vector5f b) => a.X0 <= b.X0 && a.X1 <= b.X1 && a.X2 <= b.X2 && a.X3 <= b.X3 && a.X4 <= b.X4;

        public static bool operator <=(Vector5f a, float b) => a.X0 <= b && a.X1 <= b && a.X2 <= b && a.X3 <= b && a.X4 <= b;

        public static bool operator <=(float a, Vector5f b) => a <= b.X0 && a <= b.X1 && a <= b.X2 && a <= b.X3 && a <= b.X4;

        public static bool operator <=(Vector5f a, int b) => a.X0 <= b && a.X1 <= b && a.X2 <= b && a.X3 <= b && a.X4 <= b;

        public static bool operator <=(int a, Vector5f b) => a <= b.X0 && a <= b.X1 && a <= b.X2 && a <= b.X3 && a <= b.X4;

        public static bool operator >=(Vector5f a, Vector5f b) => a.X0 >= b.X0 && a.X1 >= b.X1 && a.X2 >= b.X2 && a.X3 >= b.X3 && a.X4 >= b.X4;

        public static bool operator >=(Vector5f a, float b) => a.X0 >= b && a.X1 >= b && a.X2 >= b && a.X3 >= b && a.X4 >= b;

        public static bool operator >=(float a, Vector5f b) => a >= b.X0 && a >= b.X1 && a >= b.X2 && a >= b.X3 && a >= b.X4;

        public static bool operator >=(Vector5f a, int b) => a.X0 >= b && a.X1 >= b && a.X2 >= b && a.X3 >= b && a.X4 >= b;

        public static bool operator >=(int a, Vector5f b) => a >= b.X0 && a >= b.X1 && a >= b.X2 && a >= b.X3 && a >= b.X4;

        public static bool operator ==(Vector5f a, Vector5f b) => a.X0 == b.X0 && a.X1 == b.X1 && a.X2 == b.X2 && a.X3 == b.X3 && a.X4 == b.X4;

        public static bool operator ==(Vector5f a, float b) => a.X0 == b && a.X1 == b && a.X2 == b && a.X3 == b && a.X4 == b;

        public static bool operator ==(float a, Vector5f b) => a == b.X0 && a == b.X1 && a == b.X2 && a == b.X3 && a == b.X4;

        public static bool operator ==(Vector5f a, int b) => a.X0 == b && a.X1 == b && a.X2 == b && a.X3 == b && a.X4 == b;

        public static bool operator ==(int a, Vector5f b) => a == b.X0 && a == b.X1 && a == b.X2 && a == b.X3 && a == b.X4;

        public static bool operator !=(Vector5f a, Vector5f b) => a.X0 != b.X0 || a.X1 != b.X1 || a.X2 != b.X2 || a.X3 != b.X3 || a.X4 != b.X4;

        public static bool operator !=(Vector5f a, float b) => a.X0 != b || a.X1 != b || a.X2 != b || a.X3 != b || a.X4 != b;

        public static bool operator !=(float a, Vector5f b) => a != b.X0 || a != b.X1 || a != b.X2 || a != b.X3 || a != b.X4;

        public static bool operator !=(Vector5f a, int b) => a.X0 != b || a.X1 != b || a.X2 != b || a.X3 != b || a.X4 != b;

        public static bool operator !=(int a, Vector5f b) => a != b.X0 || a != b.X1 || a != b.X2 || a != b.X3 || a != b.X4;

        public bool Equals(Vector5f other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Vector5f))
                return false;

            return Equals((Vector5f) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X0.GetHashCode() * 397) ^ (X1.GetHashCode() * 397) ^ (X2.GetHashCode() * 397) ^ (X3.GetHashCode() * 397) ^ (X4.GetHashCode() * 397);
            }
        }

        public override string ToString() => "(" + X0 + "," + X1 + "," + X2 + "," + X3 + "," + X4 + ")";
    }

    public static class Vector5fUtils
    {
        public static Vector2f X0X0(this Vector5f @this) => new Vector2f(@this.X0, @this.X0);

        public static Vector2f X0X1(this Vector5f @this) => new Vector2f(@this.X0, @this.X1);

        public static Vector2f X0X2(this Vector5f @this) => new Vector2f(@this.X0, @this.X2);

        public static Vector2f X0X3(this Vector5f @this) => new Vector2f(@this.X0, @this.X3);

        public static Vector2f X0X4(this Vector5f @this) => new Vector2f(@this.X0, @this.X4);

        public static Vector2f X1X0(this Vector5f @this) => new Vector2f(@this.X1, @this.X0);

        public static Vector2f X1X1(this Vector5f @this) => new Vector2f(@this.X1, @this.X1);

        public static Vector2f X1X2(this Vector5f @this) => new Vector2f(@this.X1, @this.X2);

        public static Vector2f X1X3(this Vector5f @this) => new Vector2f(@this.X1, @this.X3);

        public static Vector2f X1X4(this Vector5f @this) => new Vector2f(@this.X1, @this.X4);

        public static Vector2f X2X0(this Vector5f @this) => new Vector2f(@this.X2, @this.X0);

        public static Vector2f X2X1(this Vector5f @this) => new Vector2f(@this.X2, @this.X1);

        public static Vector2f X2X2(this Vector5f @this) => new Vector2f(@this.X2, @this.X2);

        public static Vector2f X2X3(this Vector5f @this) => new Vector2f(@this.X2, @this.X3);

        public static Vector2f X2X4(this Vector5f @this) => new Vector2f(@this.X2, @this.X4);

        public static Vector2f X3X0(this Vector5f @this) => new Vector2f(@this.X3, @this.X0);

        public static Vector2f X3X1(this Vector5f @this) => new Vector2f(@this.X3, @this.X1);

        public static Vector2f X3X2(this Vector5f @this) => new Vector2f(@this.X3, @this.X2);

        public static Vector2f X3X3(this Vector5f @this) => new Vector2f(@this.X3, @this.X3);

        public static Vector2f X3X4(this Vector5f @this) => new Vector2f(@this.X3, @this.X4);

        public static Vector2f X4X0(this Vector5f @this) => new Vector2f(@this.X4, @this.X0);

        public static Vector2f X4X1(this Vector5f @this) => new Vector2f(@this.X4, @this.X1);

        public static Vector2f X4X2(this Vector5f @this) => new Vector2f(@this.X4, @this.X2);

        public static Vector2f X4X3(this Vector5f @this) => new Vector2f(@this.X4, @this.X3);

        public static Vector2f X4X4(this Vector5f @this) => new Vector2f(@this.X4, @this.X4);

        public static bool AnyNegative(this Vector5f @this) => @this.X0 < 0 || @this.X1 < 0 || @this.X2 < 0 || @this.X3 < 0 || @this.X4 < 0;

        public static bool AnyNan(this Vector5f @this) => float.IsNaN(@this.X0) || float.IsNaN(@this.X1) || float.IsNaN(@this.X2) || float.IsNaN(@this.X3) || float.IsNaN(@this.X4);

        public static bool NoNan(this Vector5f @this) => !float.IsNaN(@this.X0) && !float.IsNaN(@this.X1) && !float.IsNaN(@this.X2) && !float.IsNaN(@this.X3) && !float.IsNaN(@this.X4);

        public static bool AnyInfinity(this Vector5f @this) => float.IsInfinity(@this.X0) || float.IsInfinity(@this.X1) || float.IsInfinity(@this.X2) || float.IsInfinity(@this.X3) || float.IsInfinity(@this.X4);

        public static float AreaSum(this Vector5f @this) => @this.X0 * @this.X1 * @this.X2 * @this.X3 * @this.X4;

        public static float Magnitude(this Vector5f @this) => (float) Math.Sqrt(@this.SqrMagnitude());

        public static Vector5f Normalized(this Vector5f @this)
        {
            var magnitude = @this.Magnitude();
            return magnitude <= float.Epsilon ? @this : @this / magnitude;
        }

        public static float SqrMagnitude(this Vector5f @this) => @this.X0 * @this.X0 + @this.X1 * @this.X1 + @this.X2 * @this.X2 + @this.X3 * @this.X3 + @this.X4 * @this.X4;

        public static Vector5f FromSame(float value) => new Vector5f(value, value, value, value, value);

        public static float Dot(Vector5f a, Vector5f b) => (float) ((double) a.X0 * b.X0 + (double) a.X1 * b.X1 + (double) a.X2 * b.X2 + (double) a.X3 * b.X3 + (double) a.X4 * b.X4);

        public static Vector5f Ceil(this Vector5f @this) => new Vector5f(@this.X0.CeilToInt(), @this.X1.CeilToInt(), @this.X2.CeilToInt(), @this.X3.CeilToInt(), @this.X4.CeilToInt());

        public static Vector5i CeilToVector5i(this Vector5f @this) => new Vector5i(@this.X0.CeilToInt(), @this.X1.CeilToInt(), @this.X2.CeilToInt(), @this.X3.CeilToInt(), @this.X4.CeilToInt());

        public static Vector5f Abs(this Vector5f @this) => new Vector5f(Math.Abs(@this.X0), Math.Abs(@this.X1), Math.Abs(@this.X2), Math.Abs(@this.X3), Math.Abs(@this.X4));

        public static Vector5f Floor(this Vector5f @this) => new Vector5f(@this.X0.FloorToInt(), @this.X1.FloorToInt(), @this.X2.FloorToInt(), @this.X3.FloorToInt(), @this.X4.FloorToInt());

        public static Vector5i FloorToVector5i(this Vector5f @this) => new Vector5i(@this.X0.FloorToInt(), @this.X1.FloorToInt(), @this.X2.FloorToInt(), @this.X3.FloorToInt(), @this.X4.FloorToInt());

        public static Vector5f Round(this Vector5f @this) => new Vector5f(@this.X0.RoundToInt(), @this.X1.RoundToInt(), @this.X2.RoundToInt(), @this.X3.RoundToInt(), @this.X4.RoundToInt());

        public static Vector5i RoundToVector5i(this Vector5f @this) => new Vector5i(@this.X0.RoundToInt(), @this.X1.RoundToInt(), @this.X2.RoundToInt(), @this.X3.RoundToInt(), @this.X4.RoundToInt());

        public static Vector6f ToVector6f(this Vector5f @this, float x6) => new Vector6f(@this.X0, @this.X1, @this.X2, @this.X3, @this.X4, x6);

        public static Area5f ToArea5f(this Vector5f @this) => Area5f.FromMinAndSize(Vector5f.Zero, @this);

        public static Vector5f Max(Vector5f a, Vector5f b) => new Vector5f(Math.Max(a.X0, b.X0), Math.Max(a.X1, b.X1), Math.Max(a.X2, b.X2), Math.Max(a.X3, b.X3), Math.Max(a.X4, b.X4));

        public static Vector5f Max(Vector5f a, float b) => new Vector5f(Math.Max(a.X0, b), Math.Max(a.X1, b), Math.Max(a.X2, b), Math.Max(a.X3, b), Math.Max(a.X4, b));

        public static Vector5f Max(float a, Vector5f b) => new Vector5f(Math.Max(a, b.X0), Math.Max(a, b.X1), Math.Max(a, b.X2), Math.Max(a, b.X3), Math.Max(a, b.X4));

        public static Vector5f Max(Vector5f a, int b) => new Vector5f(Math.Max(a.X0, b), Math.Max(a.X1, b), Math.Max(a.X2, b), Math.Max(a.X3, b), Math.Max(a.X4, b));

        public static Vector5f Max(int a, Vector5f b) => new Vector5f(Math.Max(a, b.X0), Math.Max(a, b.X1), Math.Max(a, b.X2), Math.Max(a, b.X3), Math.Max(a, b.X4));

        public static Vector5f Min(Vector5f a, Vector5f b) => new Vector5f(Math.Min(a.X0, b.X0), Math.Min(a.X1, b.X1), Math.Min(a.X2, b.X2), Math.Min(a.X3, b.X3), Math.Min(a.X4, b.X4));

        public static Vector5f Min(Vector5f a, float b) => new Vector5f(Math.Min(a.X0, b), Math.Min(a.X1, b), Math.Min(a.X2, b), Math.Min(a.X3, b), Math.Min(a.X4, b));

        public static Vector5f Min(float a, Vector5f b) => new Vector5f(Math.Min(a, b.X0), Math.Min(a, b.X1), Math.Min(a, b.X2), Math.Min(a, b.X3), Math.Min(a, b.X4));

        public static Vector5f Min(Vector5f a, int b) => new Vector5f(Math.Min(a.X0, b), Math.Min(a.X1, b), Math.Min(a.X2, b), Math.Min(a.X3, b), Math.Min(a.X4, b));

        public static Vector5f Min(int a, Vector5f b) => new Vector5f(Math.Min(a, b.X0), Math.Min(a, b.X1), Math.Min(a, b.X2), Math.Min(a, b.X3), Math.Min(a, b.X4));

        public static bool IsApproximatelyEqual(Vector5f a, Vector5f b) => a.X0.IsApproximatelyEqual(b.X0) && a.X1.IsApproximatelyEqual(b.X1) && a.X2.IsApproximatelyEqual(b.X2) && a.X3.IsApproximatelyEqual(b.X3) && a.X4.IsApproximatelyEqual(b.X4);

        public static bool IsApproximatelyEqual(Vector5f a, float b) => a.X0.IsApproximatelyEqual(b) && a.X1.IsApproximatelyEqual(b) && a.X2.IsApproximatelyEqual(b) && a.X3.IsApproximatelyEqual(b) && a.X4.IsApproximatelyEqual(b);

        public static bool IsApproximatelyEqual(float a, Vector5f b) => a.IsApproximatelyEqual(b.X0) && a.IsApproximatelyEqual(b.X1) && a.IsApproximatelyEqual(b.X2) && a.IsApproximatelyEqual(b.X3) && a.IsApproximatelyEqual(b.X4);

        public static bool IsApproximatelyEqual(Vector5f a, int b) => a.X0.IsApproximatelyEqual(b) && a.X1.IsApproximatelyEqual(b) && a.X2.IsApproximatelyEqual(b) && a.X3.IsApproximatelyEqual(b) && a.X4.IsApproximatelyEqual(b);

        public static bool IsApproximatelyEqual(int a, Vector5f b) => MathUtils.IsApproximatelyEqual(a, b.X0) && MathUtils.IsApproximatelyEqual(a, b.X1) && MathUtils.IsApproximatelyEqual(a, b.X2) && MathUtils.IsApproximatelyEqual(a, b.X3) && MathUtils.IsApproximatelyEqual(a, b.X4);
    }
}