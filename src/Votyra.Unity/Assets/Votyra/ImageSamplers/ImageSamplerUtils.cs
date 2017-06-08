using Votyra.Common.Models;
using Votyra.Images;
using UnityEngine;

namespace Votyra.ImageSamplers
{
    public static class ImageSamplerUtils
    {
        public static Rect ImageToWorld(this IImageSampler sampler, Rect2i rect)
        {
            var min = sampler.ImageToWorld(rect.min);
            var max = sampler.ImageToWorld(rect.max);
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
    }
}