using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IImage2f
    {
        Range1f RangeZ { get; }

        float Sample(Vector2i point);
    }
}