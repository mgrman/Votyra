using Votyra.Core.Images;
using Votyra.Core.Models;

namespace Votyra.Core.ImageSamplers
{
    public static class ImageSampler3bUtils
    {
        public static Area3f ImageToWorld(this IImageSampler3 sampler, Range3i rect)
        {
            var min = sampler.ImageToWorld(rect.Min); // - Vector3f.One;
            var max = sampler.ImageToWorld(rect.Max); // + Vector3f.One;
            return Area3f.FromMinAndSize(min, max - min);
        }

        public static SampledData3b Sample(this IImageSampler3 sampler, IImage3b image, Vector3i pos) => new SampledData3b(image.Sample(sampler.CellToX0Y0Z0(pos)), image.Sample(sampler.CellToX0Y0Z1(pos)), image.Sample(sampler.CellToX0Y1Z0(pos)), image.Sample(sampler.CellToX0Y1Z1(pos)), image.Sample(sampler.CellToX1Y0Z0(pos)), image.Sample(sampler.CellToX1Y0Z1(pos)), image.Sample(sampler.CellToX1Y1Z0(pos)), image.Sample(sampler.CellToX1Y1Z1(pos)));

        public static Range3i WorldToImage(this IImageSampler3 sampler, Area3f rect)
        {
            var min = sampler.WorldToImage(rect.Min);
            var max = sampler.WorldToImage(rect.Max);
            return Range3i.FromMinAndMax(min, max + Vector3i.One);
        }
    }
}
