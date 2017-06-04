using Votyra.Common.Models;
using Votyra.Common.Utils;
using UnityEngine;
using System;

namespace Votyra.Images
{
    public class NoiseImage : IImage2
    {
        public Vector3 Offset { get; private set; }

        public Vector3 Scale { get; private set; }

        public NoiseImage(Vector3 offset, Vector3 scale)
        {
            Offset = offset;
            Scale = scale;
        }

        public NoiseImage(Bounds bounds)
        {
            Offset = bounds.min;
            Scale = bounds.size;
        }

        public Range2 RangeZ { get { return new Range2(Offset.z, Offset.z + Scale.z); } }

        public Rect InvalidatedArea => RectUtils.All;

        public float Sample(Vector2 point)
        {
            point = point.DivideBy(Scale.XY()) + Offset.XY();

            float value = (float)Mathf.PerlinNoise(point.x, point.y);

            return value * Scale.z + Offset.z;
        }
    }
}