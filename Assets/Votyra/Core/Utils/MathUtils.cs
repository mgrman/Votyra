using System;

namespace Votyra.Core.Utils
{
    public static class MathUtils
    {
        public const float Deg2Rad = 0.01745329f;

        public const float Rad2Deg = 57.29578f;

        private static readonly FastNoise _noise = new FastNoise();

        public static int? Abs(this int? val)
        {
            return val.HasValue ? Math.Abs(val.Value) : val;
        }

        public static int Abs(this int val)
        {
            return Math.Abs(val);
        }

        public static int CeilToInt(this float f)
        {
            return (int)Math.Ceiling(f);
        }

        public static int CeilToInt(this double f)
        {
            return (int)Math.Ceiling(f);
        }

        public static float Clip(this float i, float min, float max)
        {
            return i < min ? min : (i > max ? max : i);
        }

        public static int FloorToInt(this float f)
        {
            return (int)Math.Floor(f);
        }

        public static int FloorToInt(this double f)
        {
            return (int)Math.Floor(f);
        }

        public static int? Max(this int? a, int? b)
        {
            return (a.HasValue && b.HasValue) ? Math.Max(a.Value, b.Value) : (a ?? b);
        }

        public static int Max(this int a, int? b)
        {
            return Math.Max(a, b ?? a);
        }

        public static int Max(this int? a, int b)
        {
            return Math.Max(a ?? b, b);
        }

        public static int? Min(this int? a, int? b)
        {
            return (a.HasValue && b.HasValue) ? Math.Min(a.Value, b.Value) : (a ?? b);
        }

        public static int Min(this int a, int? b)
        {
            return Math.Min(a, b ?? a);
        }

        public static int Min(this int? a, int b)
        {
            return Math.Min(a ?? b, b);
        }

        public static float PerlinNoise(float x, float y)

        {
            return _noise.GetPerlin(x, y);
        }

        public static float PerlinNoise(float x, float y, float z)
        {
            return _noise.GetPerlin(x, y, z);
        }

        public static int RoundToInt(this float f)
        {
            return (int)Math.Round(f);
        }

        public static int RoundToInt(this double f)
        {
            return (int)Math.Round(f);
        }

        public static int RoundToInt(this decimal f)
        {
            return (int)Math.Round(f);
        }

        public static int? RoundToInt(this decimal? f)
        {
            return f.HasValue ? (int)Math.Round(f.Value) : (int?)null;
        }

        public static float RoundToMultiple(this float val, float multiple)
        {
            return (float)(Math.Round(val / multiple) * multiple);
        }
    }
}