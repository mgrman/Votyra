using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Images.Noise
{
    public class NoiseImage2f : IImage2f
    {
        public NoiseImage2f(Vector3f offset, Vector3f scale)
        {
            this.Offset = offset;
            this.Scale = scale;
        }

        public Vector3f Offset { get; }

        public Vector3f Scale { get; }

        public Area1f RangeZ => Area1f.FromMinAndMax(this.Offset.Z, this.Offset.Z + this.Scale.Z);

        public float Sample(Vector2i point)
        {
            point = ((point / this.Scale.XY()) + this.Offset.XY()).RoundToVector2i();

            var value = NoiseUtils.PerlinNoise(point.X, point.Y);

            return (value * this.Scale.Z) + this.Offset.Z;
        }

        public PoolableMatrix2<float> SampleArea(Range2i area)
        {
            var min = area.Min;

            var matrix = PoolableMatrix2<float>.CreateDirty(area.Size);
            var rawMatrix = matrix.RawMatrix;
            for (var ix = 0; ix < rawMatrix.SizeX(); ix++)
            {
                for (var iy = 0; iy < rawMatrix.SizeY(); iy++)
                {
                    var matPoint = new Vector2i(ix, iy);
                    rawMatrix.Set(matPoint, this.Sample(matPoint + min));
                }
            }

            return matrix;
        }

        public bool AnyData(Range2i range) => true;
    }
}
