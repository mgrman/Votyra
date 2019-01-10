using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.Pooling
{
    public interface IPooledTerrainMeshWithFixedCapacity : IPooledTerrainMesh
    {
        new ITerrainMeshWithFixedCapacity Mesh { get; }
    }
}