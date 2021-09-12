using Votyra.Core.Pooling;

namespace Votyra.Core.Unity.TerrainMeshes
{
    public interface ITerrainMeshFactory
    {
        IPooledTerrainMesh CreatePooledTerrainMesh();
    }
}