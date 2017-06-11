using Votyra.Models;
using Votyra.Images;
using UnityEngine;

namespace Votyra.ImageSamplers
{
    public interface IImageSampler
    {
        HeightData Sample(IImage2i image, Vector2i offset);


        Vector2 WorldToImage(Vector2 pos);

        Vector2i CellToX0Y0(Vector2i pos);
        Vector2i CellToX0Y1(Vector2i pos);
        Vector2i CellToX1Y0(Vector2i pos);
        Vector2i CellToX1Y1(Vector2i pos);
        Vector2 ImageToWorld(Vector2i pos);
    }
}