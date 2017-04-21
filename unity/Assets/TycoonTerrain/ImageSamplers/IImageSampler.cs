using TycoonTerrain.Common.Models;
using TycoonTerrain.Images;

namespace TycoonTerrain.ImageSamplers
{
    public interface IImageSampler
    {
        HeightData Sample(IImage2i image, Vector2i offset, float time);
    }
}