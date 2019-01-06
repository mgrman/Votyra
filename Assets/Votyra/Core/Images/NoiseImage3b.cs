using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.Images
{
    public class NoiseImage3b : IImage3b
    {
        public NoiseImage3b(Vector3f offset, Vector3f scale)
        {
            Offset = offset;
            Scale = scale;
        }

        public Vector3f Offset { get; }

        public Vector3f Scale { get; }

        public bool Sample(Vector3i point)
        {
            var pointf = point / Scale + Offset;

            var valueXY = MathUtils.PerlinNoise(pointf.X, pointf.Y);
            var valueYZ = MathUtils.PerlinNoise(pointf.Y, pointf.Z);
            var valueZX = MathUtils.PerlinNoise(pointf.Z, pointf.X);

            var value = (valueXY + valueYZ + valueZX) / 3;

            return value > 0.5f;
        }

        public bool AnyData(Range3i range) => true;
    }
}