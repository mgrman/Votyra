using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.Images
{
    public class NoiseImage2f : IImage2f
    {
        public Vector3f Offset { get; private set; }

        public Vector3f Scale { get; private set; }

        public NoiseImage2f(Vector3f offset, Vector3f scale)
        {
            Offset = offset;
            Scale = scale;
        }

        public Range1f RangeZ { get { return new Range1f(Offset.Z, Offset.Z + Scale.Z); } }

        public float Sample(Vector2i point)
        {
            point = (point / Scale.XY + Offset.XY).RoundToVector2i();

            float value = (float)MathUtils.PerlinNoise(point.X, point.Y);

            return value * Scale.Z + Offset.Z;
        }
    }
}