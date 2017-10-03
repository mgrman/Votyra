using UnityEngine;
using Votyra.Models;

namespace Votyra.Core.Images
{
    public interface IImageInvalidatableImage2i
    {
        Rect2i InvalidatedArea { get; }
    }
}
