using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IImage3b
    {
        bool Sample(Vector3i point);

        bool AnyData(Range3i range);
    }
}
