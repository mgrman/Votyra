using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Plannar.ImageSamplers;

namespace Votyra.Plannar.Images.Constraints
{
    public interface IImageConstraint2i
    {
        void Constrain(Vector2i cellMin, Vector2i cellMax, IImageSampler2i sampler, IImage2f image, Matrix<float> editableMatrix);
    }
}