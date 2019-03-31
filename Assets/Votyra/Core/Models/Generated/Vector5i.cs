using System;
using Newtonsoft.Json;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Vector5i : IEquatable<Vector5i>
    {
        public static readonly Vector5i Zero = new Vector5i(0, 0, 0, 0, 0);

        public static readonly Vector5i One = new Vector5i(1, 1, 1, 1, 1);

        public readonly int X0;

        public readonly int X1;

        public readonly int X2;

        public readonly int X3;

        public readonly int X4;

        [JsonConstructor]
        public Vector5i(int x0, int x1, int x2, int x3, int x4)
        {
            X0 = x0;

            X1 = x1;

            X2 = x2;

            X3 = x3;

            X4 = x4;
        }

        public static Vector5i operator -(Vector5i a) => new Vector5i(-a.X0, -a.X1, -a.X2, -a.X3, -a.X4);

        public static Vector5i operator *(Vector5i a, Vector5i b) => new Vector5i(a.X0 * b.X0, a.X1 * b.X1, a.X2 * b.X2, a.X3 * b.X3, a.X4 * b.X4);

        public static Vector5i operator *(Vector5i a, int b) => new Vector5i(a.X0 * b, a.X1 * b, a.X2 * b, a.X3 * b, a.X4 * b);
        public static Vector5i operator *(int a, Vector5i b) => new Vector5i(a * b.X0, a * b.X1, a * b.X2, a * b.X3, a * b.X4);

        public static Vector5i operator +(Vector5i a, Vector5i b) => new Vector5i(a.X0 + b.X0, a.X1 + b.X1, a.X2 + b.X2, a.X3 + b.X3, a.X4 + b.X4);

        public static Vector5i operator +(Vector5i a, int b) => new Vector5i(a.X0 + b, a.X1 + b, a.X2 + b, a.X3 + b, a.X4 + b);
        public static Vector5i operator +(int a, Vector5i b) => new Vector5i(a + b.X0, a + b.X1, a + b.X2, a + b.X3, a + b.X4);

        public static Vector5i operator -(Vector5i a, Vector5i b) => new Vector5i(a.X0 - b.X0, a.X1 - b.X1, a.X2 - b.X2, a.X3 - b.X3, a.X4 - b.X4);

        public static Vector5i operator -(Vector5i a, int b) => new Vector5i(a.X0 - b, a.X1 - b, a.X2 - b, a.X3 - b, a.X4 - b);
        public static Vector5i operator -(int a, Vector5i b) => new Vector5i(a - b.X0, a - b.X1, a - b.X2, a - b.X3, a - b.X4);

        public static Vector5i operator /(Vector5i a, Vector5i b) => new Vector5i(a.X0 / b.X0, a.X1 / b.X1, a.X2 / b.X2, a.X3 / b.X3, a.X4 / b.X4);

        public static Vector5i operator /(Vector5i a, int b) => new Vector5i(a.X0 / b, a.X1 / b, a.X2 / b, a.X3 / b, a.X4 / b);
        public static Vector5i operator /(int a, Vector5i b) => new Vector5i(a / b.X0, a / b.X1, a / b.X2, a / b.X3, a / b.X4);

        public static Vector5i operator %(Vector5i a, Vector5i b) => new Vector5i(a.X0 % b.X0, a.X1 % b.X1, a.X2 % b.X2, a.X3 % b.X3, a.X4 % b.X4);

        public static Vector5i operator %(Vector5i a, int b) => new Vector5i(a.X0 % b, a.X1 % b, a.X2 % b, a.X3 % b, a.X4 % b);
        public static Vector5i operator %(int a, Vector5i b) => new Vector5i(a % b.X0, a % b.X1, a % b.X2, a % b.X3, a % b.X4);

        public static bool operator <(Vector5i a, Vector5i b) => a.X0 < b.X0 && a.X1 < b.X1 && a.X2 < b.X2 && a.X3 < b.X3 && a.X4 < b.X4;

        public static bool operator <(Vector5i a, int b) => a.X0 < b && a.X1 < b && a.X2 < b && a.X3 < b && a.X4 < b;
        public static bool operator <(int a, Vector5i b) => a < b.X0 && a < b.X1 && a < b.X2 && a < b.X3 && a < b.X4;

        public static bool operator >(Vector5i a, Vector5i b) => a.X0 > b.X0 && a.X1 > b.X1 && a.X2 > b.X2 && a.X3 > b.X3 && a.X4 > b.X4;

        public static bool operator >(Vector5i a, int b) => a.X0 > b && a.X1 > b && a.X2 > b && a.X3 > b && a.X4 > b;
        public static bool operator >(int a, Vector5i b) => a > b.X0 && a > b.X1 && a > b.X2 && a > b.X3 && a > b.X4;

        public static bool operator <=(Vector5i a, Vector5i b) => a.X0 <= b.X0 && a.X1 <= b.X1 && a.X2 <= b.X2 && a.X3 <= b.X3 && a.X4 <= b.X4;

        public static bool operator <=(Vector5i a, int b) => a.X0 <= b && a.X1 <= b && a.X2 <= b && a.X3 <= b && a.X4 <= b;
        public static bool operator <=(int a, Vector5i b) => a <= b.X0 && a <= b.X1 && a <= b.X2 && a <= b.X3 && a <= b.X4;

        public static bool operator >=(Vector5i a, Vector5i b) => a.X0 >= b.X0 && a.X1 >= b.X1 && a.X2 >= b.X2 && a.X3 >= b.X3 && a.X4 >= b.X4;

        public static bool operator >=(Vector5i a, int b) => a.X0 >= b && a.X1 >= b && a.X2 >= b && a.X3 >= b && a.X4 >= b;
        public static bool operator >=(int a, Vector5i b) => a >= b.X0 && a >= b.X1 && a >= b.X2 && a >= b.X3 && a >= b.X4;

        public static bool operator ==(Vector5i a, Vector5i b) => a.X0 == b.X0 && a.X1 == b.X1 && a.X2 == b.X2 && a.X3 == b.X3 && a.X4 == b.X4;

        public static bool operator ==(Vector5i a, int b) => a.X0 == b && a.X1 == b && a.X2 == b && a.X3 == b && a.X4 == b;
        public static bool operator ==(int a, Vector5i b) => a == b.X0 && a == b.X1 && a == b.X2 && a == b.X3 && a == b.X4;

        public static bool operator !=(Vector5i a, Vector5i b) => a.X0 != b.X0 || a.X1 != b.X1 || a.X2 != b.X2 || a.X3 != b.X3 || a.X4 != b.X4;

        public static bool operator !=(Vector5i a, int b) => a.X0 != b || a.X1 != b || a.X2 != b || a.X3 != b || a.X4 != b;
        public static bool operator !=(int a, Vector5i b) => a != b.X0 || a != b.X1 || a != b.X2 || a != b.X3 || a != b.X4;

        public static Vector5f operator *(Vector5i a, Vector5f b) => new Vector5f(a.X0 * b.X0, a.X1 * b.X1, a.X2 * b.X2, a.X3 * b.X3, a.X4 * b.X4);
        public static Vector5f operator *(Vector5f a, Vector5i b) => new Vector5f(a.X0 * b.X0, a.X1 * b.X1, a.X2 * b.X2, a.X3 * b.X3, a.X4 * b.X4);
        public static Vector5f operator *(Vector5i a, float b) => new Vector5f(a.X0 * b, a.X1 * b, a.X2 * b, a.X3 * b, a.X4 * b);
        public static Vector5f operator *(float a, Vector5i b) => new Vector5f(a * b.X0, a * b.X1, a * b.X2, a * b.X3, a * b.X4);

        public static Vector5f operator +(Vector5i a, Vector5f b) => new Vector5f(a.X0 + b.X0, a.X1 + b.X1, a.X2 + b.X2, a.X3 + b.X3, a.X4 + b.X4);
        public static Vector5f operator +(Vector5f a, Vector5i b) => new Vector5f(a.X0 + b.X0, a.X1 + b.X1, a.X2 + b.X2, a.X3 + b.X3, a.X4 + b.X4);
        public static Vector5f operator +(Vector5i a, float b) => new Vector5f(a.X0 + b, a.X1 + b, a.X2 + b, a.X3 + b, a.X4 + b);
        public static Vector5f operator +(float a, Vector5i b) => new Vector5f(a + b.X0, a + b.X1, a + b.X2, a + b.X3, a + b.X4);

        public static Vector5f operator -(Vector5i a, Vector5f b) => new Vector5f(a.X0 - b.X0, a.X1 - b.X1, a.X2 - b.X2, a.X3 - b.X3, a.X4 - b.X4);
        public static Vector5f operator -(Vector5f a, Vector5i b) => new Vector5f(a.X0 - b.X0, a.X1 - b.X1, a.X2 - b.X2, a.X3 - b.X3, a.X4 - b.X4);
        public static Vector5f operator -(Vector5i a, float b) => new Vector5f(a.X0 - b, a.X1 - b, a.X2 - b, a.X3 - b, a.X4 - b);
        public static Vector5f operator -(float a, Vector5i b) => new Vector5f(a - b.X0, a - b.X1, a - b.X2, a - b.X3, a - b.X4);

        public static Vector5f operator /(Vector5i a, Vector5f b) => new Vector5f(a.X0 / b.X0, a.X1 / b.X1, a.X2 / b.X2, a.X3 / b.X3, a.X4 / b.X4);
        public static Vector5f operator /(Vector5f a, Vector5i b) => new Vector5f(a.X0 / b.X0, a.X1 / b.X1, a.X2 / b.X2, a.X3 / b.X3, a.X4 / b.X4);
        public static Vector5f operator /(Vector5i a, float b) => new Vector5f(a.X0 / b, a.X1 / b, a.X2 / b, a.X3 / b, a.X4 / b);
        public static Vector5f operator /(float a, Vector5i b) => new Vector5f(a / b.X0, a / b.X1, a / b.X2, a / b.X3, a / b.X4);

        public static Vector5f operator %(Vector5i a, Vector5f b) => new Vector5f(a.X0 % b.X0, a.X1 % b.X1, a.X2 % b.X2, a.X3 % b.X3, a.X4 % b.X4);
        public static Vector5f operator %(Vector5f a, Vector5i b) => new Vector5f(a.X0 % b.X0, a.X1 % b.X1, a.X2 % b.X2, a.X3 % b.X3, a.X4 % b.X4);
        public static Vector5f operator %(Vector5i a, float b) => new Vector5f(a.X0 % b, a.X1 % b, a.X2 % b, a.X3 % b, a.X4 % b);
        public static Vector5f operator %(float a, Vector5i b) => new Vector5f(a % b.X0, a % b.X1, a % b.X2, a % b.X3, a % b.X4);

        public bool Equals(Vector5i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Vector5i))
                return false;

            return Equals((Vector5i) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X0 * 397) ^ (X1 * 397) ^ (X2 * 397) ^ (X3 * 397) ^ (X4 * 397);
            }
        }

        public override string ToString() => "(" + X0 + "," + X1 + "," + X2 + "," + X3 + "," + X4 + ")";
    }

    public static class Vector5iUtils
    {
        public static readonly Vector5i PlusOneX0 = new Vector5i(1, 0, 0, 0, 0);
        public static readonly Vector5i MinusOneX0 = new Vector5i(-1, 0, 0, 0, 0);

        public static readonly Vector5i PlusOneX1 = new Vector5i(0, 1, 0, 0, 0);
        public static readonly Vector5i MinusOneX1 = new Vector5i(0, -1, 0, 0, 0);

        public static readonly Vector5i PlusOneX2 = new Vector5i(0, 0, 1, 0, 0);
        public static readonly Vector5i MinusOneX2 = new Vector5i(0, 0, -1, 0, 0);

        public static readonly Vector5i PlusOneX3 = new Vector5i(0, 0, 0, 1, 0);
        public static readonly Vector5i MinusOneX3 = new Vector5i(0, 0, 0, -1, 0);

        public static readonly Vector5i PlusOneX4 = new Vector5i(0, 0, 0, 0, 1);
        public static readonly Vector5i MinusOneX4 = new Vector5i(0, 0, 0, 0, -1);
        public static float Magnitude(this Vector5i @this) => (float) Math.Sqrt(@this.SqrMagnitude());

        public static Vector5f Normalized(this Vector5i @this)
        {
            var magnitude = @this.Magnitude();
            return magnitude <= float.Epsilon ? @this.ToVector5f() : @this / magnitude;
        }

        public static float SqrMagnitude(this Vector5i @this) => @this.X0 * @this.X0 + @this.X1 * @this.X1 + @this.X2 * @this.X2 + @this.X3 * @this.X3 + @this.X4 * @this.X4;

        public static int ManhattanMagnitude(this Vector5i @this) => @this.X0 + @this.X1 + @this.X2 + @this.X3 + @this.X4;

        public static int AreaSum(this Vector5i @this) => @this.X0 * @this.X1 * @this.X2 * @this.X3 * @this.X4;

        public static int Volume(this Vector5i @this) => @this.X0 * @this.X1 * @this.X2 * @this.X3 * @this.X4;

        public static Vector2i X0X0(this Vector5i @this) => new Vector2i(@this.X0, @this.X0);

        public static Vector2i X0X1(this Vector5i @this) => new Vector2i(@this.X0, @this.X1);

        public static Vector2i X0X2(this Vector5i @this) => new Vector2i(@this.X0, @this.X2);

        public static Vector2i X0X3(this Vector5i @this) => new Vector2i(@this.X0, @this.X3);

        public static Vector2i X0X4(this Vector5i @this) => new Vector2i(@this.X0, @this.X4);

        public static Vector2i X1X0(this Vector5i @this) => new Vector2i(@this.X1, @this.X0);

        public static Vector2i X1X1(this Vector5i @this) => new Vector2i(@this.X1, @this.X1);

        public static Vector2i X1X2(this Vector5i @this) => new Vector2i(@this.X1, @this.X2);

        public static Vector2i X1X3(this Vector5i @this) => new Vector2i(@this.X1, @this.X3);

        public static Vector2i X1X4(this Vector5i @this) => new Vector2i(@this.X1, @this.X4);

        public static Vector2i X2X0(this Vector5i @this) => new Vector2i(@this.X2, @this.X0);

        public static Vector2i X2X1(this Vector5i @this) => new Vector2i(@this.X2, @this.X1);

        public static Vector2i X2X2(this Vector5i @this) => new Vector2i(@this.X2, @this.X2);

        public static Vector2i X2X3(this Vector5i @this) => new Vector2i(@this.X2, @this.X3);

        public static Vector2i X2X4(this Vector5i @this) => new Vector2i(@this.X2, @this.X4);

        public static Vector2i X3X0(this Vector5i @this) => new Vector2i(@this.X3, @this.X0);

        public static Vector2i X3X1(this Vector5i @this) => new Vector2i(@this.X3, @this.X1);

        public static Vector2i X3X2(this Vector5i @this) => new Vector2i(@this.X3, @this.X2);

        public static Vector2i X3X3(this Vector5i @this) => new Vector2i(@this.X3, @this.X3);

        public static Vector2i X3X4(this Vector5i @this) => new Vector2i(@this.X3, @this.X4);

        public static Vector2i X4X0(this Vector5i @this) => new Vector2i(@this.X4, @this.X0);

        public static Vector2i X4X1(this Vector5i @this) => new Vector2i(@this.X4, @this.X1);

        public static Vector2i X4X2(this Vector5i @this) => new Vector2i(@this.X4, @this.X2);

        public static Vector2i X4X3(this Vector5i @this) => new Vector2i(@this.X4, @this.X3);

        public static Vector2i X4X4(this Vector5i @this) => new Vector2i(@this.X4, @this.X4);

        public static bool AllPositive(this Vector5i @this) => @this.X0 > 0 && @this.X1 > 0 && @this.X2 > 0 && @this.X3 > 0 && @this.X4 > 0;

        public static bool AllZeroOrPositive(this Vector5i @this) => @this.X0 >= 0 && @this.X1 >= 0 && @this.X2 >= 0 && @this.X3 >= 0 && @this.X4 >= 0;

        public static bool AnyNegative(this Vector5i @this) => @this.X0 < 0 || @this.X1 < 0 || @this.X2 < 0 || @this.X3 < 0 || @this.X4 < 0;

        public static bool AnyZero(this Vector5i @this) => @this.X0 == 0 || @this.X1 == 0 || @this.X2 == 0 || @this.X3 == 0 || @this.X4 == 0;

        public static bool AnyZeroOrNegative(this Vector5i @this) => @this.X0 <= 0 || @this.X1 <= 0 || @this.X2 <= 0 || @this.X3 <= 0 || @this.X4 <= 0;

        public static Vector5i FromSame(int value) => new Vector5i(value, value, value, value, value);

        public static Vector5f ToVector5f(this Vector5i @this) => new Vector5f(@this.X0, @this.X1, @this.X2, @this.X3, @this.X4);

        public static Range5i ToRange5i(this Vector5i @this) => Range5i.FromMinAndSize(Vector5i.Zero, @this);
        public static Area5i ToArea5i(this Vector5i @this) => Area5i.FromMinAndSize(Vector5i.Zero, @this);

        public static Vector6i ToVector6i(this Vector5i @this, int x6) => new Vector6i(@this.X0, @this.X1, @this.X2, @this.X3, @this.X4, x6);
        public static Vector6f ToVector6f(this Vector5i @this, float x6) => new Vector6f(@this.X0, @this.X1, @this.X2, @this.X3, @this.X4, x6);

        public static Vector5i Max(Vector5i a, Vector5i b) => new Vector5i(Math.Max(a.X0, b.X0), Math.Max(a.X1, b.X1), Math.Max(a.X2, b.X2), Math.Max(a.X3, b.X3), Math.Max(a.X4, b.X4));

        public static Vector5i Max(Vector5i a, int b) => new Vector5i(Math.Max(a.X0, b), Math.Max(a.X1, b), Math.Max(a.X2, b), Math.Max(a.X3, b), Math.Max(a.X4, b));
        public static Vector5i Max(int a, Vector5i b) => new Vector5i(Math.Max(a, b.X0), Math.Max(a, b.X1), Math.Max(a, b.X2), Math.Max(a, b.X3), Math.Max(a, b.X4));

        public static Vector5i Min(Vector5i a, Vector5i b) => new Vector5i(Math.Min(a.X0, b.X0), Math.Min(a.X1, b.X1), Math.Min(a.X2, b.X2), Math.Min(a.X3, b.X3), Math.Min(a.X4, b.X4));

        public static Vector5i Min(Vector5i a, int b) => new Vector5i(Math.Min(a.X0, b), Math.Min(a.X1, b), Math.Min(a.X2, b), Math.Min(a.X3, b), Math.Min(a.X4, b));
        public static Vector5i Min(int a, Vector5i b) => new Vector5i(Math.Min(a, b.X0), Math.Min(a, b.X1), Math.Min(a, b.X2), Math.Min(a, b.X3), Math.Min(a, b.X4));

        public static Vector5i DivideUp(Vector5i a, Vector5i b) => new Vector5i(a.X0.DivideUp(b.X0), a.X1.DivideUp(b.X1), a.X2.DivideUp(b.X2), a.X3.DivideUp(b.X3), a.X4.DivideUp(b.X4));

        public static Vector5i DivideUp(Vector5i a, int b) => new Vector5i(a.X0.DivideUp(b), a.X1.DivideUp(b), a.X2.DivideUp(b), a.X3.DivideUp(b), a.X4.DivideUp(b));
        public static Vector5i DivideUp(int a, Vector5i b) => new Vector5i(a.DivideUp(b.X0), a.DivideUp(b.X1), a.DivideUp(b.X2), a.DivideUp(b.X3), a.DivideUp(b.X4));
    }
}