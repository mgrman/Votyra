using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IMask2e
    {
        MaskValues Sample(Vector2i point);

        bool AnyData(Range2i range);
    }
}