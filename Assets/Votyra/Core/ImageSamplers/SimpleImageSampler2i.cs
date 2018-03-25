using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.ImageSamplers
{
    public class SimpleImageSampler2i : IImageSampler2i
    {
        public Vector2i WorldToImage(Vector2f pos)
        {
            return new Vector2i(pos.x.RoundToInt(), pos.y.RoundToInt());
        }

        public Vector2f ImageToWorld(Vector2i pos)
        {
            return pos / 1f;
        }

        public Vector2i CellToX0Y0(Vector2i pos)
        {
            return new Vector2i(pos.x, pos.y);
        }

        public Vector2i CellToX0Y1(Vector2i pos)
        {
            return new Vector2i(pos.x, pos.y + 1);
        }

        public Vector2i CellToX1Y0(Vector2i pos)
        {
            return new Vector2i(pos.x + 1, pos.y);
        }

        public Vector2i CellToX1Y1(Vector2i pos)
        {
            return new Vector2i(pos.x + 1, pos.y + 1);
        }
    }
}