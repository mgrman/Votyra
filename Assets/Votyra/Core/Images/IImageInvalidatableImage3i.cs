using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IImageInvalidatableImage3i
    {
        Range3i InvalidatedArea { get; }
    }
}