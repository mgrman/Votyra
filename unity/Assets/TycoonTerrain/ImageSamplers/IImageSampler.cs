using TycoonTerrain.Common.Models;
using TycoonTerrain.Images;
using UnityEngine;

namespace TycoonTerrain.ImageSamplers
{
    public interface IImageSampler
    {
        HeightData Sample(IImage2i image, Vector2i offset, float time);

        Vector2 TransformPoint(Vector2 pos);
    }
}