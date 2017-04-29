using System;

namespace UnityEngine
{
    public static class Mathf
    {
        private static readonly Votyra.UnityTypes.Utils.PerlinNoise perlin = new Votyra.UnityTypes.Utils.PerlinNoise(DateTime.Now.Second);

        public static float PerlinNoise(float x, float y)
        {
            return (float)perlin.Noise(x, y,0);
        }
    }
}