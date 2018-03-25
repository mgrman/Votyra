using Votyra.Core.Images;
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

        public static SampledData3b Sample(this IImageSampler3b sampler, IImage3f image, Vector3i pos)
        {
            return new SampledData3b(
                image.Sample(sampler.CellToX0Y0Z0(pos)),
                image.Sample(sampler.CellToX0Y0Z1(pos)),
                image.Sample(sampler.CellToX0Y1Z0(pos)),
                image.Sample(sampler.CellToX0Y1Z1(pos)),
                image.Sample(sampler.CellToX1Y0Z0(pos)),
                image.Sample(sampler.CellToX1Y0Z1(pos)),
                image.Sample(sampler.CellToX1Y1Z0(pos)),
                image.Sample(sampler.CellToX1Y1Z1(pos)));
        }
    }
}