using UnityEngine;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IImageInvalidatableImage2i
    {
        Rect2i InvalidatedArea { get; }
    }
}
