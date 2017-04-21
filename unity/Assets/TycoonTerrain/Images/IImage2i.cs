using TycoonTerrain.Common.Models;

namespace TycoonTerrain.Images
{
    public interface IImage2i
    {
        bool IsAnimated { get; }

        Range2i RangeZ { get; }

        int Sample(Vector2i point, float time);
    }
}