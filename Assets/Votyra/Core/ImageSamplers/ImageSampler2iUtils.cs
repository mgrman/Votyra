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

        public static Range2i WorldToImage(this IImageSampler2i sampler, Range2i rect)
        {
            var min = sampler.CellToX0Y0(rect.Min);
            var max = sampler.CellToX1Y1(rect.Max);
            return Range2i.FromMinAndMax(min, max + Vector2i.One);
        }

        public static SampledData2i Sample(this IImageSampler2i sampler, Matrix2<Height> image, Vector2i pos)
        {
            var x0y0 = image.TryGet(sampler.CellToX0Y0(pos), Height.Default);
            var x0y1 = image.TryGet(sampler.CellToX0Y1(pos), Height.Default);
            var x1y0 = image.TryGet(sampler.CellToX1Y0(pos), Height.Default);
            var x1y1 = image.TryGet(sampler.CellToX1Y1(pos), Height.Default);

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

        public static SampledMask2e Sample(this IImageSampler2i sampler, IMask2e mask, Vector2i pos)
        {
            if (mask == null)
            {
                return new SampledMask2e(MaskValues.Terrain, MaskValues.Terrain, MaskValues.Terrain, MaskValues.Terrain);
            }
            var x0y0 = mask.Sample(sampler.CellToX0Y0(pos));
            var x0y1 = mask.Sample(sampler.CellToX0Y1(pos));
            var x1y0 = mask.Sample(sampler.CellToX1Y0(pos));
            var x1y1 = mask.Sample(sampler.CellToX1Y1(pos));

            return new SampledMask2e(x0y0, x0y1, x1y0, x1y1);
        }

        public static Height SampleX0Y0(this IImageSampler2i sampler, IImage2i image, Vector2i pos)
        {
            return image.Sample(sampler.CellToX0Y0(pos));
        }

        public static Height SampleX0Y1(this IImageSampler2i sampler, IImage2i image, Vector2i pos)
        {
            return image.Sample(sampler.CellToX0Y1(pos));
        }

        public static Height SampleX1Y0(this IImageSampler2i sampler, IImage2i image, Vector2i pos)
        {
            return image.Sample(sampler.CellToX1Y0(pos));
        }

        public static Height SampleX1Y1(this IImageSampler2i sampler, IImage2i image, Vector2i pos)
        {
            return image.Sample(sampler.CellToX1Y1(pos));
        }
    }
}