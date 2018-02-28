using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Plannar.ImageSamplers;

namespace Votyra.Plannar.Images.Constraints
{
    public interface IImageConstraint2i
    {

        Rect2i FixImage(Matrix<float> editableMatrix, Rect2i invalidatedImageArea, Direction direction);

    }
}