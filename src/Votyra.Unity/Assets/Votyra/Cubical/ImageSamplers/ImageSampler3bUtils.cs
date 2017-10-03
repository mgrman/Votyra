

using UnityEngine;
using Votyra.Core.Models;

namespace Votyra.Cubical.ImageSamplers
{
    public static class ImageSampler3bUtils
    {

        public static Bounds ImageToWorld(this IImageSampler3b sampler, Rect3i rect)
        {
            var min = sampler.ImageToWorld(rect.min);
            var max = sampler.ImageToWorld(rect.max);
            return new Bounds((max + min) / 2, max - min);
        }
    }
}
