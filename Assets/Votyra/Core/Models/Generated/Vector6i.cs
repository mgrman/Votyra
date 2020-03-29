using System;
using Newtonsoft.Json;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Vector6i : IEquatable<Vector6i>
    {
        public static readonly Vector6i Zero = new Vector6i(0, 0, 0, 0, 0, 0);

        public static readonly Vector6i One = new Vector6i(1, 1, 1, 1, 1, 1);

        public readonly int X0;

        public readonly int X1;

        public readonly int X2;

        public readonly int X3;

        public readonly int X4;

        public readonly int X5;

        [JsonConstructor]
        public Vector6i(int x0, int x1, int x2, int x3, int x4, int x5)
        {
            X0 = x0;

            X1 = x1;

            X2 = x2;

            X3 = x3;

            X4 = x4;

            X5 = x5;
        }

        public static Vector6i operator -(Vector6i a) => new Vector6i(-a.X0, -a.X1, -a.X2, -a.X3, -a.X4, -a.X5);

        public static Vector6i operator *(Vector6i a, Vector6i b) => new Vector6i(a.X0 * b.X0, a.X1 * b.X1, a.X2 * b.X2, a.X3 * b.X3, a.X4 * b.X4, a.X5 * b.X5);

        public static Vector6i operator *(Vector6i a, int b) => new Vector6i(a.X0 * b, a.X1 * b, a.X2 * b, a.X3 * b, a.X4 * b, a.X5 * b);

        public static Vector6i operator *(int a, Vector6i b) => new Vector6i(a * b.X0, a * b.X1, a * b.X2, a * b.X3, a * b.X4, a * b.X5);

        public static Vector6i operator +(Vector6i a, Vector6i b) => new Vector6i(a.X0 + b.X0, a.X1 + b.X1, a.X2 + b.X2, a.X3 + b.X3, a.X4 + b.X4, a.X5 + b.X5);

        public static Vector6i operator +(Vector6i a, int b) => new Vector6i(a.X0 + b, a.X1 + b, a.X2 + b, a.X3 + b, a.X4 + b, a.X5 + b);

        public static Vector6i operator +(int a, Vector6i b) => new Vector6i(a + b.X0, a + b.X1, a + b.X2, a + b.X3, a + b.X4, a + b.X5);

        public static Vector6i operator -(Vector6i a, Vector6i b) => new Vector6i(a.X0 - b.X0, a.X1 - b.X1, a.X2 - b.X2, a.X3 - b.X3, a.X4 - b.X4, a.X5 - b.X5);

        public static Vector6i operator -(Vector6i a, int b) => new Vector6i(a.X0 - b, a.X1 - b, a.X2 - b, a.X3 - b, a.X4 - b, a.X5 - b);

        public static Vector6i operator -(int a, Vector6i b) => new Vector6i(a - b.X0, a - b.X1, a - b.X2, a - b.X3, a - b.X4, a - b.X5);

        public static Vector6i operator /(Vector6i a, Vector6i b) => new Vector6i(a.X0 / b.X0, a.X1 / b.X1, a.X2 / b.X2, a.X3 / b.X3, a.X4 / b.X4, a.X5 / b.X5);

        public static Vector6i operator /(Vector6i a, int b) => new Vector6i(a.X0 / b, a.X1 / b, a.X2 / b, a.X3 / b, a.X4 / b, a.X5 / b);

        public static Vector6i operator /(int a, Vector6i b) => new Vector6i(a / b.X0, a / b.X1, a / b.X2, a / b.X3, a / b.X4, a / b.X5);

        public static Vector6i operator %(Vector6i a, Vector6i b) => new Vector6i(a.X0 % b.X0, a.X1 % b.X1, a.X2 % b.X2, a.X3 % b.X3, a.X4 % b.X4, a.X5 % b.X5);

        public static Vector6i operator %(Vector6i a, int b) => new Vector6i(a.X0 % b, a.X1 % b, a.X2 % b, a.X3 % b, a.X4 % b, a.X5 % b);

        public static Vector6i operator %(int a, Vector6i b) => new Vector6i(a % b.X0, a % b.X1, a % b.X2, a % b.X3, a % b.X4, a % b.X5);

        public static bool operator <(Vector6i a, Vector6i b) => a.X0 < b.X0 && a.X1 < b.X1 && a.X2 < b.X2 && a.X3 < b.X3 && a.X4 < b.X4 && a.X5 < b.X5;

        public static bool operator <(Vector6i a, int b) => a.X0 < b && a.X1 < b && a.X2 < b && a.X3 < b && a.X4 < b && a.X5 < b;

        public static bool operator <(int a, Vector6i b) => a < b.X0 && a < b.X1 && a < b.X2 && a < b.X3 && a < b.X4 && a < b.X5;

        public static bool operator >(Vector6i a, Vector6i b) => a.X0 > b.X0 && a.X1 > b.X1 && a.X2 > b.X2 && a.X3 > b.X3 && a.X4 > b.X4 && a.X5 > b.X5;

        public static bool operator >(Vector6i a, int b) => a.X0 > b && a.X1 > b && a.X2 > b && a.X3 > b && a.X4 > b && a.X5 > b;

        public static bool operator >(int a, Vector6i b) => a > b.X0 && a > b.X1 && a > b.X2 && a > b.X3 && a > b.X4 && a > b.X5;

        public static bool operator <=(Vector6i a, Vector6i b) => a.X0 <= b.X0 && a.X1 <= b.X1 && a.X2 <= b.X2 && a.X3 <= b.X3 && a.X4 <= b.X4 && a.X5 <= b.X5;

        public static bool operator <=(Vector6i a, int b) => a.X0 <= b && a.X1 <= b && a.X2 <= b && a.X3 <= b && a.X4 <= b && a.X5 <= b;

        public static bool operator <=(int a, Vector6i b) => a <= b.X0 && a <= b.X1 && a <= b.X2 && a <= b.X3 && a <= b.X4 && a <= b.X5;

        public static bool operator >=(Vector6i a, Vector6i b) => a.X0 >= b.X0 && a.X1 >= b.X1 && a.X2 >= b.X2 && a.X3 >= b.X3 && a.X4 >= b.X4 && a.X5 >= b.X5;

        public static bool operator >=(Vector6i a, int b) => a.X0 >= b && a.X1 >= b && a.X2 >= b && a.X3 >= b && a.X4 >= b && a.X5 >= b;

        public static bool operator >=(int a, Vector6i b) => a >= b.X0 && a >= b.X1 && a >= b.X2 && a >= b.X3 && a >= b.X4 && a >= b.X5;

        public static bool operator ==(Vector6i a, Vector6i b) => a.X0 == b.X0 && a.X1 == b.X1 && a.X2 == b.X2 && a.X3 == b.X3 && a.X4 == b.X4 && a.X5 == b.X5;

        public static bool operator ==(Vector6i a, int b) => a.X0 == b && a.X1 == b && a.X2 == b && a.X3 == b && a.X4 == b && a.X5 == b;

        public static bool operator ==(int a, Vector6i b) => a == b.X0 && a == b.X1 && a == b.X2 && a == b.X3 && a == b.X4 && a == b.X5;

        public static bool operator !=(Vector6i a, Vector6i b) => a.X0 != b.X0 || a.X1 != b.X1 || a.X2 != b.X2 || a.X3 != b.X3 || a.X4 != b.X4 || a.X5 != b.X5;

        public static bool operator !=(Vector6i a, int b) => a.X0 != b || a.X1 != b || a.X2 != b || a.X3 != b || a.X4 != b || a.X5 != b;

        public static bool operator !=(int a, Vector6i b) => a != b.X0 || a != b.X1 || a != b.X2 || a != b.X3 || a != b.X4 || a != b.X5;

        public static Vector6f operator *(Vector6i a, Vector6f b) => new Vector6f(a.X0 * b.X0, a.X1 * b.X1, a.X2 * b.X2, a.X3 * b.X3, a.X4 * b.X4, a.X5 * b.X5);

        public static Vector6f operator *(Vector6f a, Vector6i b) => new Vector6f(a.X0 * b.X0, a.X1 * b.X1, a.X2 * b.X2, a.X3 * b.X3, a.X4 * b.X4, a.X5 * b.X5);

        public static Vector6f operator *(Vector6i a, float b) => new Vector6f(a.X0 * b, a.X1 * b, a.X2 * b, a.X3 * b, a.X4 * b, a.X5 * b);

        public static Vector6f operator *(float a, Vector6i b) => new Vector6f(a * b.X0, a * b.X1, a * b.X2, a * b.X3, a * b.X4, a * b.X5);

        public static Vector6f operator +(Vector6i a, Vector6f b) => new Vector6f(a.X0 + b.X0, a.X1 + b.X1, a.X2 + b.X2, a.X3 + b.X3, a.X4 + b.X4, a.X5 + b.X5);

        public static Vector6f operator +(Vector6f a, Vector6i b) => new Vector6f(a.X0 + b.X0, a.X1 + b.X1, a.X2 + b.X2, a.X3 + b.X3, a.X4 + b.X4, a.X5 + b.X5);

        public static Vector6f operator +(Vector6i a, float b) => new Vector6f(a.X0 + b, a.X1 + b, a.X2 + b, a.X3 + b, a.X4 + b, a.X5 + b);

        public static Vector6f operator +(float a, Vector6i b) => new Vector6f(a + b.X0, a + b.X1, a + b.X2, a + b.X3, a + b.X4, a + b.X5);

        public static Vector6f operator -(Vector6i a, Vector6f b) => new Vector6f(a.X0 - b.X0, a.X1 - b.X1, a.X2 - b.X2, a.X3 - b.X3, a.X4 - b.X4, a.X5 - b.X5);

        public static Vector6f operator -(Vector6f a, Vector6i b) => new Vector6f(a.X0 - b.X0, a.X1 - b.X1, a.X2 - b.X2, a.X3 - b.X3, a.X4 - b.X4, a.X5 - b.X5);

        public static Vector6f operator -(Vector6i a, float b) => new Vector6f(a.X0 - b, a.X1 - b, a.X2 - b, a.X3 - b, a.X4 - b, a.X5 - b);

        public static Vector6f operator -(float a, Vector6i b) => new Vector6f(a - b.X0, a - b.X1, a - b.X2, a - b.X3, a - b.X4, a - b.X5);

        public static Vector6f operator /(Vector6i a, Vector6f b) => new Vector6f(a.X0 / b.X0, a.X1 / b.X1, a.X2 / b.X2, a.X3 / b.X3, a.X4 / b.X4, a.X5 / b.X5);

        public static Vector6f operator /(Vector6f a, Vector6i b) => new Vector6f(a.X0 / b.X0, a.X1 / b.X1, a.X2 / b.X2, a.X3 / b.X3, a.X4 / b.X4, a.X5 / b.X5);

        public static Vector6f operator /(Vector6i a, float b) => new Vector6f(a.X0 / b, a.X1 / b, a.X2 / b, a.X3 / b, a.X4 / b, a.X5 / b);

        public static Vector6f operator /(float a, Vector6i b) => new Vector6f(a / b.X0, a / b.X1, a / b.X2, a / b.X3, a / b.X4, a / b.X5);

        public static Vector6f operator %(Vector6i a, Vector6f b) => new Vector6f(a.X0 % b.X0, a.X1 % b.X1, a.X2 % b.X2, a.X3 % b.X3, a.X4 % b.X4, a.X5 % b.X5);

        public static Vector6f operator %(Vector6f a, Vector6i b) => new Vector6f(a.X0 % b.X0, a.X1 % b.X1, a.X2 % b.X2, a.X3 % b.X3, a.X4 % b.X4, a.X5 % b.X5);

        public static Vector6f operator %(Vector6i a, float b) => new Vector6f(a.X0 % b, a.X1 % b, a.X2 % b, a.X3 % b, a.X4 % b, a.X5 % b);

        public static Vector6f operator %(float a, Vector6i b) => new Vector6f(a % b.X0, a % b.X1, a % b.X2, a % b.X3, a % b.X4, a % b.X5);

        public bool Equals(Vector6i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Vector6i))
            {
                return false;
            }

            return Equals((Vector6i) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X0 * 397) ^ (X1 * 397) ^ (X2 * 397) ^ (X3 * 397) ^ (X4 * 397) ^ (X5 * 397);
            }
        }

        public override string ToString() => "(" + X0 + "," + X1 + "," + X2 + "," + X3 + "," + X4 + "," + X5 + ")";
    }

    public static class Vector6iUtils
    {
        public static readonly Vector6i PlusOneX0 = new Vector6i(1, 0, 0, 0, 0, 0);
        public static readonly Vector6i MinusOneX0 = new Vector6i(-1, 0, 0, 0, 0, 0);

        public static readonly Vector6i PlusOneX1 = new Vector6i(0, 1, 0, 0, 0, 0);
        public static readonly Vector6i MinusOneX1 = new Vector6i(0, -1, 0, 0, 0, 0);

        public static readonly Vector6i PlusOneX2 = new Vector6i(0, 0, 1, 0, 0, 0);
        public static readonly Vector6i MinusOneX2 = new Vector6i(0, 0, -1, 0, 0, 0);

        public static readonly Vector6i PlusOneX3 = new Vector6i(0, 0, 0, 1, 0, 0);
        public static readonly Vector6i MinusOneX3 = new Vector6i(0, 0, 0, -1, 0, 0);

        public static readonly Vector6i PlusOneX4 = new Vector6i(0, 0, 0, 0, 1, 0);
        public static readonly Vector6i MinusOneX4 = new Vector6i(0, 0, 0, 0, -1, 0);

        public static readonly Vector6i PlusOneX5 = new Vector6i(0, 0, 0, 0, 0, 1);
        public static readonly Vector6i MinusOneX5 = new Vector6i(0, 0, 0, 0, 0, -1);

        public static float Magnitude(this Vector6i @this) => (float) Math.Sqrt(@this.SqrMagnitude());

        public static Vector6f Normalized(this Vector6i @this)
        {
            var magnitude = @this.Magnitude();
            return magnitude <= float.Epsilon ? @this.ToVector6f() : @this / magnitude;
        }

        public static float SqrMagnitude(this Vector6i @this) => @this.X0 * @this.X0 + @this.X1 * @this.X1 + @this.X2 * @this.X2 + @this.X3 * @this.X3 + @this.X4 * @this.X4 + @this.X5 * @this.X5;

        public static int ManhattanMagnitude(this Vector6i @this) => @this.X0 + @this.X1 + @this.X2 + @this.X3 + @this.X4 + @this.X5;

        public static int AreaSum(this Vector6i @this) => @this.X0 * @this.X1 * @this.X2 * @this.X3 * @this.X4 * @this.X5;

        public static int Volume(this Vector6i @this) => @this.X0 * @this.X1 * @this.X2 * @this.X3 * @this.X4 * @this.X5;

        public static Vector2i X0X0(this Vector6i @this) => new Vector2i(@this.X0, @this.X0);

        public static Vector2i X0X1(this Vector6i @this) => new Vector2i(@this.X0, @this.X1);

        public static Vector2i X0X2(this Vector6i @this) => new Vector2i(@this.X0, @this.X2);

        public static Vector2i X0X3(this Vector6i @this) => new Vector2i(@this.X0, @this.X3);

        public static Vector2i X0X4(this Vector6i @this) => new Vector2i(@this.X0, @this.X4);

        public static Vector2i X0X5(this Vector6i @this) => new Vector2i(@this.X0, @this.X5);

        public static Vector2i X1X0(this Vector6i @this) => new Vector2i(@this.X1, @this.X0);

        public static Vector2i X1X1(this Vector6i @this) => new Vector2i(@this.X1, @this.X1);

        public static Vector2i X1X2(this Vector6i @this) => new Vector2i(@this.X1, @this.X2);

        public static Vector2i X1X3(this Vector6i @this) => new Vector2i(@this.X1, @this.X3);

        public static Vector2i X1X4(this Vector6i @this) => new Vector2i(@this.X1, @this.X4);

        public static Vector2i X1X5(this Vector6i @this) => new Vector2i(@this.X1, @this.X5);

        public static Vector2i X2X0(this Vector6i @this) => new Vector2i(@this.X2, @this.X0);

        public static Vector2i X2X1(this Vector6i @this) => new Vector2i(@this.X2, @this.X1);

        public static Vector2i X2X2(this Vector6i @this) => new Vector2i(@this.X2, @this.X2);

        public static Vector2i X2X3(this Vector6i @this) => new Vector2i(@this.X2, @this.X3);

        public static Vector2i X2X4(this Vector6i @this) => new Vector2i(@this.X2, @this.X4);

        public static Vector2i X2X5(this Vector6i @this) => new Vector2i(@this.X2, @this.X5);

        public static Vector2i X3X0(this Vector6i @this) => new Vector2i(@this.X3, @this.X0);

        public static Vector2i X3X1(this Vector6i @this) => new Vector2i(@this.X3, @this.X1);

        public static Vector2i X3X2(this Vector6i @this) => new Vector2i(@this.X3, @this.X2);

        public static Vector2i X3X3(this Vector6i @this) => new Vector2i(@this.X3, @this.X3);

        public static Vector2i X3X4(this Vector6i @this) => new Vector2i(@this.X3, @this.X4);

        public static Vector2i X3X5(this Vector6i @this) => new Vector2i(@this.X3, @this.X5);

        public static Vector2i X4X0(this Vector6i @this) => new Vector2i(@this.X4, @this.X0);

        public static Vector2i X4X1(this Vector6i @this) => new Vector2i(@this.X4, @this.X1);

        public static Vector2i X4X2(this Vector6i @this) => new Vector2i(@this.X4, @this.X2);

        public static Vector2i X4X3(this Vector6i @this) => new Vector2i(@this.X4, @this.X3);

        public static Vector2i X4X4(this Vector6i @this) => new Vector2i(@this.X4, @this.X4);

        public static Vector2i X4X5(this Vector6i @this) => new Vector2i(@this.X4, @this.X5);

        public static Vector2i X5X0(this Vector6i @this) => new Vector2i(@this.X5, @this.X0);

        public static Vector2i X5X1(this Vector6i @this) => new Vector2i(@this.X5, @this.X1);

        public static Vector2i X5X2(this Vector6i @this) => new Vector2i(@this.X5, @this.X2);

        public static Vector2i X5X3(this Vector6i @this) => new Vector2i(@this.X5, @this.X3);

        public static Vector2i X5X4(this Vector6i @this) => new Vector2i(@this.X5, @this.X4);

        public static Vector2i X5X5(this Vector6i @this) => new Vector2i(@this.X5, @this.X5);

        public static bool AllPositive(this Vector6i @this) => @this.X0 > 0 && @this.X1 > 0 && @this.X2 > 0 && @this.X3 > 0 && @this.X4 > 0 && @this.X5 > 0;

        public static bool AllZeroOrPositive(this Vector6i @this) => @this.X0 >= 0 && @this.X1 >= 0 && @this.X2 >= 0 && @this.X3 >= 0 && @this.X4 >= 0 && @this.X5 >= 0;

        public static bool AnyNegative(this Vector6i @this) => @this.X0 < 0 || @this.X1 < 0 || @this.X2 < 0 || @this.X3 < 0 || @this.X4 < 0 || @this.X5 < 0;

        public static bool AnyZero(this Vector6i @this) => @this.X0 == 0 || @this.X1 == 0 || @this.X2 == 0 || @this.X3 == 0 || @this.X4 == 0 || @this.X5 == 0;

        public static bool AnyZeroOrNegative(this Vector6i @this) => @this.X0 <= 0 || @this.X1 <= 0 || @this.X2 <= 0 || @this.X3 <= 0 || @this.X4 <= 0 || @this.X5 <= 0;

        public static Vector6i FromSame(int value) => new Vector6i(value, value, value, value, value, value);

        public static Vector6f ToVector6f(this Vector6i @this) => new Vector6f(@this.X0, @this.X1, @this.X2, @this.X3, @this.X4, @this.X5);

        public static Range6i ToRange6i(this Vector6i @this) => Range6i.FromMinAndSize(Vector6i.Zero, @this);

        public static Area6i ToArea6i(this Vector6i @this) => Area6i.FromMinAndSize(Vector6i.Zero, @this);

        public static Vector6i Max(Vector6i a, Vector6i b) => new Vector6i(Math.Max(a.X0, b.X0), Math.Max(a.X1, b.X1), Math.Max(a.X2, b.X2), Math.Max(a.X3, b.X3), Math.Max(a.X4, b.X4), Math.Max(a.X5, b.X5));

        public static Vector6i Max(Vector6i a, int b) => new Vector6i(Math.Max(a.X0, b), Math.Max(a.X1, b), Math.Max(a.X2, b), Math.Max(a.X3, b), Math.Max(a.X4, b), Math.Max(a.X5, b));

        public static Vector6i Max(int a, Vector6i b) => new Vector6i(Math.Max(a, b.X0), Math.Max(a, b.X1), Math.Max(a, b.X2), Math.Max(a, b.X3), Math.Max(a, b.X4), Math.Max(a, b.X5));

        public static Vector6i Min(Vector6i a, Vector6i b) => new Vector6i(Math.Min(a.X0, b.X0), Math.Min(a.X1, b.X1), Math.Min(a.X2, b.X2), Math.Min(a.X3, b.X3), Math.Min(a.X4, b.X4), Math.Min(a.X5, b.X5));

        public static Vector6i Min(Vector6i a, int b) => new Vector6i(Math.Min(a.X0, b), Math.Min(a.X1, b), Math.Min(a.X2, b), Math.Min(a.X3, b), Math.Min(a.X4, b), Math.Min(a.X5, b));

        public static Vector6i Min(int a, Vector6i b) => new Vector6i(Math.Min(a, b.X0), Math.Min(a, b.X1), Math.Min(a, b.X2), Math.Min(a, b.X3), Math.Min(a, b.X4), Math.Min(a, b.X5));

        public static Vector6i DivideUp(Vector6i a, Vector6i b) => new Vector6i(a.X0.DivideUp(b.X0), a.X1.DivideUp(b.X1), a.X2.DivideUp(b.X2), a.X3.DivideUp(b.X3), a.X4.DivideUp(b.X4), a.X5.DivideUp(b.X5));

        public static Vector6i DivideUp(Vector6i a, int b) => new Vector6i(a.X0.DivideUp(b), a.X1.DivideUp(b), a.X2.DivideUp(b), a.X3.DivideUp(b), a.X4.DivideUp(b), a.X5.DivideUp(b));

        public static Vector6i DivideUp(int a, Vector6i b) => new Vector6i(a.DivideUp(b.X0), a.DivideUp(b.X1), a.DivideUp(b.X2), a.DivideUp(b.X3), a.DivideUp(b.X4), a.DivideUp(b.X5));
    }
}
