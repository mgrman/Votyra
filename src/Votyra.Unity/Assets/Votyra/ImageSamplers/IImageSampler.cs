using Votyra.Common.Models;
using Votyra.Images;
using UnityEngine;

namespace Votyra.ImageSamplers
{
    public interface IImageSampler
    {
        HeightData Sample(IImage2i image, Vector2i offset, float time);

        Vector2 TransformPoint(Vector2 pos);
    }
}