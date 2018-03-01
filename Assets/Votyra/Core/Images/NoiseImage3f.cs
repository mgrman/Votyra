using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.Images
{
    public class NoiseImage3f : IImage3f
    {
        public Vector3f Offset { get; private set; }

        public Vector3f Scale { get; private set; }

        public NoiseImage3f(Vector3f offset, Vector3f scale)
        {
            Offset = offset;
            Scale = scale;
        }

        public float Sample(Vector3i point)
        {
            var pointf = (point / Scale + Offset);

            float valueXY = MathUtils.PerlinNoise(pointf.x, pointf.y);
            float valueYZ = MathUtils.PerlinNoise(pointf.y, pointf.z);
            float valueZX = MathUtils.PerlinNoise(pointf.z, pointf.x);

            float value = (valueXY + valueYZ + valueZX) / 3;

            return value - 0.5f;
        }
    }
}