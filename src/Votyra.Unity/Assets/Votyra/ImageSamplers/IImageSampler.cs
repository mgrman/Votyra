using Votyra.Common.Models;
using Votyra.Images;
using UnityEngine;

namespace Votyra.ImageSamplers
{
    public interface IImageSampler
    {
        HeightData Sample(IImage2i image, Vector2i offset);

        Vector2 Transform(Vector2 pos);
        Vector2 InverseTransform(Vector2 pos);
    }
}