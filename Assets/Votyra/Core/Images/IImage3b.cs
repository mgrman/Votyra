using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IImage3b
    {
        bool AnyData(Range3i range);

        bool Sample(Vector3i point);
    }
}