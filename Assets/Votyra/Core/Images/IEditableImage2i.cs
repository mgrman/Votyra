using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IEditableImage2i
    {
        IEditableImageAccessor2i RequestAccess(Range2i area);
    }
}