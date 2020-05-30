using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IEditableImage2F
    {
        IEditableImageAccessor2F RequestAccess(Range2i area);
    }
}
