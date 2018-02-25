using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Utils;

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

        public static SampledData2i Sample(this IImageSampler2i sampler, IImage2f image, Vector2i pos)
        {
            var x0y0 = image.Sample(sampler.CellToX0Y0(pos));
            var x0y1 = image.Sample(sampler.CellToX0Y1(pos));
            var x1y0 = image.Sample(sampler.CellToX1Y0(pos));
            var x1y1 = image.Sample(sampler.CellToX1Y1(pos));

            return new SampledData2i(x0y0, x0y1, x1y0, x1y1);
        }

        public static int SampleX0Y0(this IImageSampler2i sampler, IImage2f image, Vector2i pos)
        {
            return image.Sample(sampler.CellToX0Y0(pos)).FloorToInt();
        }

        public static int SampleX0Y1(this IImageSampler2i sampler, IImage2f image, Vector2i pos)
        {
            return image.Sample(sampler.CellToX0Y1(pos)).FloorToInt();
        }

        public static int SampleX1Y0(this IImageSampler2i sampler, IImage2f image, Vector2i pos)
        {
            return image.Sample(sampler.CellToX1Y0(pos)).FloorToInt();
        }

        public static int SampleX1Y1(this IImageSampler2i sampler, IImage2f image, Vector2i pos)
        {
            return image.Sample(sampler.CellToX1Y1(pos)).FloorToInt();
        }
    }
}