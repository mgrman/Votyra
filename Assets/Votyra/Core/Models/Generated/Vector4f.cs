using System;
using Newtonsoft.Json;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Vector4f : IEquatable<Vector4f>
    {
        public static readonly Vector4f One = new Vector4f(1, 1, 1, 1);
        public static readonly Vector4f Zero = new Vector4f(0, 0, 0, 0);
        public static readonly Vector4f NaN = new Vector4f(float.NaN, float.NaN, float.NaN, float.NaN);

        public readonly float X;

        public readonly float Y;

        public readonly float Z;

        public readonly float W;

        [JsonConstructor]
        public Vector4f(float x, float y, float z, float w)
        {
            X = x;

            Y = y;

            Z = z;

            W = w;
        }

        public static Vector4f operator -(Vector4f a) => new Vector4f(-a.X, -a.Y, -a.Z, -a.W);

        public static Vector4f operator *(Vector4f a, Vector4f b) => new Vector4f(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);

        public static Vector4f operator *(Vector4f a, float b) => new Vector4f(a.X * b, a.Y * b, a.Z * b, a.W * b);

        public static Vector4f operator *(float a, Vector4f b) => new Vector4f(a * b.X, a * b.Y, a * b.Z, a * b.W);

        public static Vector4f operator *(Vector4f a, int b) => new Vector4f(a.X * b, a.Y * b, a.Z * b, a.W * b);

        public static Vector4f operator *(int a, Vector4f b) => new Vector4f(a * b.X, a * b.Y, a * b.Z, a * b.W);

        public static Vector4f operator +(Vector4f a, Vector4f b) => new Vector4f(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);

        public static Vector4f operator +(Vector4f a, float b) => new Vector4f(a.X + b, a.Y + b, a.Z + b, a.W + b);

        public static Vector4f operator +(float a, Vector4f b) => new Vector4f(a + b.X, a + b.Y, a + b.Z, a + b.W);

        public static Vector4f operator +(Vector4f a, int b) => new Vector4f(a.X + b, a.Y + b, a.Z + b, a.W + b);

        public static Vector4f operator +(int a, Vector4f b) => new Vector4f(a + b.X, a + b.Y, a + b.Z, a + b.W);

        public static Vector4f operator -(Vector4f a, Vector4f b) => new Vector4f(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);

        public static Vector4f operator -(Vector4f a, float b) => new Vector4f(a.X - b, a.Y - b, a.Z - b, a.W - b);

        public static Vector4f operator -(float a, Vector4f b) => new Vector4f(a - b.X, a - b.Y, a - b.Z, a - b.W);

        public static Vector4f operator -(Vector4f a, int b) => new Vector4f(a.X - b, a.Y - b, a.Z - b, a.W - b);

        public static Vector4f operator -(int a, Vector4f b) => new Vector4f(a - b.X, a - b.Y, a - b.Z, a - b.W);

        public static Vector4f operator /(Vector4f a, Vector4f b) => new Vector4f(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);

        public static Vector4f operator /(Vector4f a, float b) => new Vector4f(a.X / b, a.Y / b, a.Z / b, a.W / b);

        public static Vector4f operator /(float a, Vector4f b) => new Vector4f(a / b.X, a / b.Y, a / b.Z, a / b.W);

        public static Vector4f operator /(Vector4f a, int b) => new Vector4f(a.X / b, a.Y / b, a.Z / b, a.W / b);

        public static Vector4f operator /(int a, Vector4f b) => new Vector4f(a / b.X, a / b.Y, a / b.Z, a / b.W);

        public static Vector4f operator %(Vector4f a, Vector4f b) => new Vector4f(a.X % b.X, a.Y % b.Y, a.Z % b.Z, a.W % b.W);

        public static Vector4f operator %(Vector4f a, float b) => new Vector4f(a.X % b, a.Y % b, a.Z % b, a.W % b);

        public static Vector4f operator %(float a, Vector4f b) => new Vector4f(a % b.X, a % b.Y, a % b.Z, a % b.W);

        public static Vector4f operator %(Vector4f a, int b) => new Vector4f(a.X % b, a.Y % b, a.Z % b, a.W % b);

        public static Vector4f operator %(int a, Vector4f b) => new Vector4f(a % b.X, a % b.Y, a % b.Z, a % b.W);

        public static bool operator <(Vector4f a, Vector4f b) => a.X < b.X && a.Y < b.Y && a.Z < b.Z && a.W < b.W;

        public static bool operator <(Vector4f a, float b) => a.X < b && a.Y < b && a.Z < b && a.W < b;

        public static bool operator <(float a, Vector4f b) => a < b.X && a < b.Y && a < b.Z && a < b.W;

        public static bool operator <(Vector4f a, int b) => a.X < b && a.Y < b && a.Z < b && a.W < b;

        public static bool operator <(int a, Vector4f b) => a < b.X && a < b.Y && a < b.Z && a < b.W;

        public static bool operator >(Vector4f a, Vector4f b) => a.X > b.X && a.Y > b.Y && a.Z > b.Z && a.W > b.W;

        public static bool operator >(Vector4f a, float b) => a.X > b && a.Y > b && a.Z > b && a.W > b;

        public static bool operator >(float a, Vector4f b) => a > b.X && a > b.Y && a > b.Z && a > b.W;

        public static bool operator >(Vector4f a, int b) => a.X > b && a.Y > b && a.Z > b && a.W > b;

        public static bool operator >(int a, Vector4f b) => a > b.X && a > b.Y && a > b.Z && a > b.W;

        public static bool operator <=(Vector4f a, Vector4f b) => a.X <= b.X && a.Y <= b.Y && a.Z <= b.Z && a.W <= b.W;

        public static bool operator <=(Vector4f a, float b) => a.X <= b && a.Y <= b && a.Z <= b && a.W <= b;

        public static bool operator <=(float a, Vector4f b) => a <= b.X && a <= b.Y && a <= b.Z && a <= b.W;

        public static bool operator <=(Vector4f a, int b) => a.X <= b && a.Y <= b && a.Z <= b && a.W <= b;

        public static bool operator <=(int a, Vector4f b) => a <= b.X && a <= b.Y && a <= b.Z && a <= b.W;

        public static bool operator >=(Vector4f a, Vector4f b) => a.X >= b.X && a.Y >= b.Y && a.Z >= b.Z && a.W >= b.W;

        public static bool operator >=(Vector4f a, float b) => a.X >= b && a.Y >= b && a.Z >= b && a.W >= b;

        public static bool operator >=(float a, Vector4f b) => a >= b.X && a >= b.Y && a >= b.Z && a >= b.W;

        public static bool operator >=(Vector4f a, int b) => a.X >= b && a.Y >= b && a.Z >= b && a.W >= b;

        public static bool operator >=(int a, Vector4f b) => a >= b.X && a >= b.Y && a >= b.Z && a >= b.W;

        public static bool operator ==(Vector4f a, Vector4f b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;

        public static bool operator ==(Vector4f a, float b) => a.X == b && a.Y == b && a.Z == b && a.W == b;

        public static bool operator ==(float a, Vector4f b) => a == b.X && a == b.Y && a == b.Z && a == b.W;

        public static bool operator ==(Vector4f a, int b) => a.X == b && a.Y == b && a.Z == b && a.W == b;

        public static bool operator ==(int a, Vector4f b) => a == b.X && a == b.Y && a == b.Z && a == b.W;

        public static bool operator !=(Vector4f a, Vector4f b) => a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.W != b.W;

        public static bool operator !=(Vector4f a, float b) => a.X != b || a.Y != b || a.Z != b || a.W != b;

        public static bool operator !=(float a, Vector4f b) => a != b.X || a != b.Y || a != b.Z || a != b.W;

        public static bool operator !=(Vector4f a, int b) => a.X != b || a.Y != b || a.Z != b || a.W != b;

        public static bool operator !=(int a, Vector4f b) => a != b.X || a != b.Y || a != b.Z || a != b.W;

        public bool Equals(Vector4f other) => this == other;

        public override bool Equals(object obj)
        {
            if (!(obj is Vector4f))
                return false;

            return Equals((Vector4f) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ (Y.GetHashCode() * 397) ^ (Z.GetHashCode() * 397) ^ (W.GetHashCode() * 397);
            }
        }

        public override string ToString() => "(" + X + "," + Y + "," + Z + "," + W + ")";
    }

    public static class Vector4fUtils
    {
        public static Vector2f XX(this Vector4f @this) => new Vector2f(@this.X, @this.X);

        public static Vector2f XY(this Vector4f @this) => new Vector2f(@this.X, @this.Y);

        public static Vector2f XZ(this Vector4f @this) => new Vector2f(@this.X, @this.Z);

        public static Vector2f XW(this Vector4f @this) => new Vector2f(@this.X, @this.W);

        public static Vector2f YX(this Vector4f @this) => new Vector2f(@this.Y, @this.X);

        public static Vector2f YY(this Vector4f @this) => new Vector2f(@this.Y, @this.Y);

        public static Vector2f YZ(this Vector4f @this) => new Vector2f(@this.Y, @this.Z);

        public static Vector2f YW(this Vector4f @this) => new Vector2f(@this.Y, @this.W);

        public static Vector2f ZX(this Vector4f @this) => new Vector2f(@this.Z, @this.X);

        public static Vector2f ZY(this Vector4f @this) => new Vector2f(@this.Z, @this.Y);

        public static Vector2f ZZ(this Vector4f @this) => new Vector2f(@this.Z, @this.Z);

        public static Vector2f ZW(this Vector4f @this) => new Vector2f(@this.Z, @this.W);

        public static Vector2f WX(this Vector4f @this) => new Vector2f(@this.W, @this.X);

        public static Vector2f WY(this Vector4f @this) => new Vector2f(@this.W, @this.Y);

        public static Vector2f WZ(this Vector4f @this) => new Vector2f(@this.W, @this.Z);

        public static Vector2f WW(this Vector4f @this) => new Vector2f(@this.W, @this.W);

        public static bool AnyNegative(this Vector4f @this) => @this.X < 0 || @this.Y < 0 || @this.Z < 0 || @this.W < 0;

        public static bool AnyNan(this Vector4f @this) => float.IsNaN(@this.X) || float.IsNaN(@this.Y) || float.IsNaN(@this.Z) || float.IsNaN(@this.W);

        public static bool AnyInfinity(this Vector4f @this) => float.IsInfinity(@this.X) || float.IsInfinity(@this.Y) || float.IsInfinity(@this.Z) || float.IsInfinity(@this.W);

        public static float AreaSum(this Vector4f @this) => @this.X * @this.Y * @this.Z * @this.W;

        public static float Magnitude(this Vector4f @this) => (float) Math.Sqrt(@this.SqrMagnitude());

        public static Vector4f Normalized(this Vector4f @this)
        {
            var magnitude = @this.Magnitude();
            return magnitude <= float.Epsilon ? @this : @this / magnitude;
        }

        public static float SqrMagnitude(this Vector4f @this) => @this.X * @this.X + @this.Y * @this.Y + @this.Z * @this.Z + @this.W * @this.W;

        public static Vector4f FromSame(float value) => new Vector4f(value, value, value, value);

        public static float Dot(Vector4f a, Vector4f b) => (float) ((double) a.X * b.X + (double) a.Y * b.Y + (double) a.Z * b.Z + (double) a.W * b.W);

        public static Vector4f Ceil(this Vector4f @this) => new Vector4f(@this.X.CeilToInt(), @this.Y.CeilToInt(), @this.Z.CeilToInt(), @this.W.CeilToInt());

        public static Vector4i CeilToVector4i(this Vector4f @this) => new Vector4i(@this.X.CeilToInt(), @this.Y.CeilToInt(), @this.Z.CeilToInt(), @this.W.CeilToInt());

        public static Vector4f Abs(this Vector4f @this) => new Vector4f(Math.Abs(@this.X), Math.Abs(@this.Y), Math.Abs(@this.Z), Math.Abs(@this.W));

        public static Vector4f Floor(this Vector4f @this) => new Vector4f(@this.X.FloorToInt(), @this.Y.FloorToInt(), @this.Z.FloorToInt(), @this.W.FloorToInt());

        public static Vector4i FloorToVector4i(this Vector4f @this) => new Vector4i(@this.X.FloorToInt(), @this.Y.FloorToInt(), @this.Z.FloorToInt(), @this.W.FloorToInt());

        public static Vector4f Round(this Vector4f @this) => new Vector4f(@this.X.RoundToInt(), @this.Y.RoundToInt(), @this.Z.RoundToInt(), @this.W.RoundToInt());

        public static Vector4i RoundToVector4i(this Vector4f @this) => new Vector4i(@this.X.RoundToInt(), @this.Y.RoundToInt(), @this.Z.RoundToInt(), @this.W.RoundToInt());

        public static Vector5f ToVector5f(this Vector4f @this, float x5) => new Vector5f(@this.X, @this.Y, @this.Z, @this.W, x5);

        public static Vector6f ToVector6f(this Vector4f @this, float x5, float x6) => new Vector6f(@this.X, @this.Y, @this.Z, @this.W, x5, x6);

        public static Area4f ToArea4f(this Vector4f @this) => Area4f.FromMinAndSize(Vector4f.Zero, @this);

        public static Vector4f Max(Vector4f a, Vector4f b) => new Vector4f(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z), Math.Max(a.W, b.W));

        public static Vector4f Max(Vector4f a, float b) => new Vector4f(Math.Max(a.X, b), Math.Max(a.Y, b), Math.Max(a.Z, b), Math.Max(a.W, b));

        public static Vector4f Max(float a, Vector4f b) => new Vector4f(Math.Max(a, b.X), Math.Max(a, b.Y), Math.Max(a, b.Z), Math.Max(a, b.W));

        public static Vector4f Max(Vector4f a, int b) => new Vector4f(Math.Max(a.X, b), Math.Max(a.Y, b), Math.Max(a.Z, b), Math.Max(a.W, b));

        public static Vector4f Max(int a, Vector4f b) => new Vector4f(Math.Max(a, b.X), Math.Max(a, b.Y), Math.Max(a, b.Z), Math.Max(a, b.W));

        public static Vector4f Min(Vector4f a, Vector4f b) => new Vector4f(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z), Math.Min(a.W, b.W));

        public static Vector4f Min(Vector4f a, float b) => new Vector4f(Math.Min(a.X, b), Math.Min(a.Y, b), Math.Min(a.Z, b), Math.Min(a.W, b));

        public static Vector4f Min(float a, Vector4f b) => new Vector4f(Math.Min(a, b.X), Math.Min(a, b.Y), Math.Min(a, b.Z), Math.Min(a, b.W));

        public static Vector4f Min(Vector4f a, int b) => new Vector4f(Math.Min(a.X, b), Math.Min(a.Y, b), Math.Min(a.Z, b), Math.Min(a.W, b));

        public static Vector4f Min(int a, Vector4f b) => new Vector4f(Math.Min(a, b.X), Math.Min(a, b.Y), Math.Min(a, b.Z), Math.Min(a, b.W));

        public static bool IsApproximatelyEqual(Vector4f a, Vector4f b) => a.X.IsApproximatelyEqual(b.X) && a.Y.IsApproximatelyEqual(b.Y) && a.Z.IsApproximatelyEqual(b.Z) && a.W.IsApproximatelyEqual(b.W);

        public static bool IsApproximatelyEqual(Vector4f a, float b) => a.X.IsApproximatelyEqual(b) && a.Y.IsApproximatelyEqual(b) && a.Z.IsApproximatelyEqual(b) && a.W.IsApproximatelyEqual(b);

        public static bool IsApproximatelyEqual(float a, Vector4f b) => a.IsApproximatelyEqual(b.X) && a.IsApproximatelyEqual(b.Y) && a.IsApproximatelyEqual(b.Z) && a.IsApproximatelyEqual(b.W);

        public static bool IsApproximatelyEqual(Vector4f a, int b) => a.X.IsApproximatelyEqual(b) && a.Y.IsApproximatelyEqual(b) && a.Z.IsApproximatelyEqual(b) && a.W.IsApproximatelyEqual(b);

        public static bool IsApproximatelyEqual(int a, Vector4f b) => MathUtils.IsApproximatelyEqual(a, b.X) && MathUtils.IsApproximatelyEqual(a, b.Y) && MathUtils.IsApproximatelyEqual(a, b.Z) && MathUtils.IsApproximatelyEqual(a, b.W);
    }
}