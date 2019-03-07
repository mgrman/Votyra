using System;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public static class Vector1f
    {
        public static readonly float One = 1f;
        public static readonly float Zero = 0f;
    }

    public static class Vector1fUtils
    {
        public static bool AnyNegative(this float @this) => @this < 0;

        public static float AreaSum(this float @this) => @this;

        public static float Magnitude(this float @this) => (float) Math.Sqrt(@this.SqrMagnitude());

        public static float Normalized(this float @this)
        {
            var magnitude = @this.Magnitude();
            return magnitude <= float.Epsilon ? @this : @this / magnitude;
        }

        public static float SqrMagnitude(this float @this) => @this * @this;

        public static float FromSame(float value) => value;

        public static float Dot(float a, float b) => (float) ((double) a * b);

        public static float Ceil(this float @this) => @this.CeilToInt();

        public static int CeilToVector1i(this float @this) => @this.CeilToInt();

        public static float Abs(this float @this) => Math.Abs(@this);

        public static float Floor(this float @this) => @this.FloorToInt();

        public static int FloorToVector1i(this float @this) => @this.FloorToInt();

        public static float Round(this float @this) => @this.RoundToInt();

        public static int RoundToVector1i(this float @this) => @this.RoundToInt();

        public static Vector2f ToVector2f(this float @this, float x2) => new Vector2f(@this, x2);

        public static Vector3f ToVector3f(this float @this, float x2, float x3) => new Vector3f(@this, x2, x3);

        public static Vector4f ToVector4f(this float @this, float x2, float x3, float x4) => new Vector4f(@this, x2, x3, x4);

        public static Vector5f ToVector5f(this float @this, float x2, float x3, float x4, float x5) => new Vector5f(@this, x2, x3, x4, x5);

        public static Vector6f ToVector6f(this float @this, float x2, float x3, float x4, float x5, float x6) => new Vector6f(@this, x2, x3, x4, x5, x6);

        public static Area1f ToArea1f(this float @this) => Area1f.FromMinAndSize(Vector1f.Zero, @this);

        public static float Max(float a, float b) => Math.Max(a, b);

        public static float Max(float a, int b) => Math.Max(a, b);

        public static float Max(int a, float b) => Math.Max(a, b);

        public static float Min(float a, float b) => Math.Min(a, b);

        public static float Min(float a, int b) => Math.Min(a, b);

        public static float Min(int a, float b) => Math.Min(a, b);
    }
}