using TycoonTerrain.Common.Models;
using TycoonTerrain.Common.Utils;

namespace TycoonTerrain.Images
{
    public class NoiseImage : IImage2
    {
        public Vector3 Offset { get; }
        public Vector3 Scale { get; }

        public float TimeRounding { get; }
        public float TimeScale { get; }

        private PerlinNoise _noise;

        public NoiseImage(Vector3 offset,Vector3 scale, float timeRounding, float timeScale)
        {
            Offset = offset;
            Scale = scale;
            TimeRounding = timeRounding;
            TimeScale = timeScale;
            _noise = new PerlinNoise(41634);
        }
        public NoiseImage(Bounds bounds, float timeRounding, float timeScale)
        {
            Offset = bounds.min;
            Scale = bounds.size;
            TimeRounding = timeRounding;
            TimeScale = timeScale;
            _noise = new PerlinNoise(41634);
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

            point = point / Scale.XY + Offset.XY;

            float value = (float)_noise.Noise(point.x, point.y, offsetTime);
         
            return value * Scale.z + Offset.z;
        }
    }
}