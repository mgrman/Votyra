using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Plannar.ImageSamplers;

namespace Votyra.Plannar.Images.Constraints
{
    public interface IImageConstraint2i
    {
        Rect2i Constrain(Direction direction, Rect2i invalidatedCellArea, IImageSampler2i sampler, Matrix<float> editableMatrix);
    }
}