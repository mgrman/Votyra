using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface ITerrainConfig
    {
        Vector3i CellInGroupCount { get; }
        bool FlipTriangles { get; }
        bool DrawBounds { get; }
        bool Async { get; }
    }
}