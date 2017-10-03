using Votyra.Models;
using Votyra.Images;
using UnityEngine;

namespace Votyra.Plannar.ImageSamplers
{
    public static class ImageSampler2iUtils
    {
        public static Rect ImageToWorld(this IImageSampler2i sampler, Rect2i rect)
        {
            var min = sampler.ImageToWorld(rect.min);
            var max = sampler.ImageToWorld(rect.max);
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
    }
}
