using Votyra.Models;

namespace Votyra.Images.EditableImages
{
    public interface IEditableImage2i
    {
        IEditableImageAccessor2i RequestAccess(Rect2i area);
    }
}