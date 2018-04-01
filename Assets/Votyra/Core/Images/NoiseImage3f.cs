using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.Images
{
    public class NoiseImage3b : IImage3b
    {
        public Vector3f Offset { get; private set; }

        public Vector3f Scale { get; private set; }

        public NoiseImage3b(Vector3f offset, Vector3f scale)
        {
            Offset = offset;
            Scale = scale;
        }

        public bool Sample(Vector3i point)
        {
            var pointf = (point / Scale + Offset);

            float valueXY = MathUtils.PerlinNoise(pointf.X, pointf.Y);
            float valueYZ = MathUtils.PerlinNoise(pointf.Y, pointf.Z);
            float valueZX = MathUtils.PerlinNoise(pointf.Z, pointf.X);

            float value = (valueXY + valueYZ + valueZX) / 3;

            return value > 0.5f;
        }
    }
}