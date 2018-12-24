using Votyra.Core.Images;
using Votyra.Core.Models;

namespace Votyra.Core.ImageSamplers
{
    public static class ImageSampler2iUtils
    {
        public static readonly Vector2i OffsetX0Y1 = new Vector2i(0, 1);
        public static readonly Vector2i OffsetX1Y0 = new Vector2i(1, 0);
        public static readonly Vector2i OffsetX1Y1 = new Vector2i(1, 1);

        public static Vector2i CellToX0Y0(Vector2i pos) => pos;

        public static Vector2i CellToX0Y1(Vector2i pos) => pos + OffsetX0Y1;

        public static Vector2i CellToX1Y0(Vector2i pos) => pos + OffsetX1Y0;

        public static Vector2i CellToX1Y1(Vector2i pos) => pos + OffsetX1Y1;

        public static SampledData2h SampleCell(this Matrix2<Height> image, Vector2i cell)
        {
            var x0y0 = image.TryGet(ImageSampler2iUtils.CellToX0Y0(cell), Height.Default);
            var x0y1 = image.TryGet(ImageSampler2iUtils.CellToX0Y1(cell), Height.Default);
            var x1y0 = image.TryGet(ImageSampler2iUtils.CellToX1Y0(cell), Height.Default);
            var x1y1 = image.TryGet(ImageSampler2iUtils.CellToX1Y1(cell), Height.Default);

            return new SampledData2h(x0y0, x0y1, x1y0, x1y1);
        }

        public static SampledData2h SampleCell(this IImage2i image, Vector2i cell)
        {
            var x0y0 = image.Sample(ImageSampler2iUtils.CellToX0Y0(cell));
            var x0y1 = image.Sample(ImageSampler2iUtils.CellToX0Y1(cell));
            var x1y0 = image.Sample(ImageSampler2iUtils.CellToX1Y0(cell));
            var x1y1 = image.Sample(ImageSampler2iUtils.CellToX1Y1(cell));

            return new SampledData2h(x0y0, x0y1, x1y0, x1y1);
        }

        public static SampledMask2e SampleCell(this IMask2e mask, Vector2i cell)
        {
            if (mask == null)
            {
                return new SampledMask2e(MaskValues.Terrain, MaskValues.Terrain, MaskValues.Terrain, MaskValues.Terrain);
            }
            var x0y0 = mask.Sample(ImageSampler2iUtils.CellToX0Y0(cell));
            var x0y1 = mask.Sample(ImageSampler2iUtils.CellToX0Y1(cell));
            var x1y0 = mask.Sample(ImageSampler2iUtils.CellToX1Y0(cell));
            var x1y1 = mask.Sample(ImageSampler2iUtils.CellToX1Y1(cell));

            return new SampledMask2e(x0y0, x0y1, x1y0, x1y1);
        }
    }
}