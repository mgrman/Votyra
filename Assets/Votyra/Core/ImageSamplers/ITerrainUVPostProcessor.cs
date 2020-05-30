using Votyra.Core.Models;

namespace Votyra.Core.ImageSamplers
{
    public interface ITerrainUvPostProcessor
    {
        Vector2f ProcessUv(Vector2f vertex);

        Vector2f ReverseUv(Vector2f vertex);
    }
}
