using Votyra.Common.Models;
using Votyra.Images;
using UnityEngine;

namespace Votyra.ImageSamplers
{
    public static class ImageSamplerUtils
    {
        public static Rect Transform(this IImageSampler sampler, Rect rect)
        {
            var min = sampler.Transform(rect.min);
            var max = sampler.Transform(rect.max);
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
        public static Rect InverseTransform(this IImageSampler sampler, Rect rect)
        {
            var min = sampler.InverseTransform(rect.min);
            var max = sampler.InverseTransform(rect.max);
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
    }
}