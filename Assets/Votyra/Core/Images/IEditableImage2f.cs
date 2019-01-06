using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IEditableImage2f
    {
        IEditableImageAccessor2f RequestAccess(Range2i area);
    }
}