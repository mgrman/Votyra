using TycoonTerrain.Common.Models;
using TycoonTerrain.Common.Utils;
using UnityEngine;

namespace TycoonTerrain.Images
{
    public class NoiseImage : IImage2
    {
        public Vector3 Offset { get; private set; }
        public Vector3 Scale { get; private set; }

        public float TimeRounding { get; private set; }
        public float TimeScale { get; private set; }

        public NoiseImage(Vector3 offset, Vector3 scale, float timeRounding, float timeScale)
        {
            Offset = offset;
            Scale = scale;
            TimeRounding = timeRounding;
            TimeScale = timeScale;
        }

        public NoiseImage(Bounds bounds, float timeRounding, float timeScale)
        {
            Offset = bounds.min;
            Scale = bounds.size;
            TimeRounding = timeRounding;
            TimeScale = timeScale;
        }

        public Range2 RangeZ { get { return new Range2(Offset.z, Offset.z + Scale.z); } }

        public bool IsAnimated
        {
            get
            {
                return TimeScale != 0;
            }
        }

        public float Sample(Vector2 point, float time)
        {
            float offsetTime = TimeRounding > 0 ? (time * TimeScale).Round(TimeRounding) : time * TimeScale;

            point = point.DivideBy(Scale.XY()) + Offset.XY();

            float value = (float)Mathf.PerlinNoise(point.x, point.y + offsetTime);

            return value * Scale.z + Offset.z;
        }
    }
}