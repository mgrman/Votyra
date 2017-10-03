using UnityEngine;
using Votyra.Models;

namespace Votyra.Images
{
    public interface IImageInvalidatableImage2i
    {
        Rect2i InvalidatedArea { get; }
    }
}