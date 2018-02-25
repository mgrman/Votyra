using Votyra.Core.Models;

namespace Votyra.Plannar.Images
{
    public interface IEditableImage2f
    {
        IEditableImageAccessor2f RequestAccess(Rect2i area);
    }
}