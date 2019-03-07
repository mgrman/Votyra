using Votyra.Core.Models;

namespace Votyra.Core.Images.Constraints
{
    public interface IImageConstraint2i
    {
        Range2i FixImage(float[,] editableMatrix, Range2i invalidatedImageArea, Direction direction);
    }
}