using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IImage2i
    {
        Range1f RangeZ { get; }

        int Sample(Vector2i point);
    }
}