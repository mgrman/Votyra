using System;
using Newtonsoft.Json;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Vector4i : IEquatable<Vector4i>
    {
        public static readonly Vector4i Zero = new Vector4i(0, 0, 0, 0);

        public static readonly Vector4i One = new Vector4i(1, 1, 1, 1);

        public readonly int X;

        public readonly int Y;

        public readonly int Z;

        public readonly int W;

        [JsonConstructor]
        public Vector4i(int x, int y, int z, int w)
        {
            X = x;

            Y = y;

            Z = z;

            W = w;
        }

        public static Vector4i operator -(Vector4i a) => new Vector4i(-a.X, -a.Y, -a.Z, -a.W);

        public static Vector4i operator *(Vector4i a, Vector4i b) => new Vector4i(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);

        public static Vector4i operator *(Vector4i a, int b) => new Vector4i(a.X * b, a.Y * b, a.Z * b, a.W * b);
        public static Vector4i operator *(int a, Vector4i b) => new Vector4i(a * b.X, a * b.Y, a * b.Z, a * b.W);

        public static Vector4i operator +(Vector4i a, Vector4i b) => new Vector4i(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);

        public static Vector4i operator +(Vector4i a, int b) => new Vector4i(a.X + b, a.Y + b, a.Z + b, a.W + b);
        public static Vector4i operator +(int a, Vector4i b) => new Vector4i(a + b.X, a + b.Y, a + b.Z, a + b.W);

        public static Vector4i operator -(Vector4i a, Vector4i b) => new Vector4i(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);

        public static Vector4i operator -(Vector4i a, int b) => new Vector4i(a.X - b, a.Y - b, a.Z - b, a.W - b);
        public static Vector4i operator -(int a, Vector4i b) => new Vector4i(a - b.X, a - b.Y, a - b.Z, a - b.W);

        public static Vector4i operator /(Vector4i a, Vector4i b) => new Vector4i(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);

        public static Vector4i operator /(Vector4i a, int b) => new Vector4i(a.X / b, a.Y / b, a.Z / b, a.W / b);
        public static Vector4i operator /(int a, Vector4i b) => new Vector4i(a / b.X, a / b.Y, a / b.Z, a / b.W);

        public static Vector4i operator %(Vector4i a, Vector4i b) => new Vector4i(a.X % b.X, a.Y % b.Y, a.Z % b.Z, a.W % b.W);

        public static Vector4i operator %(Vector4i a, int b) => new Vector4i(a.X % b, a.Y % b, a.Z % b, a.W % b);
        public static Vector4i operator %(int a, Vector4i b) => new Vector4i(a % b.X, a % b.Y, a % b.Z, a % b.W);

        public static bool operator <(Vector4i a, Vector4i b) => a.X < b.X && a.Y < b.Y && a.Z < b.Z && a.W < b.W;

        public static bool operator <(Vector4i a, int b) => a.X < b && a.Y < b && a.Z < b && a.W < b;
        public static bool operator <(int a, Vector4i b) => a < b.X && a < b.Y && a < b.Z && a < b.W;

        public static bool operator >(Vector4i a, Vector4i b) => a.X > b.X && a.Y > b.Y && a.Z > b.Z && a.W > b.W;

        public static bool operator >(Vector4i a, int b) => a.X > b && a.Y > b && a.Z > b && a.W > b;
        public static bool operator >(int a, Vector4i b) => a > b.X && a > b.Y && a > b.Z && a > b.W;

        public static bool operator <=(Vector4i a, Vector4i b) => a.X <= b.X && a.Y <= b.Y && a.Z <= b.Z && a.W <= b.W;

        public static bool operator <=(Vector4i a, int b) => a.X <= b && a.Y <= b && a.Z <= b && a.W <= b;
        public static bool operator <=(int a, Vector4i b) => a <= b.X && a <= b.Y && a <= b.Z && a <= b.W;

        public static bool operator >=(Vector4i a, Vector4i b) => a.X >= b.X && a.Y >= b.Y && a.Z >= b.Z && a.W >= b.W;

        public static bool operator >=(Vector4i a, int b) => a.X >= b && a.Y >= b && a.Z >= b && a.W >= b;
        public static bool operator >=(int a, Vector4i b) => a >= b.X && a >= b.Y && a >= b.Z && a >= b.W;

        public static bool operator ==(Vector4i a, Vector4i b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;

        public static bool operator ==(Vector4i a, int b) => a.X == b && a.Y == b && a.Z == b && a.W == b;
        public static bool operator ==(int a, Vector4i b) => a == b.X && a == b.Y && a == b.Z && a == b.W;

        public static bool operator !=(Vector4i a, Vector4i b) => a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.W != b.W;

        public static bool operator !=(Vector4i a, int b) => a.X != b || a.Y != b || a.Z != b || a.W != b;
        public static bool operator !=(int a, Vector4i b) => a != b.X || a != b.Y || a != b.Z || a != b.W;

        public static Vector4f operator *(Vector4i a, Vector4f b) => new Vector4f(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
        public static Vector4f operator *(Vector4f a, Vector4i b) => new Vector4f(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
        public static Vector4f operator *(Vector4i a, float b) => new Vector4f(a.X * b, a.Y * b, a.Z * b, a.W * b);
        public static Vector4f operator *(float a, Vector4i b) => new Vector4f(a * b.X, a * b.Y, a * b.Z, a * b.W);

        public static Vector4f operator +(Vector4i a, Vector4f b) => new Vector4f(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        public static Vector4f operator +(Vector4f a, Vector4i b) => new Vector4f(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        public static Vector4f operator +(Vector4i a, float b) => new Vector4f(a.X + b, a.Y + b, a.Z + b, a.W + b);
        public static Vector4f operator +(float a, Vector4i b) => new Vector4f(a + b.X, a + b.Y, a + b.Z, a + b.W);

        public static Vector4f operator -(Vector4i a, Vector4f b) => new Vector4f(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        public static Vector4f operator -(Vector4f a, Vector4i b) => new Vector4f(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        public static Vector4f operator -(Vector4i a, float b) => new Vector4f(a.X - b, a.Y - b, a.Z - b, a.W - b);
        public static Vector4f operator -(float a, Vector4i b) => new Vector4f(a - b.X, a - b.Y, a - b.Z, a - b.W);

        public static Vector4f operator /(Vector4i a, Vector4f b) => new Vector4f(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);
        public static Vector4f operator /(Vector4f a, Vector4i b) => new Vector4f(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);
        public static Vector4f operator /(Vector4i a, float b) => new Vector4f(a.X / b, a.Y / b, a.Z / b, a.W / b);
        public static Vector4f operator /(float a, Vector4i b) => new Vector4f(a / b.X, a / b.Y, a / b.Z, a / b.W);

        public static Vector4f operator %(Vector4i a, Vector4f b) => new Vector4f(a.X % b.X, a.Y % b.Y, a.Z % b.Z, a.W % b.W);
        public static Vector4f operator %(Vector4f a, Vector4i b) => new Vector4f(a.X % b.X, a.Y % b.Y, a.Z % b.Z, a.W % b.W);
        public static Vector4f operator %(Vector4i a, float b) => new Vector4f(a.X % b, a.Y % b, a.Z % b, a.W % b);
        public static Vector4f operator %(float a, Vector4i b) => new Vector4f(a % b.X, a % b.Y, a % b.Z, a % b.W);

        public bool Equals(Vector4i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Vector4i))
                return false;

            return Equals((Vector4i) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ (Y * 397) ^ (Z * 397) ^ (W * 397);
            }
        }

        public override string ToString() => "(" + X + "," + Y + "," + Z + "," + W + ")";
    }

    public static class Vector4iUtils
    {
        public static readonly Vector4i PlusOneX = new Vector4i(1, 0, 0, 0);
        public static readonly Vector4i MinusOneX = new Vector4i(-1, 0, 0, 0);

        public static readonly Vector4i PlusOneY = new Vector4i(0, 1, 0, 0);
        public static readonly Vector4i MinusOneY = new Vector4i(0, -1, 0, 0);

        public static readonly Vector4i PlusOneZ = new Vector4i(0, 0, 1, 0);
        public static readonly Vector4i MinusOneZ = new Vector4i(0, 0, -1, 0);

        public static readonly Vector4i PlusOneW = new Vector4i(0, 0, 0, 1);
        public static readonly Vector4i MinusOneW = new Vector4i(0, 0, 0, -1);
        public static float Magnitude(this Vector4i @this) => (float) Math.Sqrt(@this.SqrMagnitude());

        public static Vector4f Normalized(this Vector4i @this)
        {
            var magnitude = @this.Magnitude();
            return magnitude <= float.Epsilon ? @this.ToVector4f() : @this / magnitude;
        }

        public static float SqrMagnitude(this Vector4i @this) => @this.X * @this.X + @this.Y * @this.Y + @this.Z * @this.Z + @this.W * @this.W;

        public static int ManhattanMagnitude(this Vector4i @this) => @this.X + @this.Y + @this.Z + @this.W;

        public static int AreaSum(this Vector4i @this) => @this.X * @this.Y * @this.Z * @this.W;

        public static int Volume(this Vector4i @this) => @this.X * @this.Y * @this.Z * @this.W;

        public static Vector2i XX(this Vector4i @this) => new Vector2i(@this.X, @this.X);

        public static Vector2i XY(this Vector4i @this) => new Vector2i(@this.X, @this.Y);

        public static Vector2i XZ(this Vector4i @this) => new Vector2i(@this.X, @this.Z);

        public static Vector2i XW(this Vector4i @this) => new Vector2i(@this.X, @this.W);

        public static Vector2i YX(this Vector4i @this) => new Vector2i(@this.Y, @this.X);

        public static Vector2i YY(this Vector4i @this) => new Vector2i(@this.Y, @this.Y);

        public static Vector2i YZ(this Vector4i @this) => new Vector2i(@this.Y, @this.Z);

        public static Vector2i YW(this Vector4i @this) => new Vector2i(@this.Y, @this.W);

        public static Vector2i ZX(this Vector4i @this) => new Vector2i(@this.Z, @this.X);

        public static Vector2i ZY(this Vector4i @this) => new Vector2i(@this.Z, @this.Y);

        public static Vector2i ZZ(this Vector4i @this) => new Vector2i(@this.Z, @this.Z);

        public static Vector2i ZW(this Vector4i @this) => new Vector2i(@this.Z, @this.W);

        public static Vector2i WX(this Vector4i @this) => new Vector2i(@this.W, @this.X);

        public static Vector2i WY(this Vector4i @this) => new Vector2i(@this.W, @this.Y);

        public static Vector2i WZ(this Vector4i @this) => new Vector2i(@this.W, @this.Z);

        public static Vector2i WW(this Vector4i @this) => new Vector2i(@this.W, @this.W);

        public static bool AllPositive(this Vector4i @this) => @this.X > 0 && @this.Y > 0 && @this.Z > 0 && @this.W > 0;

        public static bool AllZeroOrPositive(this Vector4i @this) => @this.X >= 0 && @this.Y >= 0 && @this.Z >= 0 && @this.W >= 0;

        public static bool AnyNegative(this Vector4i @this) => @this.X < 0 || @this.Y < 0 || @this.Z < 0 || @this.W < 0;

        public static bool AnyZero(this Vector4i @this) => @this.X == 0 || @this.Y == 0 || @this.Z == 0 || @this.W == 0;

        public static bool AnyZeroOrNegative(this Vector4i @this) => @this.X <= 0 || @this.Y <= 0 || @this.Z <= 0 || @this.W <= 0;

        public static Vector4i FromSame(int value) => new Vector4i(value, value, value, value);

        public static Vector4f ToVector4f(this Vector4i @this) => new Vector4f(@this.X, @this.Y, @this.Z, @this.W);

        public static Range4i ToRange4i(this Vector4i @this) => Range4i.FromMinAndSize(Vector4i.Zero, @this);
        public static Area4i ToArea4i(this Vector4i @this) => Area4i.FromMinAndSize(Vector4i.Zero, @this);

        public static Vector5i ToVector5i(this Vector4i @this, int x5) => new Vector5i(@this.X, @this.Y, @this.Z, @this.W, x5);
        public static Vector5f ToVector5f(this Vector4i @this, float x5) => new Vector5f(@this.X, @this.Y, @this.Z, @this.W, x5);

        public static Vector6i ToVector6i(this Vector4i @this, int x5, int x6) => new Vector6i(@this.X, @this.Y, @this.Z, @this.W, x5, x6);
        public static Vector6f ToVector6f(this Vector4i @this, float x5, float x6) => new Vector6f(@this.X, @this.Y, @this.Z, @this.W, x5, x6);

        public static Vector4i Max(Vector4i a, Vector4i b) => new Vector4i(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z), Math.Max(a.W, b.W));

        public static Vector4i Max(Vector4i a, int b) => new Vector4i(Math.Max(a.X, b), Math.Max(a.Y, b), Math.Max(a.Z, b), Math.Max(a.W, b));
        public static Vector4i Max(int a, Vector4i b) => new Vector4i(Math.Max(a, b.X), Math.Max(a, b.Y), Math.Max(a, b.Z), Math.Max(a, b.W));

        public static Vector4i Min(Vector4i a, Vector4i b) => new Vector4i(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z), Math.Min(a.W, b.W));

        public static Vector4i Min(Vector4i a, int b) => new Vector4i(Math.Min(a.X, b), Math.Min(a.Y, b), Math.Min(a.Z, b), Math.Min(a.W, b));
        public static Vector4i Min(int a, Vector4i b) => new Vector4i(Math.Min(a, b.X), Math.Min(a, b.Y), Math.Min(a, b.Z), Math.Min(a, b.W));

        public static Vector4i DivideUp(Vector4i a, Vector4i b) => new Vector4i(a.X.DivideUp(b.X), a.Y.DivideUp(b.Y), a.Z.DivideUp(b.Z), a.W.DivideUp(b.W));

        public static Vector4i DivideUp(Vector4i a, int b) => new Vector4i(a.X.DivideUp(b), a.Y.DivideUp(b), a.Z.DivideUp(b), a.W.DivideUp(b));
        public static Vector4i DivideUp(int a, Vector4i b) => new Vector4i(a.DivideUp(b.X), a.DivideUp(b.Y), a.DivideUp(b.Z), a.DivideUp(b.W));
    }
}