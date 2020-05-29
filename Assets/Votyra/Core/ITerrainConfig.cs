using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface ITerrainConfig : ISharedConfig
    {
        Vector3i CellInGroupCount { get; }

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
