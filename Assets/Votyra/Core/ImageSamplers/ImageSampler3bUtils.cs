using Votyra.Core.Images;
using Votyra.Core.Models;

namespace Votyra.Core.ImageSamplers
{
    public static class ImageSampler3bUtils
    {
        public static Rect3f ImageToWorld(this IImageSampler3 sampler, Rect3i rect)
        {
            var min = sampler.ImageToWorld(rect.Min);//- Vector3f.One;
            var max = sampler.ImageToWorld(rect.Max);//+ Vector3f.One;
            return Rect3f.FromMinAndSize(min, max - min);
        }

        public static SampledData3b Sample(this IImageSampler3 sampler, IImage3b image, Vector3i pos)
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