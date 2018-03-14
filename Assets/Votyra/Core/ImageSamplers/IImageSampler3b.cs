using Votyra.Core.Images;
using Votyra.Core.Models;

namespace Votyra.Core.ImageSamplers
{
    public interface IImageSampler3b
    {
        SampledData3b Sample(IImage3f image, Vector3i offset);

        Vector3i WorldToImage(Vector3f pos);

        Vector3i CellToX0Y0Z0(Vector3i pos);

        Vector3i CellToX0Y0Z1(Vector3i pos);

        Vector3i CellToX0Y1Z0(Vector3i pos);

        Vector3i CellToX0Y1Z1(Vector3i pos);

        Vector3i CellToX1Y0Z0(Vector3i pos);

        Vector3i CellToX1Y0Z1(Vector3i pos);

        Vector3i CellToX1Y1Z0(Vector3i pos);

        Vector3i CellToX1Y1Z1(Vector3i pos);

        Vector3f ImageToWorld(Vector3i pos);
    }
}