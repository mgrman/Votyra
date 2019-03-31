using System;
using Newtonsoft.Json;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Vector3i : IEquatable<Vector3i>
    {
        public static readonly Vector3i Zero = new Vector3i(0, 0, 0);

        public static readonly Vector3i One = new Vector3i(1, 1, 1);

        public readonly int X;

        public readonly int Y;

        public readonly int Z;

        [JsonConstructor]
        public Vector3i(int x, int y, int z)
        {
            X = x;

            Y = y;

            Z = z;
        }

        public static Vector3i operator -(Vector3i a) => new Vector3i(-a.X, -a.Y, -a.Z);

        public static Vector3i operator *(Vector3i a, Vector3i b) => new Vector3i(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

        public static Vector3i operator *(Vector3i a, int b) => new Vector3i(a.X * b, a.Y * b, a.Z * b);
        public static Vector3i operator *(int a, Vector3i b) => new Vector3i(a * b.X, a * b.Y, a * b.Z);

        public static Vector3i operator +(Vector3i a, Vector3i b) => new Vector3i(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static Vector3i operator +(Vector3i a, int b) => new Vector3i(a.X + b, a.Y + b, a.Z + b);
        public static Vector3i operator +(int a, Vector3i b) => new Vector3i(a + b.X, a + b.Y, a + b.Z);

        public static Vector3i operator -(Vector3i a, Vector3i b) => new Vector3i(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static Vector3i operator -(Vector3i a, int b) => new Vector3i(a.X - b, a.Y - b, a.Z - b);
        public static Vector3i operator -(int a, Vector3i b) => new Vector3i(a - b.X, a - b.Y, a - b.Z);

        public static Vector3i operator /(Vector3i a, Vector3i b) => new Vector3i(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

        public static Vector3i operator /(Vector3i a, int b) => new Vector3i(a.X / b, a.Y / b, a.Z / b);
        public static Vector3i operator /(int a, Vector3i b) => new Vector3i(a / b.X, a / b.Y, a / b.Z);

        public static Vector3i operator %(Vector3i a, Vector3i b) => new Vector3i(a.X % b.X, a.Y % b.Y, a.Z % b.Z);

        public static Vector3i operator %(Vector3i a, int b) => new Vector3i(a.X % b, a.Y % b, a.Z % b);
        public static Vector3i operator %(int a, Vector3i b) => new Vector3i(a % b.X, a % b.Y, a % b.Z);

        public static bool operator <(Vector3i a, Vector3i b) => a.X < b.X && a.Y < b.Y && a.Z < b.Z;

        public static bool operator <(Vector3i a, int b) => a.X < b && a.Y < b && a.Z < b;
        public static bool operator <(int a, Vector3i b) => a < b.X && a < b.Y && a < b.Z;

        public static bool operator >(Vector3i a, Vector3i b) => a.X > b.X && a.Y > b.Y && a.Z > b.Z;

        public static bool operator >(Vector3i a, int b) => a.X > b && a.Y > b && a.Z > b;
        public static bool operator >(int a, Vector3i b) => a > b.X && a > b.Y && a > b.Z;

        public static bool operator <=(Vector3i a, Vector3i b) => a.X <= b.X && a.Y <= b.Y && a.Z <= b.Z;

        public static bool operator <=(Vector3i a, int b) => a.X <= b && a.Y <= b && a.Z <= b;
        public static bool operator <=(int a, Vector3i b) => a <= b.X && a <= b.Y && a <= b.Z;

        public static bool operator >=(Vector3i a, Vector3i b) => a.X >= b.X && a.Y >= b.Y && a.Z >= b.Z;

        public static bool operator >=(Vector3i a, int b) => a.X >= b && a.Y >= b && a.Z >= b;
        public static bool operator >=(int a, Vector3i b) => a >= b.X && a >= b.Y && a >= b.Z;

        public static bool operator ==(Vector3i a, Vector3i b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z;

        public static bool operator ==(Vector3i a, int b) => a.X == b && a.Y == b && a.Z == b;
        public static bool operator ==(int a, Vector3i b) => a == b.X && a == b.Y && a == b.Z;

        public static bool operator !=(Vector3i a, Vector3i b) => a.X != b.X || a.Y != b.Y || a.Z != b.Z;

        public static bool operator !=(Vector3i a, int b) => a.X != b || a.Y != b || a.Z != b;
        public static bool operator !=(int a, Vector3i b) => a != b.X || a != b.Y || a != b.Z;

        public static Vector3f operator *(Vector3i a, Vector3f b) => new Vector3f(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        public static Vector3f operator *(Vector3f a, Vector3i b) => new Vector3f(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        public static Vector3f operator *(Vector3i a, float b) => new Vector3f(a.X * b, a.Y * b, a.Z * b);
        public static Vector3f operator *(float a, Vector3i b) => new Vector3f(a * b.X, a * b.Y, a * b.Z);

        public static Vector3f operator +(Vector3i a, Vector3f b) => new Vector3f(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3f operator +(Vector3f a, Vector3i b) => new Vector3f(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3f operator +(Vector3i a, float b) => new Vector3f(a.X + b, a.Y + b, a.Z + b);
        public static Vector3f operator +(float a, Vector3i b) => new Vector3f(a + b.X, a + b.Y, a + b.Z);

        public static Vector3f operator -(Vector3i a, Vector3f b) => new Vector3f(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3f operator -(Vector3f a, Vector3i b) => new Vector3f(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3f operator -(Vector3i a, float b) => new Vector3f(a.X - b, a.Y - b, a.Z - b);
        public static Vector3f operator -(float a, Vector3i b) => new Vector3f(a - b.X, a - b.Y, a - b.Z);

        public static Vector3f operator /(Vector3i a, Vector3f b) => new Vector3f(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        public static Vector3f operator /(Vector3f a, Vector3i b) => new Vector3f(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        public static Vector3f operator /(Vector3i a, float b) => new Vector3f(a.X / b, a.Y / b, a.Z / b);
        public static Vector3f operator /(float a, Vector3i b) => new Vector3f(a / b.X, a / b.Y, a / b.Z);

        public static Vector3f operator %(Vector3i a, Vector3f b) => new Vector3f(a.X % b.X, a.Y % b.Y, a.Z % b.Z);
        public static Vector3f operator %(Vector3f a, Vector3i b) => new Vector3f(a.X % b.X, a.Y % b.Y, a.Z % b.Z);
        public static Vector3f operator %(Vector3i a, float b) => new Vector3f(a.X % b, a.Y % b, a.Z % b);
        public static Vector3f operator %(float a, Vector3i b) => new Vector3f(a % b.X, a % b.Y, a % b.Z);

        public bool Equals(Vector3i other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Vector3i))
                return false;

            return Equals((Vector3i) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ (Y * 397) ^ (Z * 397);
            }
        }

        public override string ToString() => "(" + X + "," + Y + "," + Z + ")";
    }

    public static class Vector3iUtils
    {
        public static readonly Vector3i PlusOneX = new Vector3i(1, 0, 0);
        public static readonly Vector3i MinusOneX = new Vector3i(-1, 0, 0);

        public static readonly Vector3i PlusOneY = new Vector3i(0, 1, 0);
        public static readonly Vector3i MinusOneY = new Vector3i(0, -1, 0);

        public static readonly Vector3i PlusOneZ = new Vector3i(0, 0, 1);
        public static readonly Vector3i MinusOneZ = new Vector3i(0, 0, -1);
        public static float Magnitude(this Vector3i @this) => (float) Math.Sqrt(@this.SqrMagnitude());

        public static Vector3f Normalized(this Vector3i @this)
        {
            var magnitude = @this.Magnitude();
            return magnitude <= float.Epsilon ? @this.ToVector3f() : @this / magnitude;
        }

        public static float SqrMagnitude(this Vector3i @this) => @this.X * @this.X + @this.Y * @this.Y + @this.Z * @this.Z;

        public static int ManhattanMagnitude(this Vector3i @this) => @this.X + @this.Y + @this.Z;

        public static int AreaSum(this Vector3i @this) => @this.X * @this.Y * @this.Z;

        public static int Volume(this Vector3i @this) => @this.X * @this.Y * @this.Z;

        public static Vector2i XX(this Vector3i @this) => new Vector2i(@this.X, @this.X);

        public static Vector2i XY(this Vector3i @this) => new Vector2i(@this.X, @this.Y);

        public static Vector2i XZ(this Vector3i @this) => new Vector2i(@this.X, @this.Z);

        public static Vector2i YX(this Vector3i @this) => new Vector2i(@this.Y, @this.X);

        public static Vector2i YY(this Vector3i @this) => new Vector2i(@this.Y, @this.Y);

        public static Vector2i YZ(this Vector3i @this) => new Vector2i(@this.Y, @this.Z);

        public static Vector2i ZX(this Vector3i @this) => new Vector2i(@this.Z, @this.X);

        public static Vector2i ZY(this Vector3i @this) => new Vector2i(@this.Z, @this.Y);

        public static Vector2i ZZ(this Vector3i @this) => new Vector2i(@this.Z, @this.Z);

        public static bool AllPositive(this Vector3i @this) => @this.X > 0 && @this.Y > 0 && @this.Z > 0;

        public static bool AllZeroOrPositive(this Vector3i @this) => @this.X >= 0 && @this.Y >= 0 && @this.Z >= 0;

        public static bool AnyNegative(this Vector3i @this) => @this.X < 0 || @this.Y < 0 || @this.Z < 0;

        public static bool AnyZero(this Vector3i @this) => @this.X == 0 || @this.Y == 0 || @this.Z == 0;

        public static bool AnyZeroOrNegative(this Vector3i @this) => @this.X <= 0 || @this.Y <= 0 || @this.Z <= 0;

        public static Vector3i FromSame(int value) => new Vector3i(value, value, value);

        public static Vector3i Cross(Vector3i lhs, Vector3i rhs) => new Vector3i(lhs.Y * rhs.Z - lhs.Z * rhs.Y, lhs.Z * rhs.X - lhs.X * rhs.Z, lhs.X * rhs.Y - lhs.Y * rhs.X);

        public static Vector3f ToVector3f(this Vector3i @this) => new Vector3f(@this.X, @this.Y, @this.Z);

        public static Range3i ToRange3i(this Vector3i @this) => Range3i.FromMinAndSize(Vector3i.Zero, @this);
        public static Area3i ToArea3i(this Vector3i @this) => Area3i.FromMinAndSize(Vector3i.Zero, @this);

        public static Vector4i ToVector4i(this Vector3i @this, int x4) => new Vector4i(@this.X, @this.Y, @this.Z, x4);
        public static Vector4f ToVector4f(this Vector3i @this, float x4) => new Vector4f(@this.X, @this.Y, @this.Z, x4);

        public static Vector5i ToVector5i(this Vector3i @this, int x4, int x5) => new Vector5i(@this.X, @this.Y, @this.Z, x4, x5);
        public static Vector5f ToVector5f(this Vector3i @this, float x4, float x5) => new Vector5f(@this.X, @this.Y, @this.Z, x4, x5);

        public static Vector6i ToVector6i(this Vector3i @this, int x4, int x5, int x6) => new Vector6i(@this.X, @this.Y, @this.Z, x4, x5, x6);
        public static Vector6f ToVector6f(this Vector3i @this, float x4, float x5, float x6) => new Vector6f(@this.X, @this.Y, @this.Z, x4, x5, x6);

        public static Vector3i Max(Vector3i a, Vector3i b) => new Vector3i(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

        public static Vector3i Max(Vector3i a, int b) => new Vector3i(Math.Max(a.X, b), Math.Max(a.Y, b), Math.Max(a.Z, b));
        public static Vector3i Max(int a, Vector3i b) => new Vector3i(Math.Max(a, b.X), Math.Max(a, b.Y), Math.Max(a, b.Z));

        public static Vector3i Min(Vector3i a, Vector3i b) => new Vector3i(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));

        public static Vector3i Min(Vector3i a, int b) => new Vector3i(Math.Min(a.X, b), Math.Min(a.Y, b), Math.Min(a.Z, b));
        public static Vector3i Min(int a, Vector3i b) => new Vector3i(Math.Min(a, b.X), Math.Min(a, b.Y), Math.Min(a, b.Z));

        public static Vector3i DivideUp(Vector3i a, Vector3i b) => new Vector3i(a.X.DivideUp(b.X), a.Y.DivideUp(b.Y), a.Z.DivideUp(b.Z));

        public static Vector3i DivideUp(Vector3i a, int b) => new Vector3i(a.X.DivideUp(b), a.Y.DivideUp(b), a.Z.DivideUp(b));
        public static Vector3i DivideUp(int a, Vector3i b) => new Vector3i(a.DivideUp(b.X), a.DivideUp(b.Y), a.DivideUp(b.Z));
    }
}