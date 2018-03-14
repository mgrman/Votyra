using Votyra.Core.Models;

namespace Votyra.Core.ImageSamplers
{
    public interface IImageSampler2i
    {
        Vector2i WorldToImage(Vector2f pos);

        Vector2f ImageToWorld(Vector2i pos);

        Vector2i CellToX0Y0(Vector2i pos);

        Vector2i CellToX0Y1(Vector2i pos);

        Vector2i CellToX1Y0(Vector2i pos);

        Vector2i CellToX1Y1(Vector2i pos);
    }
}