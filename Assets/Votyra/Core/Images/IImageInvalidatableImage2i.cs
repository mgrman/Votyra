using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IImageInvalidatableImage2i
    {
        Range2i InvalidatedArea { get; }
    }
}