using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IImage2i
    {
        Range1i RangeZ { get; }

        bool AnyData(Range2i range);

        int? Sample(Vector2i point);
    }
}