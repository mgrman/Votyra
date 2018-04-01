using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.ImageSamplers
{
    public class SimpleImageSampler2i : IImageSampler2i
    {
        public Vector2i WorldToImage(Vector2f pos)
        {
            return pos.RoundToVector2i();
        }

        public Vector2f ImageToWorld(Vector2i pos)
        {
            return pos.ToVector2f();
        }

        public Vector2i CellToX0Y0(Vector2i pos)
        {
            return new Vector2i(pos.X, pos.Y);
        }

        public Vector2i CellToX0Y1(Vector2i pos)
        {
            return new Vector2i(pos.X, pos.Y + 1);
        }

        public Vector2i CellToX1Y0(Vector2i pos)
        {
            return new Vector2i(pos.X + 1, pos.Y);
        }

        public Vector2i CellToX1Y1(Vector2i pos)
        {
            return new Vector2i(pos.X + 1, pos.Y + 1);
        }
    }
}