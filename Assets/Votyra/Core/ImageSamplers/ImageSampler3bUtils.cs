using Votyra.Core.Models;

namespace Votyra.Core.ImageSamplers
{
    public static class ImageSampler3bUtils
    {
        public static Rect3f ImageToWorld(this IImageSampler3b sampler, Rect3i rect)
        {
            var min = sampler.ImageToWorld(rect.min);
            var max = sampler.ImageToWorld(rect.max);
            return new Rect3f((max + min) / 2, max - min);
        }
    }
}