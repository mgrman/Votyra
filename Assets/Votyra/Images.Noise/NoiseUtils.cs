using System;

namespace Votyra.Images.Noise
{
    public static class NoiseUtils
    {
        private static readonly FastNoise Noise = new FastNoise();

        public static float PerlinNoise(float x, float y) => Noise.GetPerlin(x, y);

        public static float PerlinNoise(float x, float y, float z) => Noise.GetPerlin(x, y, z);

    }
}
