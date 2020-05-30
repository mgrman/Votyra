using Votyra.Core.Models;

namespace Votyra.Core.Images.Constraints
{
    public interface IImageConstraint3b
    {
        Range3i FixImage(bool[,,] editableMatrix, Range3i invalidatedImageArea, Direction direction);
    }
}
