using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IImage2i
    {
        Range1hi RangeZ { get; }

        Height1i Sample(Vector2i point);
    }
}