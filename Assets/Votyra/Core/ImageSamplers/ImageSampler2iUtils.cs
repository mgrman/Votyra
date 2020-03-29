using Votyra.Core.Images;
using Votyra.Core.Models;

namespace Votyra.Core.ImageSamplers
{
    public static class ImageSampler2iUtils
    {
        public static readonly Vector2i OffsetX0Y1 = new Vector2i(0, 1);
        public static readonly Vector2i OffsetX1Y0 = new Vector2i(1, 0);
        public static readonly Vector2i OffsetX1Y1 = new Vector2i(1, 1);

        public static SampledData2f SampleCell(this float[,] image, Vector2i cell)
        {
            var x0y0 = image.TryGet(cell, 0f);
            var x0y1 = image.TryGet(new Vector2i(cell.X, cell.Y + 1), 0f);
            var x1y0 = image.TryGet(new Vector2i(cell.X + 1, cell.Y), 0f);
            var x1y1 = image.TryGet(new Vector2i(cell.X + 1, cell.Y + 1), 0f);

            return new SampledData2f(x0y0, x0y1, x1y0, x1y1);
        }

        public static SampledData2f SampleCell(this IImage2f image, Vector2i cell)
        {
            var x0y0 = image.Sample(cell);
            var x0y1 = image.Sample(new Vector2i(cell.X, cell.Y + 1));
            var x1y0 = image.Sample(new Vector2i(cell.X + 1, cell.Y));
            var x1y1 = image.Sample(new Vector2i(cell.X + 1, cell.Y + 1));

            return new SampledData2f(x0y0, x0y1, x1y0, x1y1);
        }

        public static SampledMask2e SampleCell(this IMask2e mask, Vector2i cell)
        {
            if (mask == null)
            {
                return new SampledMask2e(MaskValues.Terrain, MaskValues.Terrain, MaskValues.Terrain, MaskValues.Terrain);
            }

            var x0y0 = mask.Sample(cell);
            var x0y1 = mask.Sample(new Vector2i(cell.X, cell.Y + 1));
            var x1y0 = mask.Sample(new Vector2i(cell.X + 1, cell.Y));
            var x1y1 = mask.Sample(new Vector2i(cell.X + 1, cell.Y + 1));

            return new SampledMask2e(x0y0, x0y1, x1y0, x1y1);
        }
    }
}
