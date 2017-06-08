using Votyra.Common.Models;

namespace Votyra.Images.EditableImages
{
    public interface IEditableImage
    {
        IEditableImageAccessor RequestAccess(Rect2i area);

    }
}