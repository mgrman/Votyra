using Votyra.Core.Pooling;

namespace Votyra.Core.TerrainMeshes
{
    public interface ITerrainMeshFactory
    {
        IPooledTerrainMesh CreatePooledTerrainMesh();
    }
}