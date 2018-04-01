using System;

namespace Votyra.Core.Utils
{
    public static class MathUtils
    {
        public const float Deg2Rad = 0.01745329f;

        public const float Rad2Deg = 57.29578f;

        private static readonly FastNoise _noise = new FastNoise();

        public static float Clip(this float i, float min, float max)
        {
            return i < min ? min : (i > max ? max : i);
        }

        public static float RoundToMultiple(this float val, float multiple)
        {
            return (float)(Math.Round(val / multiple) * multiple);
        }

        public static int CeilToInt(this float f)
        {
            return (int)Math.Ceiling(f);
        }

        public static int FloorToInt(this float f)
        {
            return (int)Math.Floor(f);
        }

        public static int RoundToInt(this float f)
        {
            return (int)Math.Round(f);
        }

        public static int CeilToInt(this double f)
        {
            return (int)Math.Ceiling(f);
        }

        public static int FloorToInt(this double f)
        {
            return (int)Math.Floor(f);
        }

        public static int RoundToInt(this double f)
        {
            return (int)Math.Round(f);
        }

        public static float PerlinNoise(float x, float y)

        {
            return _noise.GetPerlin(x, y);
        }

        public static float PerlinNoise(float x, float y, float z)
        {
            return _noise.GetPerlin(x, y, z);
        }
    }
}