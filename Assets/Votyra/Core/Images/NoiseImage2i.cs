using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.Images
{
    public class NoiseImage2i : IImage2i
    {
        public Vector3f Offset { get; private set; }

        public Vector3f Scale { get; private set; }

        public NoiseImage2i(Vector3f offset, Vector3f scale)
        {
            Offset = offset;
            Scale = scale;
        }

        public Range1h RangeZ { get { return new Range1h(((int)Offset.Z).CreateHeight(), ((int)(Offset.Z + Scale.Z)).CreateHeight()); } }

        public Height Sample(Vector2i point)
        {
            point = (point / Scale.XY + Offset.XY).RoundToVector2i();

            var value = MathUtils.PerlinNoise(point.X, point.Y);

            return ((int)(value * Scale.Z + Offset.Z)).CreateHeight();
        }

        public bool AnyData(Range2i range)
        {
            return true;
        }
    }
}