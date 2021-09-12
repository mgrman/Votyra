using Votyra.Core.Images;
using Votyra.Core.Models;

namespace Votyra.Core.ImageSamplers
{
    public static class ImageSampler2iUtils
    {
        public static SampledData2f SampleCell(this Matrix2<float> image, Vector2i cell)
        {
            var x0Y0 = image.TryGet(cell, 0f);
            var x0Y1 = image.TryGet(new Vector2i(cell.X,cell.Y+1), 0f);
            var x1Y0 = image.TryGet(new Vector2i(cell.X+1, cell.Y), 0f);
            var x1Y1 = image.TryGet(new Vector2i(cell.X+1, cell.Y+1), 0f);

            return new SampledData2f(x0Y0, x0Y1, x1Y0, x1Y1);
        }

        public static SampledData2f SampleCell(this IImage2f image, Vector2i cell)
        {
            var x0Y0 = image.Sample(cell);
            var x0Y1 = image.Sample(new Vector2i(cell.X,cell.Y+1));
            var x1Y0 = image.Sample(new Vector2i(cell.X+1, cell.Y));
            var x1Y1 = image.Sample(new Vector2i(cell.X+1, cell.Y+1));

            return new SampledData2f(x0Y0, x0Y1, x1Y0, x1Y1);
        }
    }
}