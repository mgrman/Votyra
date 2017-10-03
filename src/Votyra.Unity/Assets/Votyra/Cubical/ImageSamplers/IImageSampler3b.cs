using Votyra.Models;
using Votyra.Images;
using UnityEngine;

namespace Votyra.ImageSamplers
{
    public interface IImageSampler3b
    {
        SampledData3b Sample(IImage3f image, Vector3i offset);


        Vector3 WorldToImage(Vector3 pos);

        Vector3i CellToX0Y0Z0(Vector3i pos);
        Vector3i CellToX0Y0Z1(Vector3i pos);
        Vector3i CellToX0Y1Z0(Vector3i pos);
        Vector3i CellToX0Y1Z1(Vector3i pos);
        Vector3i CellToX1Y0Z0(Vector3i pos);
        Vector3i CellToX1Y0Z1(Vector3i pos);
        Vector3i CellToX1Y1Z0(Vector3i pos);
        Vector3i CellToX1Y1Z1(Vector3i pos);
        Vector3 ImageToWorld(Vector3i pos);
    }
}