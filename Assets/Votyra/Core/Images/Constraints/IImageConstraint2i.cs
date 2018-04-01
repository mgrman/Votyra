using Votyra.Core.Models;

namespace Votyra.Core.Images.Constraints
{
    public interface IImageConstraint2i
    {
        Rect2i FixImage(Matrix2<float> editableMatrix, Rect2i invalidatedImageArea, Direction direction);
    }
}