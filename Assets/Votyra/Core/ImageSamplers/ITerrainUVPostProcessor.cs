using Votyra.Core.Models;

namespace Votyra.Core.ImageSamplers
{
    public interface ITerrainUVPostProcessor
    {
        Vector2f ProcessUV(Vector2f vertex);

        Vector2f ReverseUV(Vector2f vertex);
    }
}
