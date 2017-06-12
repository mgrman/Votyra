using Votyra.Models;
using Votyra.Utils;
using UnityEngine;
using System;

namespace Votyra.Images
{
    public class NoiseImage2 : IImage2i
    {
        public Vector3 Offset { get; private set; }

        public Vector3 Scale { get; private set; }

        public NoiseImage2(Vector3 offset, Vector3 scale)
        {
            Offset = offset;
            Scale = scale;
        }

        public Range2i RangeZ { get { return new Range2i(Offset.z.FloorToInt(), Offset.z.CeilToInt() + Scale.z.CeilToInt()); } }

        public int Sample(Vector2i point)
        {
            point = (point / Scale.XY() + Offset.XY()).ToVector2i();

            float value = (float)Mathf.PerlinNoise(point.x, point.y);

            return (int)(value * Scale.z + Offset.z);
        }
    }
}