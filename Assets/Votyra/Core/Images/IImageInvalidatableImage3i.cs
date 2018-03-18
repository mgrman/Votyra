using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IImageInvalidatableImage3i
    {
        Rect3i InvalidatedArea { get; }
    }
}