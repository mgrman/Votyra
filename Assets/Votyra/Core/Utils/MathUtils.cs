using System;

namespace Votyra.Core.Utils
{
    public static class MathUtils
    {
        public const float Deg2Rad = 0.01745329f;

        public const float Rad2Deg = 57.29578f;

        private static readonly FastNoise _noise = new FastNoise();

        public static int? Abs(this int? val) => val.HasValue ? Math.Abs(val.Value) : val;

        public static int Abs(this int val) => Math.Abs(val);

        public static int? Sign(this int? val) => val.HasValue ? Math.Sign(val.Value) : val;

        public static int Sign(this int val) => Math.Sign(val);

        public static float? Abs(this float? val) => val.HasValue ? Math.Abs(val.Value) : val;

        public static float Abs(this float val) => Math.Abs(val);

        public static int CeilToInt(this float f)
        {
            if (float.IsPositiveInfinity(f))
                return int.MaxValue;
            if (float.IsNegativeInfinity(f))
                return int.MinValue;
            if (float.IsNaN(f))
                return 0;
            return (int) Math.Ceiling(f);
        }

        public static int CeilToInt(this double f)
        {
            if (double.IsPositiveInfinity(f))
                return int.MaxValue;
            if (double.IsNegativeInfinity(f))
                return int.MinValue;
            if (double.IsNaN(f))
                return 0;
            return (int) Math.Ceiling(f);
        }

        public static float Clip(this float i, float min, float max) => i < min ? min : i > max ? max : i;

        public static int FloorToInt(this float f)
        {
            if (float.IsPositiveInfinity(f))
                return int.MaxValue;
            if (float.IsNegativeInfinity(f))
                return int.MinValue;
            if (float.IsNaN(f))
                return 0;
            return (int) Math.Floor(f);
        }

        public static int FloorToInt(this double f)
        {
            if (double.IsPositiveInfinity(f))
                return int.MaxValue;
            if (double.IsNegativeInfinity(f))
                return int.MinValue;
            if (double.IsNaN(f))
                return 0;
            return (int) Math.Floor(f);
        }

        public static int? Max(this int? a, int? b) => a.HasValue && b.HasValue ? Math.Max(a.Value, b.Value) : a ?? b;

        public static int Max(this int a, int? b) => Math.Max(a, b ?? a);

        public static int Max(this int? a, int b) => Math.Max(a ?? b, b);

        public static int? Min(this int? a, int? b) => a.HasValue && b.HasValue ? Math.Min(a.Value, b.Value) : a ?? b;

        public static int Min(this int a, int? b) => Math.Min(a, b ?? a);

        public static int Min(this int? a, int b) => Math.Min(a ?? b, b);

        public static float PerlinNoise(float x, float y) => _noise.GetPerlin(x, y);

        public static float PerlinNoise(float x, float y, float z) => _noise.GetPerlin(x, y, z);

        public static int RoundToInt(this float f)
        {
            if (float.IsPositiveInfinity(f))
                return int.MaxValue;
            if (float.IsNegativeInfinity(f))
                return int.MinValue;
            if (float.IsNaN(f))
                return 0;
            return (int) Math.Round(f);
        }

        public static int RoundToInt(this double f)
        {
            if (double.IsPositiveInfinity(f))
                return int.MaxValue;
            if (double.IsNegativeInfinity(f))
                return int.MinValue;
            if (double.IsNaN(f))
                return 0;
            return (int) Math.Round(f);
        }

        public static int RoundToInt(this decimal f) => (int) Math.Round(f);

        public static int? RoundToInt(this decimal? f) => f.HasValue ? (int) Math.Round(f.Value) : (int?) null;

        public static float RoundToMultiple(this float val, float multiple) => (float) (Math.Round(val / multiple) * multiple);
    }
}