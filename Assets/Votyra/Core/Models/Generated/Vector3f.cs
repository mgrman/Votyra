using System;
using Newtonsoft.Json;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Vector3f : IEquatable<Vector3f>
    {
        public static readonly Vector3f One = new Vector3f(1, 1, 1);
        public static readonly Vector3f Zero = new Vector3f(0, 0, 0);
        public static readonly Vector3f NaN = new Vector3f(float.NaN, float.NaN, float.NaN);

        public readonly float X;

        public readonly float Y;

        public readonly float Z;

        [JsonConstructor]
        public Vector3f(float x, float y, float z)
        {
            X = x;

            Y = y;

            Z = z;
        }

        public static Vector3f operator -(Vector3f a) => new Vector3f(-a.X, -a.Y, -a.Z);

        public static Vector3f operator *(Vector3f a, Vector3f b) => new Vector3f(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

        public static Vector3f operator *(Vector3f a, float b) => new Vector3f(a.X * b, a.Y * b, a.Z * b);

        public static Vector3f operator *(float a, Vector3f b) => new Vector3f(a * b.X, a * b.Y, a * b.Z);

        public static Vector3f operator *(Vector3f a, int b) => new Vector3f(a.X * b, a.Y * b, a.Z * b);

        public static Vector3f operator *(int a, Vector3f b) => new Vector3f(a * b.X, a * b.Y, a * b.Z);

        public static Vector3f operator +(Vector3f a, Vector3f b) => new Vector3f(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static Vector3f operator +(Vector3f a, float b) => new Vector3f(a.X + b, a.Y + b, a.Z + b);

        public static Vector3f operator +(float a, Vector3f b) => new Vector3f(a + b.X, a + b.Y, a + b.Z);

        public static Vector3f operator +(Vector3f a, int b) => new Vector3f(a.X + b, a.Y + b, a.Z + b);

        public static Vector3f operator +(int a, Vector3f b) => new Vector3f(a + b.X, a + b.Y, a + b.Z);

        public static Vector3f operator -(Vector3f a, Vector3f b) => new Vector3f(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static Vector3f operator -(Vector3f a, float b) => new Vector3f(a.X - b, a.Y - b, a.Z - b);

        public static Vector3f operator -(float a, Vector3f b) => new Vector3f(a - b.X, a - b.Y, a - b.Z);

        public static Vector3f operator -(Vector3f a, int b) => new Vector3f(a.X - b, a.Y - b, a.Z - b);

        public static Vector3f operator -(int a, Vector3f b) => new Vector3f(a - b.X, a - b.Y, a - b.Z);

        public static Vector3f operator /(Vector3f a, Vector3f b) => new Vector3f(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

        public static Vector3f operator /(Vector3f a, float b) => new Vector3f(a.X / b, a.Y / b, a.Z / b);

        public static Vector3f operator /(float a, Vector3f b) => new Vector3f(a / b.X, a / b.Y, a / b.Z);

        public static Vector3f operator /(Vector3f a, int b) => new Vector3f(a.X / b, a.Y / b, a.Z / b);

        public static Vector3f operator /(int a, Vector3f b) => new Vector3f(a / b.X, a / b.Y, a / b.Z);

        public static Vector3f operator %(Vector3f a, Vector3f b) => new Vector3f(a.X % b.X, a.Y % b.Y, a.Z % b.Z);

        public static Vector3f operator %(Vector3f a, float b) => new Vector3f(a.X % b, a.Y % b, a.Z % b);

        public static Vector3f operator %(float a, Vector3f b) => new Vector3f(a % b.X, a % b.Y, a % b.Z);

        public static Vector3f operator %(Vector3f a, int b) => new Vector3f(a.X % b, a.Y % b, a.Z % b);

        public static Vector3f operator %(int a, Vector3f b) => new Vector3f(a % b.X, a % b.Y, a % b.Z);

        public static bool operator <(Vector3f a, Vector3f b) => a.X < b.X && a.Y < b.Y && a.Z < b.Z;

        public static bool operator <(Vector3f a, float b) => a.X < b && a.Y < b && a.Z < b;

        public static bool operator <(float a, Vector3f b) => a < b.X && a < b.Y && a < b.Z;

        public static bool operator <(Vector3f a, int b) => a.X < b && a.Y < b && a.Z < b;

        public static bool operator <(int a, Vector3f b) => a < b.X && a < b.Y && a < b.Z;

        public static bool operator >(Vector3f a, Vector3f b) => a.X > b.X && a.Y > b.Y && a.Z > b.Z;

        public static bool operator >(Vector3f a, float b) => a.X > b && a.Y > b && a.Z > b;

        public static bool operator >(float a, Vector3f b) => a > b.X && a > b.Y && a > b.Z;

        public static bool operator >(Vector3f a, int b) => a.X > b && a.Y > b && a.Z > b;

        public static bool operator >(int a, Vector3f b) => a > b.X && a > b.Y && a > b.Z;

        public static bool operator <=(Vector3f a, Vector3f b) => a.X <= b.X && a.Y <= b.Y && a.Z <= b.Z;

        public static bool operator <=(Vector3f a, float b) => a.X <= b && a.Y <= b && a.Z <= b;

        public static bool operator <=(float a, Vector3f b) => a <= b.X && a <= b.Y && a <= b.Z;

        public static bool operator <=(Vector3f a, int b) => a.X <= b && a.Y <= b && a.Z <= b;

        public static bool operator <=(int a, Vector3f b) => a <= b.X && a <= b.Y && a <= b.Z;

        public static bool operator >=(Vector3f a, Vector3f b) => a.X >= b.X && a.Y >= b.Y && a.Z >= b.Z;

        public static bool operator >=(Vector3f a, float b) => a.X >= b && a.Y >= b && a.Z >= b;

        public static bool operator >=(float a, Vector3f b) => a >= b.X && a >= b.Y && a >= b.Z;

        public static bool operator >=(Vector3f a, int b) => a.X >= b && a.Y >= b && a.Z >= b;

        public static bool operator >=(int a, Vector3f b) => a >= b.X && a >= b.Y && a >= b.Z;

        public static bool operator ==(Vector3f a, Vector3f b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z;

        public static bool operator ==(Vector3f a, float b) => a.X == b && a.Y == b && a.Z == b;

        public static bool operator ==(float a, Vector3f b) => a == b.X && a == b.Y && a == b.Z;

        public static bool operator ==(Vector3f a, int b) => a.X == b && a.Y == b && a.Z == b;

        public static bool operator ==(int a, Vector3f b) => a == b.X && a == b.Y && a == b.Z;

        public static bool operator !=(Vector3f a, Vector3f b) => a.X != b.X || a.Y != b.Y || a.Z != b.Z;

        public static bool operator !=(Vector3f a, float b) => a.X != b || a.Y != b || a.Z != b;

        public static bool operator !=(float a, Vector3f b) => a != b.X || a != b.Y || a != b.Z;

        public static bool operator !=(Vector3f a, int b) => a.X != b || a.Y != b || a.Z != b;

        public static bool operator !=(int a, Vector3f b) => a != b.X || a != b.Y || a != b.Z;

        public bool Equals(Vector3f other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Vector3f))
                return false;

            return Equals((Vector3f) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ (Y.GetHashCode() * 397) ^ (Z.GetHashCode() * 397);
            }
        }

        public override string ToString() => "(" + X + "," + Y + "," + Z + ")";
    }

    public static class Vector3fUtils
    {
        public static Vector2f XX(this Vector3f @this) => new Vector2f(@this.X, @this.X);

        public static Vector2f XY(this Vector3f @this) => new Vector2f(@this.X, @this.Y);

        public static Vector2f XZ(this Vector3f @this) => new Vector2f(@this.X, @this.Z);

        public static Vector2f YX(this Vector3f @this) => new Vector2f(@this.Y, @this.X);

        public static Vector2f YY(this Vector3f @this) => new Vector2f(@this.Y, @this.Y);

        public static Vector2f YZ(this Vector3f @this) => new Vector2f(@this.Y, @this.Z);

        public static Vector2f ZX(this Vector3f @this) => new Vector2f(@this.Z, @this.X);

        public static Vector2f ZY(this Vector3f @this) => new Vector2f(@this.Z, @this.Y);

        public static Vector2f ZZ(this Vector3f @this) => new Vector2f(@this.Z, @this.Z);

        public static bool AnyNegative(this Vector3f @this) => @this.X < 0 || @this.Y < 0 || @this.Z < 0;

        public static bool AnyNan(this Vector3f @this) => float.IsNaN(@this.X) || float.IsNaN(@this.Y) || float.IsNaN(@this.Z);

        public static bool NoNan(this Vector3f @this) => !float.IsNaN(@this.X) && !float.IsNaN(@this.Y) && !float.IsNaN(@this.Z);

        public static bool AnyInfinity(this Vector3f @this) => float.IsInfinity(@this.X) || float.IsInfinity(@this.Y) || float.IsInfinity(@this.Z);

        public static float AreaSum(this Vector3f @this) => @this.X * @this.Y * @this.Z;

        public static float Magnitude(this Vector3f @this) => (float) Math.Sqrt(@this.SqrMagnitude());

        public static Vector3f Normalized(this Vector3f @this)
        {
            var magnitude = @this.Magnitude();
            return magnitude <= float.Epsilon ? @this : @this / magnitude;
        }

        public static float SqrMagnitude(this Vector3f @this) => @this.X * @this.X + @this.Y * @this.Y + @this.Z * @this.Z;

        public static Vector3f FromSame(float value) => new Vector3f(value, value, value);

        public static float Dot(Vector3f a, Vector3f b) => (float) ((double) a.X * b.X + (double) a.Y * b.Y + (double) a.Z * b.Z);

        public static Vector3f Ceil(this Vector3f @this) => new Vector3f(@this.X.CeilToInt(), @this.Y.CeilToInt(), @this.Z.CeilToInt());

        public static Vector3i CeilToVector3i(this Vector3f @this) => new Vector3i(@this.X.CeilToInt(), @this.Y.CeilToInt(), @this.Z.CeilToInt());

        public static Vector3f Abs(this Vector3f @this) => new Vector3f(Math.Abs(@this.X), Math.Abs(@this.Y), Math.Abs(@this.Z));

        public static Vector3f Floor(this Vector3f @this) => new Vector3f(@this.X.FloorToInt(), @this.Y.FloorToInt(), @this.Z.FloorToInt());

        public static Vector3i FloorToVector3i(this Vector3f @this) => new Vector3i(@this.X.FloorToInt(), @this.Y.FloorToInt(), @this.Z.FloorToInt());

        public static Vector3f Round(this Vector3f @this) => new Vector3f(@this.X.RoundToInt(), @this.Y.RoundToInt(), @this.Z.RoundToInt());

        public static Vector3i RoundToVector3i(this Vector3f @this) => new Vector3i(@this.X.RoundToInt(), @this.Y.RoundToInt(), @this.Z.RoundToInt());

        public static Vector4f ToVector4f(this Vector3f @this, float x4) => new Vector4f(@this.X, @this.Y, @this.Z, x4);

        public static Vector5f ToVector5f(this Vector3f @this, float x4, float x5) => new Vector5f(@this.X, @this.Y, @this.Z, x4, x5);

        public static Vector6f ToVector6f(this Vector3f @this, float x4, float x5, float x6) => new Vector6f(@this.X, @this.Y, @this.Z, x4, x5, x6);

        public static Area3f ToArea3f(this Vector3f @this) => Area3f.FromMinAndSize(Vector3f.Zero, @this);

        public static Vector3f Cross(Vector3f lhs, Vector3f rhs) => new Vector3f(lhs.Y * rhs.Z - lhs.Z * rhs.Y, lhs.Z * rhs.X - lhs.X * rhs.Z, lhs.X * rhs.Y - lhs.Y * rhs.X);

        public static Vector3f Max(Vector3f a, Vector3f b) => new Vector3f(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

        public static Vector3f Max(Vector3f a, float b) => new Vector3f(Math.Max(a.X, b), Math.Max(a.Y, b), Math.Max(a.Z, b));

        public static Vector3f Max(float a, Vector3f b) => new Vector3f(Math.Max(a, b.X), Math.Max(a, b.Y), Math.Max(a, b.Z));

        public static Vector3f Max(Vector3f a, int b) => new Vector3f(Math.Max(a.X, b), Math.Max(a.Y, b), Math.Max(a.Z, b));

        public static Vector3f Max(int a, Vector3f b) => new Vector3f(Math.Max(a, b.X), Math.Max(a, b.Y), Math.Max(a, b.Z));

        public static Vector3f Min(Vector3f a, Vector3f b) => new Vector3f(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));

        public static Vector3f Min(Vector3f a, float b) => new Vector3f(Math.Min(a.X, b), Math.Min(a.Y, b), Math.Min(a.Z, b));

        public static Vector3f Min(float a, Vector3f b) => new Vector3f(Math.Min(a, b.X), Math.Min(a, b.Y), Math.Min(a, b.Z));

        public static Vector3f Min(Vector3f a, int b) => new Vector3f(Math.Min(a.X, b), Math.Min(a.Y, b), Math.Min(a.Z, b));

        public static Vector3f Min(int a, Vector3f b) => new Vector3f(Math.Min(a, b.X), Math.Min(a, b.Y), Math.Min(a, b.Z));

        public static bool IsApproximatelyEqual(Vector3f a, Vector3f b) => a.X.IsApproximatelyEqual(b.X) && a.Y.IsApproximatelyEqual(b.Y) && a.Z.IsApproximatelyEqual(b.Z);

        public static bool IsApproximatelyEqual(Vector3f a, float b) => a.X.IsApproximatelyEqual(b) && a.Y.IsApproximatelyEqual(b) && a.Z.IsApproximatelyEqual(b);

        public static bool IsApproximatelyEqual(float a, Vector3f b) => a.IsApproximatelyEqual(b.X) && a.IsApproximatelyEqual(b.Y) && a.IsApproximatelyEqual(b.Z);

        public static bool IsApproximatelyEqual(Vector3f a, int b) => a.X.IsApproximatelyEqual(b) && a.Y.IsApproximatelyEqual(b) && a.Z.IsApproximatelyEqual(b);

        public static bool IsApproximatelyEqual(int a, Vector3f b) => MathUtils.IsApproximatelyEqual(a, b.X) && MathUtils.IsApproximatelyEqual(a, b.Y) && MathUtils.IsApproximatelyEqual(a, b.Z);
    }
}