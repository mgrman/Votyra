using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface ITerrainConfig : IConfig
    {
        Vector3i CellInGroupCount { get; }
        bool FlipTriangles { get; }
        ColliderType ColliderType { get; }
        bool DrawBounds { get; }
        bool AsyncTerrainGeneration { get; }
        bool AsyncInput { get; }
    }

    public enum ColliderType
    {
        None,
        Box,
        Mesh
    }
}