using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IImage2f
    {
        Range2 RangeZ { get; }

        float Sample(Vector2i point);
    }
}