using TycoonTerrain.Common.Models;

namespace TycoonTerrain.Images
{
    public interface IImage2
    {
        bool IsAnimated { get; }

        Range2 RangeZ { get; }

        float Sample(Vector2 point, float time);
    }
}