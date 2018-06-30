using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface ITerrainConfig
    {
        bool Async { get; }
        Vector3i CellInGroupCount { get; }
        bool DrawBounds { get; }
        bool FlipTriangles { get; }
    }
}