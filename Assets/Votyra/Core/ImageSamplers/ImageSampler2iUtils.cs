using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.ImageSamplers
{
    public static class ImageSampler2iUtils
    {
        public static Range2f ImageToWorld(this IImageSampler2i sampler, Range2i rect)
        {
            var min = sampler.ImageToWorld(rect.Min);
            var max = sampler.ImageToWorld(rect.Max);
            return Range2f.FromMinAndMax(min, max);
        }

        public static Range2i WorldToImage(this IImageSampler2i sampler, Range2f rect)
        {
            var min = sampler.WorldToImage(rect.Min);
            var max = sampler.WorldToImage(rect.Max);
            return Range2i.FromMinAndMax(min, max);
        }

        public static Range2i WorldToImage(this IImageSampler2i sampler, Range2i rect)
        {
            var min = sampler.CellToX0Y0(rect.Min);
            var max = sampler.CellToX1Y1(rect.Max);
            return Range2i.FromMinAndMax(min, max);
        }

        public static SampledData2i Sample(this IImageSampler2i sampler, Matrix2<int> image, Vector2i pos)
        {
            var x0y0 = image.TryGet(sampler.CellToX0Y0(pos), 0);
            var x0y1 = image.TryGet(sampler.CellToX0Y1(pos), 0);
            var x1y0 = image.TryGet(sampler.CellToX1Y0(pos), 0);
            var x1y1 = image.TryGet(sampler.CellToX1Y1(pos), 0);

            return new SampledData2i(x0y0, x0y1, x1y0, x1y1);
        }

        public static SampledData2i Sample(this IImageSampler2i sampler, IImage2i image, Vector2i pos)
        {
            var x0y0 = image.Sample(sampler.CellToX0Y0(pos));
            var x0y1 = image.Sample(sampler.CellToX0Y1(pos));
            var x1y0 = image.Sample(sampler.CellToX1Y0(pos));
            var x1y1 = image.Sample(sampler.CellToX1Y1(pos));

            return new SampledData2i(x0y0, x0y1, x1y0, x1y1);
        }

        public static int SampleX0Y0(this IImageSampler2i sampler, IImage2i image, Vector2i pos)
        {
            return image.Sample(sampler.CellToX0Y0(pos));
        }

        public static int SampleX0Y1(this IImageSampler2i sampler, IImage2i image, Vector2i pos)
        {
            return image.Sample(sampler.CellToX0Y1(pos));
        }

        public static int SampleX1Y0(this IImageSampler2i sampler, IImage2i image, Vector2i pos)
        {
            return image.Sample(sampler.CellToX1Y0(pos));
        }

        public static int SampleX1Y1(this IImageSampler2i sampler, IImage2i image, Vector2i pos)
        {
            return image.Sample(sampler.CellToX1Y1(pos));
        }
    }
}