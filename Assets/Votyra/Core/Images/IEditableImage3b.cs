using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IEditableImage3B
    {
        IEditableImageAccessor3B RequestAccess(Range3i area);
    }
}
