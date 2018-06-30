using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.ImageSamplers
{
    public class DualImageSampler2i : IImageSampler2i
    {
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

        public Vector2f ImageToWorld(Vector2i pos)
        {
            return pos / 2.0f;
        }

        public Vector2i WorldToImage(Vector2f pos)
        {
            return new Vector2i((pos.X * 2).FloorToInt(), (pos.Y * 2).FloorToInt());
        }
    }
}