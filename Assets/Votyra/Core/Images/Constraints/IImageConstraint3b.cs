using Votyra.Core.Models;

namespace Votyra.Core.Images.Constraints
{
    public interface IImageConstraint3b
    {
        Rect3i FixImage(Matrix3<bool> editableMatrix, Rect3i invalidatedImageArea, Direction direction);
    }
}