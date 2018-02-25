using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.Images
{
    public class NoiseImage2f : IImage2f
    {
        public Vector3 Offset { get; private set; }

        public Vector3 Scale { get; private set; }

        public NoiseImage2f(Vector3 offset, Vector3 scale)
        {
            Offset = offset;
            Scale = scale;
        }

        public Range2 RangeZ { get { return new Range2(Offset.z, Offset.z + Scale.z); } }

        public float Sample(Vector2i point)
        {
            point = (point / Scale.XY() + Offset.XY()).ToVector2i();

            float value = (float)Mathf.PerlinNoise(point.x, point.y);

            return value * Scale.z + Offset.z;
        }
    }
}