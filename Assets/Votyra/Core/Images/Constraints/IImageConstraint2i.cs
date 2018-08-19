using Votyra.Core.Models;

namespace Votyra.Core.Images.Constraints
{
    public interface IImageConstraint2i
    {
        Range2i FixImage(Matrix2<Height> editableMatrix, Range2i invalidatedImageArea, Direction direction);
    }
}