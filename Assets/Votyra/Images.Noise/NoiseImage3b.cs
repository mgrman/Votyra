using Votyra.Core.Images;
using Votyra.Core.Models;

namespace Votyra.Images.Noise
{
    public class NoiseImage3b : IImage3b
    {
        public NoiseImage3b(Vector3f offset, Vector3f scale)
        {
            this.Offset = offset;
            this.Scale = scale;
        }

        public Vector3f Offset { get; }

        public Vector3f Scale { get; }

        public bool Sample(Vector3i point)
        {
            var pointf = (point / this.Scale) + this.Offset;

            var valueXy = NoiseUtils.PerlinNoise(pointf.X, pointf.Y);
            var valueYz = NoiseUtils.PerlinNoise(pointf.Y, pointf.Z);
            var valueZx = NoiseUtils.PerlinNoise(pointf.Z, pointf.X);

            var value = (valueXy + valueYz + valueZx) / 3;

            return value > 0.5f;
        }

        public bool AnyData(Range3i range) => true;
    }
}
