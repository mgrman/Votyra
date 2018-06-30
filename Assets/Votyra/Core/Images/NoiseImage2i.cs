using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.Images
{
    public class NoiseImage2i : IImage2i
    {
        public NoiseImage2i(Vector3f offset, Vector3f scale)
        {
            Offset = offset;
            Scale = scale;
        }

        public Vector3f Offset { get; private set; }

        public Range1i RangeZ { get { return new Range1i((int)Offset.Z, (int)(Offset.Z + Scale.Z)); } }
        public Vector3f Scale { get; private set; }

        public bool AnyData(Range2i range)
        {
            return true;
        }

        public int? Sample(Vector2i point)
        {
            point = (point / Scale.XY + Offset.XY).RoundToVector2i();

            var value = MathUtils.PerlinNoise(point.X, point.Y);

            return (int)(value * Scale.Z + Offset.Z);
        }
    }
}