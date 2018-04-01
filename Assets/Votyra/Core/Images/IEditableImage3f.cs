using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IEditableImage3b
    {
        IEditableImageAccessor3b RequestAccess(Rect3i area);
    }
}