using Votyra.Models;
using Votyra.Images;
using UnityEngine;

namespace Votyra.ImageSamplers
{
    public interface IImageSampler2i
    {
        SampledData2i Sample(IImage2f image, Vector2i offset);


        Vector2 WorldToImage(Vector2 pos);

        int SampleX0Y0(IImage2f image, Vector2i pos);
        int SampleX0Y1(IImage2f image, Vector2i pos);
        int SampleX1Y0(IImage2f image, Vector2i pos);
        int SampleX1Y1(IImage2f image, Vector2i pos);

        Vector2i CellToX0Y0(Vector2i pos);
        Vector2i CellToX0Y1(Vector2i pos);
        Vector2i CellToX1Y0(Vector2i pos);
        Vector2i CellToX1Y1(Vector2i pos);
        Vector2 ImageToWorld(Vector2i pos);
    }
}