using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.ImageSamplers
{
    public class DualImageSampler2i : IImageSampler2i
    {
        public Vector2i WorldToImage(Vector2f pos)
        {
            return new Vector2i((pos.X * 2).FloorToInt(), (pos.Y * 2).FloorToInt());
        }

        public Vector2f ImageToWorld(Vector2i pos)
        {
            return pos / 2.0f;
        }

        public Vector2i CellToX0Y0(Vector2i pos)
        {
            return new Vector2i(pos.X * 2 + 0, pos.Y * 2 + 0);
        }

        public Vector2i CellToX0Y1(Vector2i pos)
        {
            return new Vector2i(pos.X * 2 + 0, pos.Y * 2 + 1);
        }

        public Vector2i CellToX1Y0(Vector2i pos)
        {
            return new Vector2i(pos.X * 2 + 1, pos.Y * 2 + 0);
        }

        public Vector2i CellToX1Y1(Vector2i pos)
        {
            return new Vector2i(pos.X * 2 + 1, pos.Y * 2 + 1);
        }

        public SampledData2i Sample(IImage2f image, Vector2i offset)
        {
            offset = offset + offset;

            var x0y0 = image.Sample(offset);
            var x0y1 = image.Sample(new Vector2i(offset.X, offset.Y + 1));
            var x1y0 = image.Sample(new Vector2i(offset.X + 1, offset.Y));
            var x1y1 = image.Sample(new Vector2i(offset.X + 1, offset.Y + 1));

            return new SampledData2i(x0y0, x0y1, x1y0, x1y1);
        }

        public int SampleX0Y0(IImage2f image, Vector2i pos)
        {
            return image.Sample(new Vector2i(pos.X * 2 + 0, pos.Y * 2 + 0)).FloorToInt();
        }

        public int SampleX0Y1(IImage2f image, Vector2i pos)
        {
            return image.Sample(new Vector2i(pos.X * 2 + 0, pos.Y * 2 + 1)).FloorToInt();
        }

        public int SampleX1Y0(IImage2f image, Vector2i pos)
        {
            return image.Sample(new Vector2i(pos.X * 2 + 1, pos.Y * 2 + 0)).FloorToInt();
        }

        public int SampleX1Y1(IImage2f image, Vector2i pos)
        {
            return image.Sample(new Vector2i(pos.X * 2 + 1, pos.Y * 2 + 1)).FloorToInt();
        }
    }
}