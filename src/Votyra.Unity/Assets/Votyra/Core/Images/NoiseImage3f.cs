using Votyra.Models;
using Votyra.Utils;
using UnityEngine;
using System;

namespace Votyra.Core.Images
{
    public class NoiseImage3f : IImage3f
    {
        public Vector3 Offset { get; private set; }

        public Vector3 Scale { get; private set; }

        public NoiseImage3f(Vector3 offset, Vector3 scale)
        {
            Offset = offset;
            Scale = scale;
        }

        public float Sample(Vector3i point)
        {
            var pointf = (point / Scale + Offset);

            float valueXY = Mathf.PerlinNoise(pointf.x, pointf.y);
            float valueYZ = Mathf.PerlinNoise(pointf.y, pointf.z);
            float valueZX = Mathf.PerlinNoise(pointf.z, pointf.x);

            float value = (valueXY + valueYZ + valueZX) / 3;

            return value - 0.5f;
        }
    }
}
