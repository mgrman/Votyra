using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IEditableImage2i
    {
        IEditableImageAccessor2f RequestAccess(Range2i area);
    }
}