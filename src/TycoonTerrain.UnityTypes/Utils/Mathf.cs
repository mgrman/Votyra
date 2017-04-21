using System;

namespace UnityEngine
{
    public static class Mathf
    {
        private static readonly TycoonTerrain.UnityTypes.Utils.PerlinNoise perlin = new TycoonTerrain.UnityTypes.Utils.PerlinNoise(DateTime.Now.Second);

        public static float PerlinNoise(float x, float y)
        {
            return (float)perlin.Noise(x, y,0);
        }
    }
}