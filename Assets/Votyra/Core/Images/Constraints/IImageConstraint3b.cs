using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.ImageSamplers;

namespace Votyra.Core.Images.Constraints
{
    public interface IImageConstraint3b
    {

        Rect3i FixImage(Matrix3<bool> editableMatrix, Rect3i invalidatedImageArea, Direction direction);

    }
}