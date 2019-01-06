using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IImageInvalidatableImage2
    {
        Range2i InvalidatedArea { get; }
    }
}