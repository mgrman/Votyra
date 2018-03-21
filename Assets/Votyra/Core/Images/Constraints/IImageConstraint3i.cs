using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.ImageSamplers;

namespace Votyra.Core.Images.Constraints
{
    public interface IImageConstraint3i
    {

        Rect3i FixImage(Matrix3<float> editableMatrix, Rect3i invalidatedImageArea, Direction direction);

    }
}