using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IImage2f
    {
        Area1f RangeZ { get; }

        float Sample(Vector2i point);

        PoolableMatrix2<float> SampleArea(Range2i area);
    }
}