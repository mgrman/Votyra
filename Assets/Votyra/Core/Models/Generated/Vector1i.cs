using System;
using Votyra.Core.Utils;

#pragma warning disable SA1402

namespace Votyra.Core.Models
{
    public static class Vector1i
    {
        public static readonly int One = 1;
        public static readonly int Zero = 0;
    }

    public static class Vector1iUtils
    {
        public static readonly int PlusOneX = 1;
        public static readonly int MinusOneX = -1;

        public static float Magnitude(this int @this) => (float)Math.Sqrt(@this.SqrMagnitude());

        public static float Normalized(this int @this)
        {
            var magnitude = @this.Magnitude();
            return magnitude <= float.Epsilon ? @this.ToVector1f() : @this / magnitude;
        }

        public static float SqrMagnitude(this int @this) => @this * @this;

        public static int ManhattanMagnitude(this int @this) => @this;

        public static int AreaSum(this int @this) => @this;

        public static int Volume(this int @this) => @this;

        public static bool AllPositive(this int @this) => @this > 0;

        public static bool AllZeroOrPositive(this int @this) => @this >= 0;

        public static bool AnyNegative(this int @this) => @this < 0;

        public static bool AnyZero(this int @this) => @this == 0;

        public static bool AnyZeroOrNegative(this int @this) => @this <= 0;

        public static int FromSame(int value) => value;

        public static float ToVector1f(this int @this) => @this;

        public static Range1i ToRange1i(this int @this) => Range1i.FromMinAndSize(Vector1i.Zero, @this);

        public static Area1i ToArea1i(this int @this) => Area1i.FromMinAndSize(Vector1i.Zero, @this);

        public static Vector2i ToVector2i(this int @this, int x2) => new Vector2i(@this, x2);

        public static Vector2f ToVector2f(this int @this, float x2) => new Vector2f(@this, x2);

        public static Vector3i ToVector3i(this int @this, int x2, int x3) => new Vector3i(@this, x2, x3);

        public static Vector3f ToVector3f(this int @this, float x2, float x3) => new Vector3f(@this, x2, x3);

        public static Vector4i ToVector4i(this int @this, int x2, int x3, int x4) => new Vector4i(@this, x2, x3, x4);

        public static Vector4f ToVector4f(this int @this, float x2, float x3, float x4) => new Vector4f(@this, x2, x3, x4);

        public static Vector5i ToVector5i(this int @this, int x2, int x3, int x4, int x5) => new Vector5i(@this, x2, x3, x4, x5);

        public static Vector5f ToVector5f(this int @this, float x2, float x3, float x4, float x5) => new Vector5f(@this, x2, x3, x4, x5);

        public static Vector6i ToVector6i(this int @this, int x2, int x3, int x4, int x5, int x6) => new Vector6i(@this, x2, x3, x4, x5, x6);

        public static Vector6f ToVector6f(this int @this, float x2, float x3, float x4, float x5, float x6) => new Vector6f(@this, x2, x3, x4, x5, x6);

        public static int Max(int a, int b) => Math.Max(a, b);

        public static int Min(int a, int b) => Math.Min(a, b);

        public static int DivideUp(int a, int b) => a.DivideUp(b);
    }
}
