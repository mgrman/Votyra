using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.Images
{
    public class NoiseImage2f : IImage2f
    {
        public NoiseImage2f(Vector3f offset, Vector3f scale)
        {
            Offset = offset;
            Scale = scale;
        }

        public Vector3f Offset { get; private set; }

        public Vector3f Scale { get; private set; }
        public Area1f RangeZ { get { return new Area1f(Offset.Z, Offset.Z + Scale.Z); } }

        public float Sample(Vector2i point)
        {
            point = (point / Scale.XY + Offset.XY).RoundToVector2i();

            var value = MathUtils.PerlinNoise(point.X, point.Y);

            return value * Scale.Z + Offset.Z;
        }

        public IPoolableMatrix2<float> SampleArea(Range2i area)
        {
            var min = area.Min;

            var matrix = PoolableMatrix<float>.CreateDirty(area.Size);
            matrix.Size.ForeachPointExlusive(matPoint => { matrix[matPoint] = Sample(matPoint + min); });
            return matrix;
        }

        public bool AnyData(Range2i range)
        {
            return true;
        }
    }
}